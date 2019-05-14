package Task;

import javafx.application.Platform;
import javafx.concurrent.WorkerStateEvent;
import javafx.event.EventHandler;
import javafx.scene.control.Alert;
import javafx.scene.control.Label;
import java.net.Socket;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class Updater implements EventHandler<WorkerStateEvent>
{
    private Socket socket;

    private Label hr;
    private Label gsr;

    private String biosensor;

    private ExecutorService executor;
    private BiosensorDataTask task;
    private Timer timer;

    private HttpClient httpClient;
    private URI valueUri;
    private URI questionUri;

    public Updater(Socket socket, Label hr, Label gsr, String biosensor)
    {
        this.socket = socket;
        this.hr = hr;
        this.gsr = gsr;
        this.executor = Executors.newSingleThreadExecutor();
        this.biosensor = biosensor;

        valueUri = URI.create("https://us-central1-auispeechvr-93119.cloudfunctions.net/updateValues");
        questionUri = URI.create("https://us-central1-auispeechvr-93119.cloudfunctions.net/addQuestion");
    }

    @Override
    public void handle(WorkerStateEvent event)
    {
        if(task.getValue())
        {
            error();
        }
    }

    public void startNewTask()
    {
        if(!executor.isShutdown())
        {
            task = new BiosensorDataTask(socket, hr, gsr, executor);
            task.setOnSucceeded(this);
            executor.submit(task);
        }
    }

    private void setNewTimer()
    {
        timer = new Timer();
        timer.scheduleAtFixedRate(new TimerTask()
        {
            @Override
            public void run()
            {
                try
                {
                    HttpResponse response = httpClient.send(HttpRequest.newBuilder(valueUri)
                            .header("Content-Type", "application/json")
                            .POST(HttpRequest.BodyPublishers.ofString(
                                    "{\"room\": \"" + "PcClientSender_EmpaticaE4_" + biosensor + "\", \"hr\":\"" + hr.getText() + "\", \"gsr\":\"" + gsr.getText() +"\"}"
                            )).build(), HttpResponse.BodyHandlers.ofString());
                    if(response.statusCode() != 200)
                    {
                        Platform.runLater(() -> error());
                    }
                } catch (Exception e)
                {
                    Platform.runLater(() -> error());
                }
            }
        },1000, 2000);
    }

    public void stop()
    {
        timer.cancel();
        executor.shutdown();

        httpClient.sendAsync(HttpRequest.newBuilder(URI.create("https://us-central1-auispeechvr-93119.cloudfunctions.net/removeRoom"))
                .header("Content-Type", "application/json")
                .POST(HttpRequest.BodyPublishers.ofString(
                        "{\"room\":\"" + "PcClientSender_EmpaticaE4_" + biosensor + "\"}"
                )).build(), HttpResponse.BodyHandlers.ofString());
    }

    private void error()
    {
        stop();
        Alert alert = new Alert(Alert.AlertType.ERROR);
        alert.setTitle("Error");
        alert.setHeaderText("Connection error");
        alert.setContentText("Something happened with the biosensor");
        alert.showAndWait();
        Platform.exit();
    }

    public void initialize()
    {
        startNewTask();
        httpClient = HttpClient.newHttpClient();
        setNewTimer();
    }

    public void sendQuestion(String question)
    {
        httpClient.sendAsync(HttpRequest.newBuilder(questionUri)
                .header("Content-Type", "application/json")
                .POST(HttpRequest.BodyPublishers.ofString(
                        "{\"room\":\"" + "PcClientSender_EmpaticaE4_" + biosensor + "\", \"question\":\"" + question + "\"}"
                )).build(), HttpResponse.BodyHandlers.ofString()).thenAcceptAsync(response ->
                    {
                        if(response.statusCode() != 200)
                            Platform.runLater(this::error);
                    });
    }
}
