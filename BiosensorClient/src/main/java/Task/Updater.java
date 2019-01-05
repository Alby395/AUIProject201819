package Task;

import javafx.application.Platform;
import javafx.concurrent.Task;
import javafx.concurrent.WorkerStateEvent;
import javafx.event.EventHandler;
import javafx.scene.control.Alert;
import javafx.scene.control.Label;
import java.net.Socket;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class Updater implements EventHandler<WorkerStateEvent>
{
    private Socket socket;

    private Label hr;
    private Label gsr;
    private Label update;

    private ExecutorService executor;
    private BiosensorDataTask task;

    public Updater(Socket socket, Label hr, Label gsr, Label update)
    {
        this.socket = socket;
        this.hr = hr;
        this.gsr = gsr;
        this.update = update;
        this.executor = Executors.newSingleThreadExecutor();
    }

    @Override
    public void handle(WorkerStateEvent event)
    {
        String[] result = task.getValue();
        if(result == null)
        {
            Alert alert = new Alert(Alert.AlertType.ERROR);
            alert.setTitle("Error");
            alert.setHeaderText("Connection error");
            alert.setContentText("Something happened with the biosensor");
            alert.showAndWait();
            Platform.exit();
        }
        else
        {
            gsr.setText(result[0]);
            hr.setText(result[1]);

            startNewTask();
        }
    }

    public void startNewTask()
    {
        if(!executor.isShutdown())
        {
            task = new BiosensorDataTask(socket);
            task.setOnSucceeded(this);
            executor.submit(task);
        }
    }

    private void setNewTimer()
    {
        System.out.println(executor.isShutdown());
        if(!executor.isShutdown())
        {
           new Timer().schedule(new TimerTask()
            {
                @Override
                public void run()
                {
                    //TODO Add connection to server
                    setNewTimer();
                }
            }, Integer.decode(update.getText()) * 1000);
        }
    }

    public void stop()
    {
        executor.shutdown();
    }
}
