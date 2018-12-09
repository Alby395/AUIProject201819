using UnityEngine;

public class Seat : MonoBehaviour
{
    [SerializeField] private GameObject _person;
    private bool _occupied;

    public bool IsOccupied()
    {
        return _occupied;
    }

    public void AssignSeat(GameObject person)
    {
        _person = person;
        _occupied = true;
    }

    public void FreeSeat()
    {
        _occupied = false;
        
        //TODO Return model in objectPool Location
    }
}
