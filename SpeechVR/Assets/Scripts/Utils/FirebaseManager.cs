using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
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
	public bool NewHr { get; private set; }
	public bool NewGsr { get; private set; }
	
	[SerializeField] private float baseTimer = 60f;
	private QrData _info; 
	private DatabaseReference _reference;

	private bool _stop;
	private bool _ready;

	private int _hr, _baseHr;
	private float _gsr, _baseGsr;

	private int _hrBaseRange, _hrCurrentRange;

	private bool _lastGsrStatus;
	private Queue<string> _question;

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
					_ready = false;
					FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://auispeechvr-93119.firebaseio.com/");
					_question = new Queue<string>();

				} else
				{
					//TODO Don't allow to do anything
					Console.WriteLine("ERROR");
					Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
				}
			});
			
		}
		else
		{
			Destroy(this);
		}
	}

	/// <summary>
	/// Sets the reference to the database where the data is stored
	/// </summary>
	/// <param name="code">Room to find</param>
	public void SetDatabaseReference(String code)
	{
		try
		{
			Debug.Log(code);

			FirebaseDatabase.DefaultInstance.GetReference("/Rooms/" + code).GetValueAsync().ContinueWith(
				task =>
				{
					if (task.IsFaulted)
					{
						ToastManager.Instance.ShowToast("Error");
					}

					if (task.Result != null)
					{
						_reference = FirebaseDatabase.DefaultInstance.GetReference("/Rooms/" + code);
						_ready = true;
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

	/// <summary>
	/// Returns whether the room was found
	/// </summary>
	/// <returns>whether the room was found</returns>
	public bool RoomFound()
	{
		return _ready;
	}
	
	/// <summary>
	/// Starts reading value from the database.
	/// </summary>
	public void StartReading()
	{
		_reference.Child("hr").ValueChanged += ReadHrDatabase;
		_reference.Child("gsr").ValueChanged += ReadGsrDatabase;
	}

	/// <summary>
	/// Action to be performed when a new hr value is read
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="args">Value changed</param>
	void ReadHrDatabase(object sender, ValueChangedEventArgs args)
	{
		if (args.DatabaseError != null)
		{
			//TODO ERROR
			return;
		}

		DataSnapshot snapshot = args.Snapshot;

		if (snapshot != null)
		{
			_hr = int.Parse(snapshot.Value.ToString());
			
			NewHr = true;
			
		}
		else
		{
			Debug.Log("ERROR");
		}
	}
	
	/// <summary>
	/// Action to be performed when a new gsr value is read
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="args">Value changed</param>
	void ReadGsrDatabase(object sender, ValueChangedEventArgs args)
	{
		if (args.DatabaseError != null)
		{
			//TODO ERROR
			return;
		}

		DataSnapshot snapshot = args.Snapshot;

		if (snapshot != null)
		{
			_gsr = float.Parse(snapshot.Value.ToString());
			
			NewGsr = true;
		}
		else
		{
			Debug.Log("ERROR");
		}
	}
	
	/// <summary>
	/// Stop reading from the database
	/// </summary>
	public void StopReading()
	{
		_reference.Child("hr").ValueChanged -= ReadHrDatabase;
		_reference.Child("gsr").ValueChanged -= ReadGsrDatabase;
	}

	/// <summary>
	/// Starts searching the base value for the heart rate
	/// </summary>
	public void SearchBaseValue()
	{
		StartCoroutine(HrBaseValueCoroutine());
	}

	/// <summary>
	/// Coroutine that search the base value
	/// </summary>
	/// <returns></returns>
	private IEnumerator HrBaseValueCoroutine()
	{
		Boolean found = false;
		float timer = 0f;
		int counter = 0;
		int range = 10;
		
		_baseHr = _hr;
		while (!found && timer < baseTimer)
		{
			if (NewHr)
			{
				NewHr = false;

				int current = _hr;

				if (current > _baseHr - range && current < _baseHr + range)
				{
					counter++;

					if (counter > 3)
					{
						counter = 0;
						range--;

						if (range < 6)
							found = true;
					}
				}
				else
				{
					counter = 0;

					_baseHr = (_baseHr + current) / 2;
				}
			}

			timer += Time.deltaTime;
			
			yield return null;
		}

		Debug.Log(_baseHr);

		_hrBaseRange = 7;
		_hrCurrentRange = 3;
		
		ActivityManager.Instance.ActivateNextActivity();
	}

	/// <summary>
	/// Starts tracking hr and gsr value
	/// </summary>
	public void StartTracking()
	{
		StartCoroutine(TrackHr());
		StartCoroutine(TrackGsr());
	}
	
	/// <summary>
	/// Coroutine that tracks the hr value
	/// </summary>
	/// <returns></returns>
	private IEnumerator TrackHr()
	{
		int previousHr = _hr;

		int counter = 0;
		
		NewHr = false;
		
		do
		{
			float timer = 0;
			
			while (!NewHr && timer < 2.5f)
			{
				timer += Time.deltaTime;
				yield return null;
			}

			if(_hr > 95)
				ActivityManager.Instance.ActivateNextActivity();
			
			int differenceBase = _hr - _baseHr;

			if (differenceBase < _hrBaseRange)
			{
				if (!_lastGsrStatus)
				{
					counter++;
				}
			}
			else
			{
				int differencePrevious = _hr - previousHr;

				if (Math.Abs(differencePrevious) > _hrCurrentRange)
				{
					if (Math.Sign(differencePrevious) == 1)
					{
						counter -= 2;
					}
					else
					{
						counter += 2;
					}
				}
				else if(_lastGsrStatus)
				{
					counter--;
				}
			}

			if (counter >= 10)
			{
				TheaterManager.Instance.ChangeLevel(true);
				counter = 2;
			}
			else if (counter <= -10)
			{
				TheaterManager.Instance.ChangeLevel(false);
				counter = -2;
			}

			previousHr = _hr;

			yield return null;
		} while (!_stop);
	}

	/// <summary>
	/// Coroutine that tracks the gsr value
	/// </summary>
	/// <returns></returns>
	private IEnumerator TrackGsr()
	{
		float previous = _gsr;
		do
		{
			float timer = 0;

			while (!NewGsr && timer < 2.5f)
			{
				timer += Time.deltaTime;
				yield return null;
			}

			float difference = _gsr - previous;

			if (difference <= 0)
			{
				_lastGsrStatus = false;
			}
			else
			{
				_lastGsrStatus = true;
			}

			yield return null;
		} while (!_stop);
	}

	/// <summary>
	/// Stop every reading from the database
	/// </summary>
	public void StopEverything()
	{
		StopReading();

		_stop = true;
	}

	/// <summary>
	/// Gets reference for the questions
	/// </summary>
	public void StartQuestion()
	{
		FirebaseDatabase.DefaultInstance.GetReference("/Questions/" + _reference.Key).ValueChanged += AddQuestion;
	}

	/// <summary>
	/// Reads questions from the database
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="args">Value changed</param>
	private void AddQuestion(object sender, ValueChangedEventArgs args)
	{
		if (args.DatabaseError != null)
        {
        	//TODO ERROR
        	return;
        }
		
		DataSnapshot snapshot = args.Snapshot;

		if (snapshot != null && snapshot.ChildrenCount > 0)
		{
			foreach (DataSnapshot child in snapshot.Children)
			{
				_question.Enqueue((string) child.Value);
			}
		}
		else
		{
			Console.WriteLine("ERROR");
		}
	}

	/// <summary>
	/// Gets the next question
	/// </summary>
	/// <returns>Next question</returns>
	public string GetQuestion()
	{
		return _question.Dequeue();
	}

	/// <summary>
	/// Returns whether there is a new question in the queue
	/// </summary>
	/// <returns>whether there is a new question in the queue</returns>
	public bool HasQuestion()
	{
		return _question.Count > 0;
	}
}
