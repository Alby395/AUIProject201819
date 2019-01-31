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

	public void SetEasyPreset()
	{
		presetMenuScript.SetPresetValue(1f, 0f, 0f);
		_canvas.enabled = false;
		_presetCanvas.enabled = true;
	}

	public void SetMediumPreset()
	{
		presetMenuScript.SetPresetValue(0.7f, 0.2f, 0.1f);
		_canvas.enabled = false;
		_presetCanvas.enabled = true;
	}

	public void SetHardPreset()
	{
		presetMenuScript.SetPresetValue(0.5f, 0.3f, 0.2f);
		_canvas.enabled = false;
		_presetCanvas.enabled = true;
	}

	public void Back()
	{
		_canvas.enabled = false;
		modeCanvas.enabled = true;
	}
	
}
