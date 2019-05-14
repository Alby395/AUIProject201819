package Task;

import javafx.application.Platform;
import javafx.concurrent.Task;
import javafx.scene.control.Label;

import java.io.InputStream;
import java.net.Socket;
import java.util.Scanner;
import java.util.concurrent.ExecutorService;

public class BiosensorDataTask extends Task<Boolean>
{
    private Socket socket;
    private Label hrLabel;
    private Label gsrLabel;

    private ExecutorService executor;
    private InputStream stream;
    private String gsrValue, hrValue;
    private boolean printed;

    public BiosensorDataTask(Socket socket, Label hrLabel, Label gsrLabel, ExecutorService executor)
    {
        this.socket = socket;
        this.hrLabel = hrLabel;
        this.gsrLabel = gsrLabel;
        this.executor = executor;
    }

    @Override
    public Boolean call()
    {
        try
        {
            socket.setSoTimeout(10000);
            boolean gsr, hr;

            int errors = 0;
            stream = socket.getInputStream();

            do
            {
                Scanner scanner = new Scanner(stream);
                hr = gsr = printed = false;
                String[] data;

                while(!hr || !gsr)
                {
                    String input = scanner.nextLine();
                    System.out.println(input);
                    data = input.split(" ");

                    if(data[0].contains("Gsr"))
                    {
                        gsrValue = data[2];
                        gsr = true;
                        errors = 0;
                    }
                    else if(data[0].contains("Bvp"))
                    {
                        hrValue = data[2];
                        hr = true;
                        errors = 0;
                    }
                    else
                    {
                        errors++;
                        System.out.println(data[0]);
                        if(errors > 5)
                            return true;
                    }
                }

                Platform.runLater(() ->
                {
                    gsrLabel.setText(getGsrValue());
                    hrLabel.setText(String.valueOf(Math.round(60/Float.parseFloat(getHrValue()))));
                    setPrinted();
                });

                Thread.sleep(300);
                while(!printed){}

                System.out.println("FUORI");

            }while(!executor.isShutdown());

            return false;
        }
        catch(Exception e)
        {
            return true;
        }
    }

    public String getGsrValue()
    {
        return gsrValue;
    }

    public String getHrValue()
    {
        return hrValue;
    }

    private void setPrinted()
    {
        printed = true;
        System.out.println("settato");
    }
}
