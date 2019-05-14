using UnityEngine;

public class Seat : MonoBehaviour
{
    private Person _person;
    
    private bool _occupied;
    
    private readonly Vector3 _offset = new Vector3(1f, 0.5f, 0f);

    /// <summary>
    /// Checks whether the seat is occupied
    /// </summary>
    /// <returns>True if it's occupied, false otherwise.</returns>
    public bool IsOccupied()
    {
        return _occupied;
    }

    /// <summary>
    /// Assigns the seat to the given person
    /// </summary>
    /// <param name="person">Person that is going to occupy the seat</param>
    public void AssignSeat(Person person)
    {
        _person = person;
        _occupied = true;
    }

    /// <summary>
    /// Frees the seat and return the person to the ObjectPoolManager
    /// </summary>
    public void FreeSeat()
    {       
        if(_occupied)
            ObjectPoolManager.Instance.ReturnToPool(_person);
    }

    /// <summary>
    /// Activates the person assigned
    /// </summary>
    public void ActivatePerson()
    {
        if (_occupied)
        {
            _person.gameObject.SetActive(true);
            _person.transform.position = transform.position + _offset;
        }
    }

    /// <summary>
    /// Gives the person assigned to the seat.
    /// </summary>
    /// <returns>The person assigned.</returns>
    public Person GetPerson()
    {
        return _person;
    }
}
