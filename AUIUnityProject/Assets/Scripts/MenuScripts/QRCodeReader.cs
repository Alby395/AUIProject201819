using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class QRCodeReader : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _text;

	private RawImage _rawImage;
	private WebCamTexture _cam;
	private BarcodeReader _barcodeReader;
	
	private void Start ()
	{
		Screen.orientation = ScreenOrientation.Portrait;
		_rawImage = GetComponent<RawImage>();
		_barcodeReader = new BarcodeReader();
		
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
					_rawImage.texture = _cam;	
				}
				else
				{
					_text.text = "Nope";
				}
			}
		}
	}

	public void CapturePicture()
	{
		Result result = _barcodeReader.Decode(_cam.GetPixels32(), _cam.width, _cam.height);

		if (result != null)
		{
			_text.text = result.Text;
		}
		else
		{
			_text.text = "Error";
		}
	}
}
