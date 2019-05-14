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

	/// <summary>
	/// Sets the mode of the activity
	/// </summary>
	/// <param name="mode">The chosen mode</param>
	public void SetMode(SpeechMode mode)
	{
		_mode = mode;
	}
	
	/// <summary>
	/// Sets the amount of people for each category
	/// </summary>
	/// <param name="audience">Total number of people in the audience</param>
	/// <param name="kind">Kind people in the audience</param>
	/// <param name="indifferent">Indifferent people in the audience</param>
	/// <param name="serious">Serious people in the audience</param>
	public void SetQuantity(int audience, float kind, float indifferent, float serious)
	{
		_audience = audience;
		
		_kind = (int) (kind * _audience);
		_indifferent = (int) (indifferent * _audience);
		_serious = (int) (serious * _audience);
	}

	/// <summary>
	/// Gives the total number of people in the audience
	/// </summary>
	/// <returns>Number of people in the audience</returns>
	public int GetAudience()
	{
		return _audience;
	}

	/// <summary>
	/// Gives the total number of kind people in the audience
	/// </summary>
	/// <returns>Number of kind people in the audience</returns>
	public int GetKind()
	{
		return _kind;
	}

	/// <summary>
	/// Gives the total number of indifferent people in the audience
	/// </summary>
	/// <returns>Number of indifferent people in the audience</returns>
	public int GetIndifferent()
	{
		return _indifferent;
	}

	/// <summary>
	/// Gives the total number of serious people in the audience
	/// </summary>
	/// <returns>Number of serious people in the audience</returns>
	public int GetSerious()
	{
		return _serious;
	}

	/// <summary>
	/// Gives the chosen mode.
	/// </summary>
	/// <returns>The chosen mode.</returns>
	public SpeechMode GetMode()
	{
		return _mode;
	}
}
