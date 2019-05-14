using System.Linq;
using InfinityEngine.Localization;
using UnityEngine;

public class SpeechActivity: MonoBehaviour, Activity
{
    [SerializeField] private AudioSource _source;

    [SerializeField] private GameObject soloCanvas;
    [SerializeField] private GameObject interviewCanvas;
    [SerializeField] private GameObject questionCanvas;

    [SerializeField] private FinalActivity nextActivity;
    private delegate void QuestionReader();

    private QuestionReader _reader;
    
    public void StartActivity()
    {
        Debug.Log("STARTING SPEECH");

        if (MainMenuManager.Instance.GetMode() == MainMenuManager.SpeechMode.Interview)
        {
            Debug.Log("SOLO ENABLED");
            questionCanvas.SetActive(true);
            
            FirebaseManager.Instance.StartQuestion();
        }
        else
        {
            Debug.Log("SOLO ENABLED");
            soloCanvas.SetActive(true);
        }
        
        TheaterManager.Instance.ChangeCurtainsState(true, 0);
        
        FirebaseManager.Instance.StartTracking();
        MicrophoneManager.Instance.StartTracking();
        
    }

    public Activity NextActivity()
    {
        TheaterManager.Instance.ChangeCurtainsState(false, 2f);
        TheaterManager.Instance.StartFinalClap();
        return nextActivity;
    }

    public void Activate(bool status)
    {
        gameObject.SetActive(status);
    }

    /// <summary>
    /// Asks the next question if it's available
    /// </summary>
    public void StartNextQuestion()
    {
        if (FirebaseManager.Instance.HasQuestion())
        {
            _reader();
        }
        else
        {
            SpeechEngine.Speak("There's no question available.");
        }
    }

    /// <summary>
    /// Reads the question
    /// </summary>
    private void VoiceReader()
    {
        SpeechEngine.Speak(FirebaseManager.Instance.GetQuestion());
    }

    /// <summary>
    /// Picks the person that should say the question.
    /// </summary>
    private void AudienceReader()
    {
        TheaterManager.Instance.QuestionAudience();
    }

    /// <summary>
    /// Assign the right mode for the interview
    /// </summary>
    /// <param name="op">Whether the question should be asked by the audience or not.</param>
    public void AssignReader(bool op)
    {
        SpeechEngine.SetLanguage(Locale.English);
        
        if (op)
        {
            _reader += VoiceReader;

            SpeechEngine.SetVoice(SpeechEngine.AvaillableVoices.First(voice => voice.Name.Equals("en-gb-x-rjs-network")));
        }
        else
        {
            _reader += AudienceReader;
            TheaterManager.Instance.ActivateColliders();
        }

        questionCanvas.SetActive(false);
        interviewCanvas.SetActive(true);
    }
}
