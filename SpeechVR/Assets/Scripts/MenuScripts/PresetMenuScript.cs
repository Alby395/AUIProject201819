using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PresetMenuScript : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider kind;
    [SerializeField] private Slider indifferent;
    [SerializeField] private Slider serious;
    [SerializeField] private Slider audience;
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI kindPercentage;
    [SerializeField] private TextMeshProUGUI indifferentPercentage;
    [SerializeField] private TextMeshProUGUI seriousPercentage;
    [SerializeField] private TextMeshProUGUI audienceNumber;

    [SerializeField] private Canvas difficultyCanvas;
    [SerializeField] private VRWarningMenuScript vrWarning;
    
    private Canvas _canvas;
    
    private bool _working;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
    }

    /// <summary>
    /// Set the label for the audience value
    /// </summary>
    public void AudienceValue()
    {
        audienceNumber.text = audience.value.ToString();
    }
    
    /// <summary>
    /// Change the value of the sliders
    /// </summary>
    public void ChangeValue()
    {
        if (!_working)
        {
            _working = true;
            float kValue = kind.value;
            float iValue = indifferent.value;
            float sValue = serious.value;

            float sum = kValue + iValue + sValue;

            kValue = kValue / sum;
            iValue = iValue / sum;
            sValue = sValue / sum;
            
            kind.value = kValue;
            kindPercentage.text = kValue.ToString("P0");
            
            indifferent.value = iValue;
            indifferentPercentage.text = iValue.ToString("P0");
            
            serious.value = sValue;
            seriousPercentage.text = sValue.ToString("P0");
            
            _working = false;
        }

    }

    /// <summary>
    /// Set the preset value
    /// </summary>
    /// <param name="kValue">Kind value</param>
    /// <param name="iValue">Indifferent value</param>
    /// <param name="sValue">Serious value</param>
    public void SetPresetValue(float kValue, float iValue, float sValue)
    {
        kind.value = kValue;
        indifferent.value = iValue;
        serious.value = sValue;
    }

    /// <summary>
    /// Start the next screen
    /// </summary>
    public void StartTheater()
    {
        MainMenuManager.Instance.SetQuantity((int) audience.value, kind.value, indifferent.value, serious.value);
        _canvas.enabled = false;
        vrWarning.StartWarning();
    }

    /// <summary>
    /// Returns to the previous menu
    /// </summary>
    public void Back()
    {
        _canvas.enabled = false;
        difficultyCanvas.enabled = true;
    }
}
