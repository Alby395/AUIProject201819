using UnityEngine;

public class InterviewButton: MonoBehaviour, VRInteractiveItem
{
    [SerializeField] private bool op;
    [SerializeField] private SpeechActivity activity;
    
    public void StartInteraction()
    {
        activity.AssignReader(op);
    }
}
