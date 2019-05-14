using InfinityEngine.Localization;
using UnityEngine;

public class AudienceQuestion: MonoBehaviour, VRInteractiveItem
{
    private Person _person;
    
    /// <summary>
    /// Assign the person to the collider
    /// </summary>
    /// <param name="person">Person to assign</param>
    public void AssignPerson(Person person)
    {
        _person = person;
        _person.SetVoice();
        
        gameObject.SetActive(true);
    }
    
    public void StartInteraction()
    {   
        TheaterManager.Instance.SitOther(_person);
        
        _person.AskQuestion();
        
        gameObject.SetActive(false);
    }
}
