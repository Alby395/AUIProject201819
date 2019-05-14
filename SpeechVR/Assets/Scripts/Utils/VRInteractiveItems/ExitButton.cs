using UnityEngine;

public class ExitButton: MonoBehaviour, VRInteractiveItem
{
    [SerializeField] private FinalActivity activity;
    public void StartInteraction()
    {
        activity.Exit();
    }
}
