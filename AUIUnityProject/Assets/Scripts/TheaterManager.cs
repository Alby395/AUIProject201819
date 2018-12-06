using System.Collections;

using UnityEngine;


public class TheaterManager : MonoBehaviour 
{
	[Header("Light related variables")]
    public Light Spotlight;
	public float LightTime;
	public float[] LightLevels;
	
	private int _currentLevel;
	private Coroutine _coroutine;
	
	// Use this for initialization
	void Start ()
	{
		_currentLevel = 0;
		Spotlight.spotAngle = LightLevels[0];
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			if (_currentLevel < (LightLevels.Length) - 1)
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
		Debug.Log("I'm here");
		float currentSpotAngle = Spotlight.spotAngle;
		
		for (float t = 0; t < 1.0f; t += Time.deltaTime / LightTime)
		{
			Spotlight.spotAngle = Mathf.Lerp(currentSpotAngle, LightLevels[_currentLevel], t);

			yield return null;
		}
	}
}
