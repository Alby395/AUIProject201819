package Controller;

import Task.BiosensorConnect;
import Task.BiosensorListTask;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Scene;
import javafx.scene.control.Alert;
import javafx.scene.control.ListView;
import javafx.scene.control.TextField;
import javafx.scene.input.KeyEvent;
import javafx.stage.Stage;

import java.io.IOException;
import java.net.Socket;
import java.util.List;
import java.util.regex.Pattern;

public class SelectionScreenController
{
    @FXML
    private TextField portTextField;
    @FXML
    private ListView<String> biosensorList;
    @FXML
    private TextField ipTextField;

    private boolean correctIp;
    private boolean correctPort;

    private Pattern pattern;
    private Socket socket;

    public void initialize()
    {
        correctIp = true;
        correctPort = true;
        ipTextField.setText("127.0.0.1");
        pattern = Pattern.compile("(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)");
    }

    public void checkIP(KeyEvent keyEvent)
    {
        if (pattern.matcher(ipTextField.getText()).matches())
        {
            if (!correctIp)
            {
                correctIp = true;
                ipTextField.setStyle("-fx-border-color: none");
            }
        } else
        {
            if (correctIp)
            {
                correctIp = false;
                ipTextField.setStyle("-fx-border-color: red");
            }
        }
    }

    public void checkPort(KeyEvent keyEvent)
    {
        try
        {
            int port = Integer.decode(portTextField.getText());
            if (port >= 0 && port <= 65535)
            {
                if (!correctPort)
                {
                    correctPort = true;
                    portTextField.setStyle("-fx-border-color: none");
                }
            } else
            {
                if (correctPort)
                {
                    correctPort = false;
                    portTextField.setStyle("-fx-border-color: red");
                }
            }
        } catch (Exception e)
        {
            if (correctPort)
            {
                correctPort = false;
                portTextField.setStyle("-fx-border-color: red");
            }
        }

    }

    public void startSearch(ActionEvent actionEvent)
    {
        if (correctIp && correctPort)
        {
            try
            {
                socket = new Socket(ipTextField.getText(), Integer.decode(portTextField.getText()));
                BiosensorListTask task = new BiosensorListTask(socket);
                task.setOnSucceeded(handler ->
                {
                    List<String> strings = task.getValue();

                    if (strings.size() > 1)
                        biosensorList.getItems().addAll(strings.subList(1, strings.size()));

                });
                Thread thread = new Thread(task);
                thread.run();
            } catch (IOException e)
            {
                e.printStackTrace();
            }
        }
    }

    public void createRoom(ActionEvent actionEvent)
    {
        String string = biosensorList.getSelectionModel().getSelectedItem();
        BiosensorConnect task = new BiosensorConnect(socket, string);
        task.setOnSucceeded(handler -> {
            String result = task.getValue();

            if(result.contains("ERR"))
            {
                Alert alert = new Alert(Alert.AlertType.ERROR);
                alert.setTitle("Connection Error");
                alert.setHeaderText("Error during connection");
                alert.setContentText("Something happened during the connection with the biosensor.");
                alert.show();
            }
            else if (result.contains("OK"))
            {
                try
                {
                    FXMLLoader loader = new FXMLLoader(getClass().getResource("BiosensorScreen.fxml"));
                    Scene scene = new Scene(loader.load());

                    BiosensorScreenController controller = loader.getController();
                    controller.setSocket(socket);
                    controller.setBiosensor(task.getBiosensor());

                    Stage stage = new Stage();
                    stage.setScene(scene);
                    stage.setResizable(false);
                    stage.centerOnScreen();
                    stage.show();

                    portTextField.getScene().getWindow().hide();
                }
                catch (Exception e)
                {
                    e.printStackTrace();
                }
            }
        });
        Thread thread = new Thread(task);
        thread.run();
    }
}
