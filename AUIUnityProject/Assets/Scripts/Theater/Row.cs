using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    [SerializeField] private List<Seat> _seats;

    private delegate int ChangeAmount(int n);
    
    public bool IsActive()
    {
        foreach (Seat seat in _seats)
        {
            if (seat.IsOccupied())
                return true;
        }

        return false;
    }

    /// <summary>
    /// Gives the number of seats in the row.
    /// </summary>
    /// <returns>The number of seats in the row.</returns>
    public int SeatPerRow()
    {
        return _seats.Count;
    }
    
    public IEnumerator AssignSeats(int n)
    {
        List<GameObject> people = ObjectPoolManager.Instance.GivePeople(n);
        
        if (people.Count < _seats.Count)
            return AssignRandom(people);
        else
            return AssignAll(people);
    }

    /// <summary>
    /// Frees all the seats in the row
    /// </summary>
    public void FreeRow()
    {
        foreach (Seat seat in _seats)
        {
            seat.FreeSeat();      
        }
    }
    
    private IEnumerator AssignAll(List<GameObject> people)
    {
        for (int i = 0; i < _seats.Count; i++)
        {
            _seats[i].AssignSeat(people[i]);
            
            yield return null;
        }
    }

    private IEnumerator AssignRandom(List<GameObject> people)
    {
        int peopleCount = people.Count;
        
        int firstSeat, lastSeat;

        ChangeAmount op;

        int seatsCount = _seats.Count;
        
        if (Random.value < 0.5f)
        {
            firstSeat = 0;
            lastSeat = _seats.Count - 1;
            op = Increment;
        }
        else
        {
            firstSeat = _seats.Count - 1;
            lastSeat = 0;
            op = Decrement;
        }
         
        foreach (GameObject person in people)
        {
            bool assigned = false;
            int i = firstSeat;
            float chance = (float) peopleCount / seatsCount;
            
            do
            {
                if (!_seats[i].IsOccupied() && Random.value < chance)
                {
                    assigned = true;
                }
                else
                {
                    op(i);
                }
            } while (!assigned && i != lastSeat);

            if (!assigned)
            {
                lastSeat--;
            }

            peopleCount--;
            seatsCount--;
            
            _seats[i].AssignSeat(person);

            yield return null;
        }
    }

    /// <summary>
    /// Increments a given integer by 1.
    /// </summary>
    /// <param name="n">Number to increase.</param>
    /// <returns>The number increased.</returns>
    private int Increment(int n)
    {
        return (n + 1);
    }
    
    /// <summary>
    /// Decrements a given integer by 1.
    /// </summary>
    /// <param name="n">Number to decrease.</param>
    /// <returns>Te number decreased.</returns>
    private int Decrement(int n)
    {
        return (n - 1);
    }
}
