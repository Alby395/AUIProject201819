using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyMenuScript : MonoBehaviour
{
	[SerializeField] private PresetMenuScript presetMenuScript;

	private Canvas _presetCanvas;
	private Canvas _canvas;
	[SerializeField] private Canvas modeCanvas;
	
	private void Start()
	{
		_canvas = GetComponent<Canvas>();
		_presetCanvas = presetMenuScript.gameObject.GetComponent<Canvas>();
	}

	/// <summary>
	/// Sets the easy preset
	/// </summary>
	public void SetEasyPreset()
	{
		presetMenuScript.SetPresetValue(1f, 0f, 0f);
		_canvas.enabled = false;
		_presetCanvas.enabled = true;
	}
	
	/// <summary>
	/// Sets the medium preset
	/// </summary>
	public void SetMediumPreset()
	{
		presetMenuScript.SetPresetValue(0.7f, 0.2f, 0.1f);
		_canvas.enabled = false;
		_presetCanvas.enabled = true;
	}

	/// <summary>
	/// Sets the hard preset
	/// </summary>
	public void SetHardPreset()
	{
		presetMenuScript.SetPresetValue(0.5f, 0.3f, 0.2f);
		_canvas.enabled = false;
		_presetCanvas.enabled = true;
	}

	/// <summary>
	/// Returns to the previous menu.
	/// </summary>
	public void Back()
	{
		Debug.Log("HERE");
		_canvas.enabled = false;
		modeCanvas.enabled = true;
	}
	
}
