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

    public void AssignSeats(List<GameObject> people)
    {
        if (people.Count < _seats.Count)
            AssignRandom(people);
        else
            AssignAll(people);
    }

    public void FreeRow()
    {
        foreach (Seat seat in _seats)
        {
            seat.FreeSeat();      
        }
    }
    private void AssignAll(List<GameObject> people)
    {
        for (int i = 0; i < _seats.Count; i++)
        {
            _seats[i].AssignSeat(people[i]);
        }
    }

    private void AssignRandom(List<GameObject> people)
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
        }
    }

    private int Increment(int n)
    {
        return (n + 1);
    }
    
    private int Decrement(int n)
    {
        return (n - 1);
    }
}
