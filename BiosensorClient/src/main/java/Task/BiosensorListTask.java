package Task;

import javafx.concurrent.Task;

import java.io.PrintWriter;
import java.net.Socket;
import java.util.List;
import java.util.Scanner;

public class BiosensorListTask extends Task<List<String>>
{
    private Socket socket;


    public BiosensorListTask(Socket socket)
    {
        this.socket = socket;
    }

    @Override
    protected List<String> call() throws Exception
    {
        try
        {
            Scanner scanner = new Scanner(socket.getInputStream());
            PrintWriter writer = new PrintWriter(socket.getOutputStream());

            writer.println("device_list");
            writer.flush();
            String string = scanner.nextLine();
            return List.of(string.split("\\|"));
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
