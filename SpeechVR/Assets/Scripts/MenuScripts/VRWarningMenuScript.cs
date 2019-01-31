using UnityEngine;

public class VRWarningMenuScript: MonoBehaviour
{
    [SerializeField] private RectTransform square;
    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
    }
    
    public void StartWarning()
    {
        _canvas.enabled = true;

        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
}
