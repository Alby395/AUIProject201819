using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
	public static MainMenuManager Instance { get; private set; }
	
	public enum SpeechMode {Solo, Interview};

	private SpeechMode _mode;
	
	private int _audience;
	private int _kind;
	private int _indifferent;
	private int _serious;

	private float _kindPercentage;
	private float _indifferentPercentage;
	private float _seriousPercentage;
	
	void Start ()
	{
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

	public void SetMode(SpeechMode mode)
	{
		_mode = mode;
	}
	
	public void SetQuantity(int audience, float kind, float indifferent, float serious)
	{
		_audience = audience;
		
		_kindPercentage = _kind;
		_kind = (int) (kind * _audience);

		_indifferentPercentage = indifferent;
		_indifferent = (int) (indifferent * _audience);

		_seriousPercentage = serious;
		_serious = (int) (serious * _audience);
	}

	public int GetAudience()
	{
		return _audience;
	}

	public int GetKind()
	{
		return _kind;
	}

	public int GetIndifferent()
	{
		return _indifferent;
	}

	public int GetSerious()
	{
		return _serious;
	}
	
	public float GetFirstIntervalThreshold()
	{
		return _kindPercentage;
	}

	public float GetSecondIntervalThreshold()
	{
		return _kindPercentage + _indifferentPercentage;
	}
}
