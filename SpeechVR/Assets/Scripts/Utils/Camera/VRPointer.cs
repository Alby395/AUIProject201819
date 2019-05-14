using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRPointer: MonoBehaviour
{
    [SerializeField] private Image reticle;
    
    [SerializeField] private float time;
    [SerializeField] private Transform cameraTransform;
    
    private Coroutine _coroutine;
    private bool _active;
    private bool _complete;

    private VRInteractiveItem _item;
   

    /// <summary>
    /// Stop the countdown
    /// </summary>
    public void StopCount()
    {
        StopCoroutine(_coroutine);
        reticle.enabled = false;
        
        _active = false;
    }

    /// <summary>
    /// Starts the countdown before activating the interaction
    /// </summary>
    /// <param name="hit">Item to interact with</param>
    public void StartCount(RaycastHit hit)
    {
        if (!_active)
        {
            _item = hit.collider.GetComponent<VRInteractiveItem>();
            _coroutine = StartCoroutine(FillReticle());
        }
    }

    /// <summary>
    /// Coroutine that fills the reticle
    /// </summary>
    /// <returns></returns>
    private IEnumerator FillReticle()
    {
        _active = true;
        reticle.fillAmount = 0f;
        reticle.enabled = true;
        
        float timer = 0f;
            
        while (timer < time)
        {
            reticle.fillAmount = timer / time;

            timer += Time.deltaTime;
            yield return null;
        }

        reticle.fillAmount = 1f;
        _active = false;
        reticle.enabled = false;
              
        _item.StartInteraction();
    }

    /// <summary>
    /// Returns whether the Pointer is counting or not
    /// </summary>
    /// <returns>whether the Pointer is counting or not</returns>
    public bool IsCounting()
    {
        return _active;
    }
}
