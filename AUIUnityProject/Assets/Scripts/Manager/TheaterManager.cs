using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TheaterManager : MonoBehaviour 
{
	public static TheaterManager Instance { get; private set; }
	
	[Header("Light related variables")]
    [SerializeField] private Light _spotlightRight;
	[SerializeField] private Light _spotlightLeft;
	[SerializeField] private float _lightTime;
	[SerializeField] private float[] _lightRadiusLevels;
	[SerializeField] private float[] _lightIntensityLevels;
	
	[SerializeField] private List<Row> _rows;

	[SerializeField] private int _initializedRows = 2;
	
	private int _seatsPerRow;
	private int _currentRow;
	private int _currentLevel;
	private Coroutine _coroutine;
	private bool _add;
	
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this);
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		_currentLevel = 0;
		_seatsPerRow = _rows[0].SeatPerRow();
		_spotlightRight.spotAngle = _spotlightLeft.spotAngle = _lightRadiusLevels[0];

		StartCoroutine(InitFirstRows());
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			if (_currentLevel < (_lightRadiusLevels.Length) - 1)
			{
				_add = true;
				_currentLevel++;
				if(_coroutine != null)
					StopCoroutine(_coroutine);
				
				_coroutine = StartCoroutine(ChangeLevel());
			}	
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			if (_currentLevel > 0)
			{
				_add = false;
				_currentLevel--;
				
				if(_coroutine != null)
					StopCoroutine(_coroutine);
				
				_coroutine = StartCoroutine(ChangeLevel());
			}
		}
	}
	
	private IEnumerator ChangeLevel()
	{
		float currentSpotAngle = _spotlightLeft.spotAngle;
		float currentIntensity = _spotlightLeft.intensity;

		if (_add && _currentRow < _rows.Count - 1 && ObjectPoolManager.Instance.RemainingElement() > 0)
		{
			Debug.Log("Assigning Row: " + _currentRow);
			
			float rand = ((float) (15 - _currentLevel) / 15) * _seatsPerRow;
			int n = Random.Range( (int) rand, _seatsPerRow) ;
			
			Debug.Log("People to assign: " + n);
			List<GameObject> people = ObjectPoolManager.Instance.GivePeople(n);

			if (people.Count < _seatsPerRow)
				yield return StartCoroutine(_rows[_currentRow++].AssignRandom(people));
			else
				yield return StartCoroutine(_rows[_currentRow++].AssignAll(people));
		}
		else if(!_add && _currentRow > _initializedRows)
		{
			Debug.Log("Clearing row: " + _currentRow);
			StartCoroutine(_rows[--_currentRow].FreeRow());
		}

		for (float t = 0; t < 1.0f; t += Time.deltaTime / _lightTime)
		{ 
			_spotlightRight.spotAngle = _spotlightLeft.spotAngle = Mathf.Lerp(currentSpotAngle, _lightRadiusLevels[_currentLevel], t);
			_spotlightLeft.intensity = _spotlightRight.intensity = Mathf.Lerp(currentIntensity, _lightIntensityLevels[_currentLevel], t);
			
			yield return null;
		}
	}

	private IEnumerator InitFirstRows()
	{
		while (!ObjectPoolManager.Instance.Ready)
			yield return null;

		for (_currentRow = 0; _currentRow < _initializedRows; _currentRow++)
		{
			yield return StartCoroutine(_rows[_currentRow].AssignAll(ObjectPoolManager.Instance.GivePeople(_seatsPerRow)));
		}
	}
}
