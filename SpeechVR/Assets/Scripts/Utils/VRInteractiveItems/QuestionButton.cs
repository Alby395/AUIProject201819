using UnityEngine;

public class QuestionButton: MonoBehaviour, VRInteractiveItem
{
    [SerializeField] private SpeechActivity activity;
    
    public void StartInteraction()
    {
        activity.StartNextQuestion();    
    }
}
