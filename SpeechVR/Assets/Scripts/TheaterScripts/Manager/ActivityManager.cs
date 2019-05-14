using UnityEngine;

public class ActivityManager: MonoBehaviour
{
    public static ActivityManager Instance { get; private set; }
    private Activity _activity;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        _activity = GameObject.FindWithTag("StartingActivity").GetComponent<StartingActivity>();
        
        _activity.StartActivity();
    }
    
    /// <summary>
    /// Activates the next activity
    /// </summary>
    public void ActivateNextActivity()
    {
        Debug.Log("NEXT");

        _activity.Activate(false);
        _activity = _activity.NextActivity();

        _activity.Activate(true);
        
        Debug.Log("I'm here");
        _activity.StartActivity();
    }
}
