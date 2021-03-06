import java.io.IOException;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Scanner;

public class TempServer
{
    public static void main(String[] args)
    {
        try
        {
            ServerSocket socket = new ServerSocket(3000);

            Socket client = socket.accept();

            System.out.println("Connected");
            Scanner scanner = new Scanner(client.getInputStream());
            PrintWriter writer = new PrintWriter(client.getOutputStream());

            System.out.println(scanner.nextLine());
            writer.println("R device_list 2 | 9ff167 Empatica_E4 | 7a3166 Empatica_E4");
            writer.flush();

            System.out.println(scanner.nextLine());
            writer.println("R device_connect OK");
            writer.flush();

            System.out.println(scanner.nextLine());
            writer.println("R device_subscribe bvp OK");
            writer.flush();

            System.out.println(scanner.nextLine());
            writer.println("R device_subscribe gsr OK");
            writer.flush();

            int gsr = 20;
            int hr = 60;
            int i = 0;
            boolean check = false;

            while(true)
            {
                if(!check)
                {
                    writer.println("E4_Bvp 123345627891.123 " + hr);
                    writer.flush();

                    writer.println("E4_Gsr 123345627891.123 " + gsr);
                    writer.flush();

                    if (i < 10)
                    {
                        gsr++;
                        hr++;
                        i++;
                    } else
                    {
                        i = 0;
                        gsr = 20;
                        hr = 60;
                    }

                    if(i%3 == 0)
                    {
                        check = true;
                    }
                }

                if(check)
                {
                    System.out.println("Checking");
                    String string = scanner.nextLine();
                    if ("pause ON".equals(string))
                    {
                        System.out.println("Stop");
                        check = true;
                        writer.println("R pause ON");
                        writer.flush();
                    } else if ("pause OFF".equals(string))
                    {
                        System.out.println("Go");
                        check = false;
                        writer.println("R pause OFF");
                        writer.flush();
                    }
                }

            }

        } catch (IOException e)
        {
            e.printStackTrace();
        }
    }

}
