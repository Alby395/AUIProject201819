using System.Collections;
using System.Linq;
using InfinityEngine.Localization;
using UnityEngine;

public class Person: MonoBehaviour
{
    private enum Gender {Male, Female}
    public enum Category {Kind, Serious, Indifferent}

    [SerializeField] private Category category;
    [SerializeField] private Gender gender;
    
    private Animator _animator;
    private static readonly int Reaction = Animator.StringToHash("Reaction");
    private static readonly int Question = Animator.StringToHash("Question");
    private Voice _voice;
    private static readonly int End = Animator.StringToHash("End");

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Animates the person
    /// </summary>
    public void Animate()
    {
        StartCoroutine(StartAnimation());
        Debug.Log("Animating " + this);
    }

    /// <summary>
    /// Coroutine that manages the animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartAnimation()
    {
        _animator.SetFloat(Reaction, Random.value);

        yield return null;
        
        _animator.SetFloat(Reaction, -1);
    }
    
    /// <summary>
    /// Makes the person read the question.
    /// </summary>
    public void AskQuestion()
    {
        SpeechEngine.SetVoice(_voice);
        SpeechEngine.Speak(FirebaseManager.Instance.GetQuestion());
    }

    /// <summary>
    /// Gives the category of the person
    /// </summary>
    /// <returns>Category of the person</returns>
    public Category GetCategory()
    {
        return category;
    }

    /// <summary>
    /// Changes the position of the person
    /// </summary>
    /// <param name="trigger">Action that should be performed</param>
    public void ChangeStandPosition(string trigger)
    {
        _animator.SetTrigger(trigger);
    }

    /// <summary>
    /// Set the right voice for the Speech Engine
    /// </summary>
    public void SetVoice()
    {
        string toSearch;
        
        if (gender.Equals(Gender.Male))
        {
            toSearch = "en-gb-x-rjs-network";
        }
        else
        {
            toSearch = "en-GB-language";
        }

        _voice = SpeechEngine.AvaillableVoices.First(voice => voice.Name.Equals(toSearch));
    }

    /// <summary>
    /// Start the final clap.
    /// </summary>
    public void Clap()
    {
        _animator.SetTrigger(End);
    }
}
