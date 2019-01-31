package Controller;

import Task.BiosensorDataTask;
import Task.Updater;
import com.google.zxing.BarcodeFormat;
import com.google.zxing.common.BitMatrix;
import com.google.zxing.qrcode.QRCodeWriter;
import javafx.application.Platform;
import javafx.embed.swing.SwingFXUtils;
import javafx.event.ActionEvent;
import javafx.event.Event;
import javafx.fxml.FXML;
import javafx.scene.control.Label;
import javafx.scene.control.Slider;
import javafx.scene.control.TextArea;
import javafx.scene.image.ImageView;
import javafx.stage.Stage;
import javafx.stage.WindowEvent;

import javax.imageio.ImageIO;
import java.awt.*;
import java.awt.image.BufferedImage;
import java.net.Socket;
import java.util.concurrent.Executor;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;


public class BiosensorScreenController
{
    @FXML
    public TextArea questionTextArea;
    @FXML
    private Label HRValue;
    @FXML
    private Label GSRValue;

    @FXML
    private ImageView qrCodeImageView;

    private String biosensor;
    private Socket socket;
    private ExecutorService executor;
    private Updater updater;

    public void initialize()
    {

        Platform.runLater(()->{
            updater = new Updater(socket, HRValue, GSRValue, biosensor);
            updater.initialize();
            HRValue.getScene().getWindow().addEventFilter(WindowEvent.WINDOW_CLOSE_REQUEST, this::closeWindow);

        });
    }

    private void closeWindow(Event event)
    {
        updater.stop();
    }

    public void setSocket(Socket socket)
    {
        this.socket = socket;
    }
    public void setBiosensor(String biosensor)
    {
        this.biosensor = biosensor;

        BufferedImage bufferedImage;
        try
        {
            int width = (int) qrCodeImageView.getFitWidth();
            int height = (int) qrCodeImageView.getFitHeight();

            QRCodeWriter qrCodeWriter = new QRCodeWriter();
            BitMatrix bitMatrix = qrCodeWriter.encode("{\"Room\": \""+biosensor+"\", \"Type\": \"Empatica E4\"}", BarcodeFormat.QR_CODE, width, height);

            bufferedImage = new BufferedImage(width, height, BufferedImage.TYPE_INT_RGB);
            bufferedImage.createGraphics();

            Graphics2D graphics = (Graphics2D) bufferedImage.getGraphics();

            graphics.setColor(Color.WHITE);
            graphics.fillRect(0, 0, width, height);
            graphics.setColor(Color.BLACK);

            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    if (bitMatrix.get(i, j)) {
                        graphics.fillRect(i, j, 1, 1);
                    }
                }
            }
            qrCodeImageView.setImage(SwingFXUtils.toFXImage(bufferedImage, null));

        } catch (Exception e)
        {
            e.printStackTrace();
        }
    }

    public void sendQuestion(ActionEvent actionEvent)
    {
        String string = questionTextArea.getText();
        if(!string.equals(""))
            updater.sendQuestion(string);

        questionTextArea.setText("");
    }
}
