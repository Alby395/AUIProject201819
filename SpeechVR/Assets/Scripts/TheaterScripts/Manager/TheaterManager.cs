using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Random = UnityEngine.Random;


public class TheaterManager : MonoBehaviour
{
	public static TheaterManager Instance { get; private set; }

	[Header("Curtains")]
	[SerializeField] private Transform curtains;
	[SerializeField] private float curtainsTime;
	[SerializeField] private float topPostition;
	private float _bottomPosition;
	
	[Header("Theatre stuff")]
	[SerializeField] private List<Light> spotlights;
	[SerializeField] private List<Row> rows;
	[SerializeField] private int initializedRows = 2;

	[SerializeField] private List<AudienceQuestion> colliders;
	
	private int _seatsPerRow;
	private int _currentLevel;
	private int _maxLevel;

	private List<Person> _questionAudience;
	
	private Coroutine _coroutine;
	private bool _add;
	
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this);
		}
	}

	// Use this for initialization
	void Start()
	{
		_currentLevel = 0;
		_bottomPosition = curtains.position.y;
		
		_seatsPerRow = rows[0].SeatPerRow();
		StartCoroutine(InitTheater());
	}
	
	/// <summary>
	/// Coroutine that initializes the row of the theatre
	/// </summary>
	/// <returns></returns>
	private IEnumerator InitTheater()
	{
		int row = 0;

		_maxLevel = -initializedRows;
		
		while (!ObjectPoolManager.Instance.Ready)
			yield return null;

		while (ObjectPoolManager.Instance.RemainingElement() && row < 3)
		{
			List<Person> people = ObjectPoolManager.Instance.GivePeople(_seatsPerRow);

			if (people.Count < _seatsPerRow)
				yield return StartCoroutine(rows[row++].AssignRandom(people));
			else
				yield return StartCoroutine(rows[row++].AssignAll(people));

			_maxLevel++;
			
		}

		int decrease = 1;

		while (ObjectPoolManager.Instance.RemainingElement())
		{
			List<Person> people = ObjectPoolManager.Instance.GivePeople(_seatsPerRow - decrease);
			
			yield return StartCoroutine(rows[row++].AssignRandom(people));

			decrease++;
			_maxLevel++;
		}
		
		for (row = 0; row < initializedRows; row++)
		{
			Debug.Log("Activating row " + row);
			yield return StartCoroutine(rows[row].ActivatePeople());
		}
	}
	
	/// <summary>
	/// Starts the right coroutine that changes the amount of people in the audience
	/// </summary>
	/// <param name="action"></param>
	public void ChangeLevel(bool action)
	{	
		if (action && _currentLevel < _maxLevel)
		{

			StartCoroutine(IncreaseLevel());
			return;
		}
		
		if (!action && _currentLevel > 0)
		{
			StartCoroutine(DecreaseLevel());
		}
	}
	
	/// <summary>
	/// Increases the number of people in the audience
	/// </summary>
	/// <returns></returns>
	private IEnumerator IncreaseLevel()
	{
		spotlights[_currentLevel].enabled = false;
		spotlights[++_currentLevel].enabled = true;

		yield return StartCoroutine(rows[_currentLevel + initializedRows - 1].ActivatePeople());
	}
	
	/// <summary>
	/// Decreases the number of people in the audience
	/// </summary>
	/// <returns></returns>
	private IEnumerator DecreaseLevel()
	{
		spotlights[_currentLevel--].enabled = false;
		
		spotlights[_currentLevel].enabled = true;
		
		yield return StartCoroutine(rows[_currentLevel + initializedRows].FreeRow());		
	}

	/// <summary>
	/// Changes the position of the curtains.
	/// </summary>
	/// <param name="status">Whether the curtains need to be put up or down</param>
	/// <param name="delay">Slows the change of the state.</param>
	public void ChangeCurtainsState(bool status, float delay)
	{
		StartCoroutine(MoveCurtains(status, delay));
	}

	/// <summary>
	/// Coroutine that changes the state of the curtains
	/// </summary>
	/// <param name="status">Whether the curtains need to be put up or down</param>
	/// <param name="delay">Slows the change of the state.</param>
	/// <returns></returns>
	private IEnumerator MoveCurtains(bool status, float delay)
	{
		float timer = 0;
		Vector3 startingPosition = curtains.position;
		Vector3 finalPosition = startingPosition;
		
		finalPosition.y = status ? topPostition : _bottomPosition;

		while (timer < 1f)
		{
			timer += Time.deltaTime / (curtainsTime + delay);

			curtains.position = Vector3.Lerp(startingPosition, finalPosition, timer);
			yield return null;
		}
	}

	/// <summary>
	/// Picks the member of the audience to animate
	/// </summary>
	public void PickAudienceAnimate()
	{
		StartCoroutine(PickPeople(true));
	}

	/// <summary>
	/// Picks the member of the audience that are part of the indifferent category to animate.
	/// </summary>
	public void PickIndifferentAnimate()
	{
		StartCoroutine(PickPeople(false));
	}

	/// <summary>
	/// Coroutine that picks the people to animate from the pool.
	/// </summary>
	/// <param name="op">Whether the people of the animate are everyone or just the indifferent one</param>
	/// <returns></returns>
	private IEnumerator PickPeople(bool op)
	{
		Debug.Log("Picking");
		int amount = 0;

		if (op)
		{
			amount = 6 + 2 * _currentLevel;
		}
		else
		{
			int indifferent = MainMenuManager.Instance.GetIndifferent();
			
			if (indifferent > 0)
			{
				amount = 2 + _currentLevel;

				amount = (amount > indifferent) ? indifferent : amount;
			}	
		}

		int max = MainMenuManager.Instance.GetAudience();
		int step = Random.Range(0, MainMenuManager.Instance.GetAudience());
		int i = step;
		int count = 0;

		List<Person> persons = ObjectPoolManager.Instance.GetPool();
		
		while (count < amount && i < max)
		{
			if (op || persons[i].GetCategory() == Person.Category.Indifferent)
			{
				if (persons[i].isActiveAndEnabled)
				{
					persons[i].Animate();
					count++;
				}
			}

			i++;
			yield return null;
		}

		i = 0;
		
		while(count < amount && i < step)
		{
			if (op || persons[i].GetCategory() == Person.Category.Indifferent)
			{
				if (persons[i].isActiveAndEnabled)
				{
					persons[i].Animate();
					count++;
				}
			}

			i++;
			yield return null;
		}
	}

	/// <summary>
	/// Starts the coroutine to question the audience
	/// </summary>
	public void QuestionAudience()
	{
		StartCoroutine(QuestionAudienceCoroutine());

	}

	/// <summary>
	/// Coroutine that pick the people that have a question
	/// </summary>
	/// <returns></returns>
	private IEnumerator QuestionAudienceCoroutine()
	{
		if (_questionAudience.Count > 0)
		{
			_questionAudience[0].ChangeStandPosition("Sit");
			_questionAudience.Clear();
			
			yield return null;
		}

		while(_questionAudience.Count < 3)
		{
			int row = Random.Range(0, initializedRows);
			int seat = Random.Range(0, _seatsPerRow);
			
			Person person = rows[row].GetPerson(seat);
			if (!_questionAudience.Contains(person))
			{
				person.ChangeStandPosition("Question");
				_questionAudience.Add(person);
			}

			Debug.Log(_questionAudience.Count);
			yield return null;
		}
	}
	
	/// <summary>
	/// Makes the people that weren't chosen sit down
	/// </summary>
	/// <param name="person">Person that should stay up</param>
	public void SitOther(Person person)
	{
		_questionAudience.Remove(person);
		
		for (int i = 0; i < _questionAudience.Count; i++)
		{
			_questionAudience[i].ChangeStandPosition("Sit");
		}
	}

	/// <summary>
	/// Activates the colliders for the question.
	/// </summary>
	public void ActivateColliders()
	{
		_questionAudience = new List<Person>();
		StartCoroutine(ActivateColliderCoroutine());
	}
	
	/// <summary>
	/// Coroutine that activates the colliders.
	/// </summary>
	/// <returns></returns>
	private IEnumerator ActivateColliderCoroutine()
	{
		int j = 0;
        for(int row = 0; row < initializedRows; row++)
        	for (int i = 0; i < _seatsPerRow; i++)
        	{
        		colliders[j++].AssignPerson(rows[row].GetPerson(i));
                yield return null;
            }
	}

	/// <summary>
	/// Starts the final clap of the audience
	/// </summary>
	public void StartFinalClap()
	{
		StartCoroutine(ClapCoroutine());
	}
	
	/// <summary>
	/// Coroutine that starts the final clap of the audience
	/// </summary>
	private IEnumerator ClapCoroutine()
	{
		List<Person> persons = ObjectPoolManager.Instance.GetPool();

		foreach (Person person in persons)
		{
			person.Clap();
			yield return null;
		}
	}
}

