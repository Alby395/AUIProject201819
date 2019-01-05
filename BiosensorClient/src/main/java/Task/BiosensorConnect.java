package Task;

import javafx.concurrent.Task;

import java.io.PrintWriter;
import java.net.Socket;
import java.util.List;
import java.util.Scanner;

public class BiosensorConnect extends Task<String>
{
    private Socket socket;
    private String biosensorCode;
    private String biosensorType;
    private PrintWriter writer;
    private Scanner scanner;

    public BiosensorConnect(Socket socket, String biosensor)
    {
        this.socket = socket;
        String[] strings = biosensor.split(" ");
        this.biosensorCode = strings[1];
        this.biosensorType = strings[2];
    }

    @Override
    protected String call()
    {
        try
        {
            scanner = new Scanner(socket.getInputStream());
            writer = new PrintWriter(socket.getOutputStream());

            writer.println("device_connect " + biosensorCode);
            writer.flush();
            String result = scanner.nextLine();

            if(result.contains("ERR"))
            {
                return "ERR";
            }
            else
            {
                writer.println("device_subscribe bvp ON");
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
        return biosensorCode + " " + biosensorType;
    }
}
