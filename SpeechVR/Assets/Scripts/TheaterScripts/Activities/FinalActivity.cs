using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using UnityEngine.XR;

public class FinalActivity: MonoBehaviour, Activity
{
    [SerializeField] private Canvas canvas;
    
    public void StartActivity()
    {
        MicrophoneManager.Instance.Stop();
        FirebaseManager.Instance.StopEverything();
        
        gameObject.SetActive(true);
        canvas.enabled = true;

        ObjectPoolManager.Instance.DestroyEverything();
    }

    public Activity NextActivity()
    {
        return null;
    }

    public void Activate(bool status)
    {
        gameObject.SetActive(status);
    }

    /// <summary>
    /// Close the VR activity
    /// </summary>
    public void Exit()
    {
        StartCoroutine(ExitVR());
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Coroutine that stops the VR activity.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExitVR()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        } 
        
        XRSettings.LoadDeviceByName("");

        yield return new WaitForSeconds(5f);

        XRSettings.enabled = false;

        operation.allowSceneActivation = true;
    }
    
}
