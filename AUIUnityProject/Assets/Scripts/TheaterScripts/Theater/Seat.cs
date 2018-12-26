using UnityEngine;

public class Seat : MonoBehaviour
{
    private GameObject _person;
    
    private bool _occupied;
    private Animator _animator;
    
    private readonly Vector3 _offset = new Vector3(1.2f, 0.3f, 0f);


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && _occupied && _animator != null && Random.value < 0.3f)
        {
            _animator.SetTrigger("Reaction1");
        }

        if (Input.GetKeyDown(KeyCode.X) && _occupied && _animator != null && Random.value < 0.3f)
        {
            _animator.SetTrigger("Reaction2");
        }

    }

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
        _person.SetActive(true);

        _animator = _person.GetComponent<Animator>();
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
