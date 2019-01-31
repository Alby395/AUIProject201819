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

    public void AudienceValue()
    {
        audienceNumber.text = audience.value.ToString();
    }
    
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

    public void SetPresetValue(float kValue, float iValue, float sValue)
    {
        kind.value = kValue;
        indifferent.value = iValue;
        serious.value = sValue;
    }

    public void StartTheater()
    {
        MainMenuManager.Instance.SetQuantity((int) audience.value, kind.value, indifferent.value, serious.value);
        _canvas.enabled = false;
        vrWarning.StartWarning();
    }

    public void Back()
    {
        _canvas.enabled = false;
        difficultyCanvas.enabled = true;
    }
}
