using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

public class FirebaseManager : MonoBehaviour
{
	public static FirebaseManager Instance { get; private set; }
	public bool NewValue { get; private set; }
	
	private QrData _info; 
	private DatabaseReference _reference;

	private bool _stop;
	private bool _ready;
	
	private float _hr;
	private float _gsr;
	
	void Start ()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
			
			FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
				var dependencyStatus = task.Result;
				if (dependencyStatus == DependencyStatus.Available)
				{
					
					Console.WriteLine("OK");
					FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://auispeechvr-93119.firebaseio.com/");

				} else
				{
					//TODO Don't allow to do anything
					Console.WriteLine("ERROR");
					Debug.LogError(String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
				}
			});
			
		}
		else
		{
			Destroy(this);
		}
	}

	public void SetDatabaseReference(String code)
	{
		try
		{
			Console.WriteLine(code);
			_info = JsonUtility.FromJson<QrData>(code);
			Console.WriteLine("/Rooms/" + _info.Room);

			FirebaseDatabase.DefaultInstance.GetReference("/Rooms/" + _info.Room).GetValueAsync().ContinueWith(
				task =>
				{
					if (task.IsFaulted)
					{
						ToastManager.Instance.ShowToast("Error");
					}

					if (task.Result != null)
					{
						_reference = FirebaseDatabase.DefaultInstance.GetReference("/Rooms/" + _info.Room);
						SceneManager.LoadScene("MainMenuScene");
					}
					else
					{
						ToastManager.Instance.ShowToast("No room found");
					}
				});
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}

	public void StartReading()
	{
		_reference.ValueChanged += ReadValuesDatabase;
	}

	void ReadValuesDatabase(object sender, ValueChangedEventArgs args)
	{
		if (args.DatabaseError != null)
		{
			//TODO ERROR
			return;
		}

		DataSnapshot snapshot = args.Snapshot;

		if (snapshot != null && snapshot.ChildrenCount > 0)
		{
			_hr = float.Parse(snapshot.Child("hr").Value.ToString());
			_gsr =  float.Parse(snapshot.Child("gsr").Value.ToString());

			NewValue = true;
			Console.WriteLine("New Value");
		}
		else
		{
			Console.WriteLine("ERROR");
		}
	}
	
	public float GetHR()
	{
		return _hr;
	}

	public float GetGsr()
	{
		return _gsr;
	}

	public void ValuesRead()
	{
		NewValue = false;
	}

	public void StopReading()
	{
		_reference.ValueChanged -= ReadValuesDatabase;
	}
}
