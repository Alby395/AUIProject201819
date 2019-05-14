using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZXing;

public class QRCodeReader : MonoBehaviour
{
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
				ToastManager.Instance.ShowToast("No camera found");
			}
			else
			{
				
				AndroidRuntimePermissions.Permission permission = AndroidRuntimePermissions.RequestPermission("android.permission.CAMERA");

				if (permission == AndroidRuntimePermissions.Permission.Granted)
				{
					_cam = new WebCamTexture(devices[i].name);
					_cam.Play();
					_rawImage.texture = _cam;
					StartCoroutine(Loader());
				}
				else
				{
					ToastManager.Instance.ShowToast("No camera permission");
				}
			}
		}
	}

	
	/// <summary>
	/// Capures the picture from the camera and analyzes it 
	/// </summary>
	public void CapturePicture()
	{
		Result result = _barcodeReader.Decode(_cam.GetPixels32(), _cam.width, _cam.height);

		if (result != null)
		{
			AndroidRuntimePermissions.Permission permission = AndroidRuntimePermissions.RequestPermission("android.permission.INTERNET");
			if(permission == AndroidRuntimePermissions.Permission.Granted)
				FirebaseManager.Instance.SetDatabaseReference(result.Text);
			else
			{
				ToastManager.Instance.ShowToast("No Internet permission");
			}
		}
		else
		{
			ToastManager.Instance.ShowToast("No QR code found");
		}
	}

	/// <summary>
	/// Coroutine that starts loading the next Menu
	/// </summary>
	/// <returns></returns>
	private IEnumerator Loader()
	{
		AsyncOperation async = SceneManager.LoadSceneAsync(1,  LoadSceneMode.Single);
		async.allowSceneActivation = false;

		while (async.progress < 0.9f || !FirebaseManager.Instance.RoomFound())
		{
			yield return null;
		}
		
		async.allowSceneActivation = true;
	}
}
