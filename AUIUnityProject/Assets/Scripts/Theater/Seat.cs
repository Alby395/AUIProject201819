using UnityEngine;

public class Seat : MonoBehaviour
{
    [SerializeField] private GameObject _person;
    
    private bool _occupied;
    
    private readonly Vector3 _offset = new Vector3(0f, 5f, 0f);
    
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
    public void AssignSeat(GameObject person)
    {
        _person = person;
        _person.transform.position = transform.position + _offset;
        _occupied = true;
    }

    /// <summary>
    /// Frees the seat and return the person to the ObjectPoolManager
    /// </summary>
    public void FreeSeat()
    {
        _occupied = false;
        
        ObjectPoolManager.Instance.ReturnToPool(_person);
    }
}
