package Task;

import javafx.concurrent.Task;

import java.io.PrintWriter;
import java.net.Socket;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.List;
import java.util.Scanner;

public class BiosensorConnect extends Task<String>
{
    private Socket socket;
    private String biosensor;
    private PrintWriter writer;
    private Scanner scanner;

    public BiosensorConnect(Socket socket, String biosensor)
    {
        this.socket = socket;
        this.biosensor = biosensor.split(" ")[0];

    }

    @Override
    protected String call()
    {
        try
        {

            scanner = new Scanner(socket.getInputStream());
            writer = new PrintWriter(socket.getOutputStream());

            writer.println("device_connect " + biosensor);
            writer.flush();
            String result = scanner.nextLine();

            if(result.contains("ERR"))
            {
                return "ERR";
            }
            else
            {
                HttpResponse response = HttpClient.newHttpClient().send(HttpRequest.newBuilder(URI.create("https://us-central1-auispeechvr-93119.cloudfunctions.net/createRoom"))
                        .header("Content-Type", "application/json")
                        .POST(HttpRequest.BodyPublishers.ofString(
                                "{\"room\": \"" + biosensor + "\"}"
                        )).build(), HttpResponse.BodyHandlers.ofString());

                if(response.statusCode() != 200)
                    return "ERR";

                writer.println("device_subscribe ibi ON");
                writer.flush();
                result = scanner.nextLine();
                if(result.contains("ERR"))
                {
                    disconnect();

                    return "ERR";
                }

                writer.println("device_subscribe gsr ON");
                writer.flush();
                result = scanner.nextLine();
                if(result.contains("ERR"))
                {
                    disconnect();

                    return "ERR";
                }

                return "OK";
            }
        }
        catch (Exception e)
        {
            return "ERR";
        }
    }

    private void disconnect()
    {
        int i = 0;
        String result;
        do
        {
            writer.println("device_disconnect");
            writer.flush();
            result = scanner.nextLine();
            i++;
        }while(result.contains("ERR") && i < 5);
    }

    public String getBiosensor()
    {
        return biosensor;
    }
}
