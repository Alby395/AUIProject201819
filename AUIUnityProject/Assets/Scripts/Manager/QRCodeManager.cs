using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;


public class QRCodeManager : MonoBehaviour
{
	[SerializeField] private RawImage _image;

	[SerializeField] private TextMeshProUGUI _text;

	private WebCamTexture _cam;

	
	private void Start ()
	{
		Screen.orientation = ScreenOrientation.Portrait;

		WebCamDevice[] devices = WebCamTexture.devices;
		
		
		if (devices.Length > 0)
		{
			int i = 0;
			
			while(i < devices.Length && devices[i].isFrontFacing)
			{
				i++;
			}

			if (i == devices.Length)
			{
				_text.text = "No back camera found";
			}
			else
			{
				AndroidRuntimePermissions.Permission permission = AndroidRuntimePermissions.RequestPermission("android.permission.CAMERA");

				if (permission == AndroidRuntimePermissions.Permission.Granted)
				{
					_cam = new WebCamTexture(devices[i].name);
					_cam.Play();
					_image.texture = _cam;	
				}
				else
				{
					_text.text = "NOPE";
				}
			}
		}
	}

	public void CapturePicture()
	{
		BarcodeReader barcodeReader = new BarcodeReader();

		Result result = barcodeReader.Decode(_cam.GetPixels32(), _cam.width, _cam.height);

		if (result != null)
		{
			_text.text = result.Text;
		}
	}
}
