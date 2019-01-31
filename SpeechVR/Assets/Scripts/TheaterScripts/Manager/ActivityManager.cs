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
        _activity = new StartingActivity();
    }
}
