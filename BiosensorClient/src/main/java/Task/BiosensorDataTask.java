package Task;

import javafx.concurrent.Task;
import java.net.Socket;
import java.util.Scanner;

public class BiosensorDataTask extends Task<String[]>
{
    private Socket socket;

    public BiosensorDataTask(Socket socket)
    {
        this.socket = socket;
    }

    @Override
    public String[] call()
    {
        try
        {
            boolean gsr, error;
            boolean hr = gsr = error = false;
            int errors = 0;

            Thread.sleep(300);
            Scanner scanner = new Scanner(socket.getInputStream());

            String[] data;
            String[] values = new String[2];


            while(!error && (!hr || !gsr))
            {
                String input = scanner.nextLine();
                System.out.println(input);
                data = input.split(" ");

                if(data[0].contains("Gsr"))
                {
                    values[0] = data[2];
                    gsr = true;
                }
                else if(data[0].contains("Bvp"))
                {
                    values[1] = data[2];
                    hr = true;
                }
                else
                {
                    errors++;
                    System.out.println(data[0]);
                    if(errors > 5)
                        error = true;
                }
            }

            if(error)
            {
                System.out.println("ESCO");
                return null;
            }
            else
            {
                return values;
            }
        }
        catch(Exception e)
        {
            return null;
        }
    }
}
