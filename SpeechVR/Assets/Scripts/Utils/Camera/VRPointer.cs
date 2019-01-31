using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRPointer: MonoBehaviour
{
    [SerializeField] private Image reticle;
    [SerializeField] private Image reticleBackground;
    [SerializeField] private float distance = 200;
    [SerializeField] private float time;
    [SerializeField] private Transform cameraTransform;
    
    private Coroutine _coroutine;
    private bool _active;
    private bool _complete;

    private Vector3 _scale;
    private VRInteractiveItem _item;
    
    private void Start()
    {
        transform.position = cameraTransform.position + cameraTransform.forward * distance;
        _scale = transform.localScale;
    }

    public void StopCount()
    {
        StopCoroutine(_coroutine);
        reticle.enabled = false;
        
        transform.position = cameraTransform.position + cameraTransform.forward * distance;
        transform.localScale = _scale;
        
        _active = false;
    }

    public void StartCount(RaycastHit hit)
    {
        if (!_active)
        {
            transform.position = hit.point;
            transform.localScale = _scale * hit.distance/distance;
            
            _item = hit.collider.GetComponent<VRInteractiveItem>();
            _coroutine = StartCoroutine(FillReticle());
        }
    }

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

    public bool IsCounting()
    {
        return _active;
    }
}
