using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class VRWarningMenuScript: MonoBehaviour
{
    [SerializeField] private RectTransform square;
    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
    }
    
    /// <summary>
    /// Enables the canvas and start loading
    /// </summary>
    public void StartWarning()
    {
        _canvas.enabled = true;

        StartCoroutine(LoadNextScene());
    }

    /// <summary>
    /// Coroutine that loads the VR activity
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadNextScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("TheaterScene", LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        } 
        
        XRSettings.LoadDeviceByName("cardboard");

        yield return new WaitForSeconds(5f);

        XRSettings.enabled = true;

        operation.allowSceneActivation = true;
    }
}
