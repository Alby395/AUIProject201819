using UnityEngine;

public class ToastManager : MonoBehaviour
{
	public static ToastManager Instance {get; private set; }
	private AndroidJavaClass _unityPlayer;
    
	private AndroidJavaObject _currentActivity;
	private string _message;
	private void Start()
	{
		_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(this);
		}
	}

	public void ShowToast(string message)
	{
		_currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

		_message = message;
        
		_currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(ShowToastAndroid));
	}

	private void ShowToastAndroid()
	{
		AndroidJavaObject context =   _currentActivity.Call<AndroidJavaObject>("getApplicationContext");
     
		AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
		AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", _message);
        
		AndroidJavaObject toast = Toast.CallStatic<AndroidJavaObject>("makeText", context, javaString, Toast.GetStatic<int>("LENGTH_LONG"));

		toast.Call ("show");
	}
}
