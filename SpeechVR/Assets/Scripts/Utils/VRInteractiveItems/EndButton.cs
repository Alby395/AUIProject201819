using UnityEngine;

public class EndButton: MonoBehaviour, VRInteractiveItem
{
    public void StartInteraction()
    {
        ActivityManager.Instance.ActivateNextActivity();
    }
}
