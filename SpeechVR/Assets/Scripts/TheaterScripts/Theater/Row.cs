using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    [SerializeField] private List<Seat> _seats;
    
    private int _index;
    
    private delegate void ChangeAmount();

    /// <summary>
    /// Gives the number of seats in the row.
    /// </summary>
    /// <returns>The number of seats in the row.</returns>
    public int SeatPerRow()
    {
        return _seats.Count;
    }

    /// <summary>
    /// Frees all the seats in the row
    /// </summary>
    public IEnumerator FreeRow()
    {
        foreach (Seat seat in _seats)
        {
            if(seat.IsOccupied())
                seat.FreeSeat();
            
            yield return null;
        }
    }

    /// <summary>
    /// Coroutine that activates all the people in a row
    /// </summary>
    /// <returns></returns>
    public IEnumerator ActivatePeople()
    {
        for (int i = 0; i < _seats.Count; i++)
        {
            _seats[i].ActivatePerson();
            yield return null;
        }
    }
    
    /// <summary>
    /// Coroutine that assign people to the seats
    /// </summary>
    /// <param name="people"></param>
    /// <returns></returns>
    public IEnumerator AssignAll(List<Person> people)
    {
        for (_index = 0; _index < _seats.Count; _index++)
        {
            _seats[_index].AssignSeat(people[_index]);
            
            yield return null;
        }
    }

    /// <summary>
    /// Coroutine that assign the seats randomly
    /// </summary>
    /// <param name="people">Number of people to assign</param>
    /// <returns></returns>
    public IEnumerator AssignRandom(List<Person> people)
    {
        int peopleCount = people.Count;
        
        int firstSeat, lastSeat;

        ChangeAmount op;

        int seatsCount = _seats.Count;
        
        if (Random.value < 0.5f)
        {
            firstSeat = 0;
            lastSeat = seatsCount - 1;
            op = Increment;
        }
        else
        {
            firstSeat = seatsCount - 1;
            lastSeat = 0;
            op = Decrement;
        }
        
        foreach (Person person in people)
        {
            bool assigned = false;
            _index = firstSeat;
            float chance = (float) peopleCount / seatsCount;
            
            do
            {
                if (!_seats[_index].IsOccupied() && Random.value < chance)
                {
                    assigned = true;
                }
                else
                {
                    op();
                }
            } while (!assigned && _index != lastSeat);

            if (!assigned)
            {
                lastSeat--;
            }

            peopleCount--;
            seatsCount--;
            
            _seats[_index].AssignSeat(person);

            yield return null;
        }
    }

    public Person GetPerson(int index)
    {
        return _seats[index].GetPerson();
    }
    
    /// <summary>
    /// Increments the index by 1.
    /// </summary>
    private void Increment()
    {
        _index++;
    }
    
    /// <summary>
    /// Decrements the index by 1.
    /// </summary>
    private void Decrement()
    {
        _index--;
    }
}
