using System.Collections;

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
	
	private int _currentLevel;
	private Coroutine _coroutine;

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
		_spotlightRight.spotAngle = _spotlightLeft.spotAngle = _lightRadiusLevels[0];
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			if (_currentLevel < (_lightRadiusLevels.Length) - 1)
			{
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
		
		for (float t = 0; t < 1.0f; t += Time.deltaTime / _lightTime)
		{ 
			_spotlightRight.spotAngle = _spotlightLeft.spotAngle = Mathf.Lerp(currentSpotAngle, _lightRadiusLevels[_currentLevel], t);
			_spotlightLeft.intensity = _spotlightRight.intensity = Mathf.Lerp(currentIntensity, _lightIntensityLevels[_currentLevel], t);
			
			yield return null;
		}
	}
}
