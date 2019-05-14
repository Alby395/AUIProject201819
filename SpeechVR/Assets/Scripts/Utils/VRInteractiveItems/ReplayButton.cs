using UnityEngine;

public class ReplayButton: MonoBehaviour, VRInteractiveItem
{
    public void StartInteraction()
    {
        MicrophoneManager.Instance.Play();
    }
}
