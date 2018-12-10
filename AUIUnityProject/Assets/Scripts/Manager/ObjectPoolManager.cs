using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
	public static ObjectPoolManager Instance { get; private set; }
    
	[Range(8, 30)]
	[SerializeField] private int _numberOfPeople;
	
	[Header("Prefabs")]
	[SerializeField] private GameObject _indifferentFemalePrefab;
	
    private List<GameObject> _people;

	private void Awake()
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

	private void Start()
	{
		_people = new List<GameObject>();
	}

	/// <summary>
	/// Returns a given person to the ObjectPoolManager
	/// </summary>
	/// <param name="person">Person that is returning in the ObjectPoolManager</param>
	public void ReturnToPool(GameObject person)
    {
	    person.transform.position = transform.position;
	    person.SetActive(false);
    	_people.Add(person);
    }

	/// <summary>
	/// Initializes the pool
	/// </summary>
	public void InitPool()	//TODO needs an int
	{
		StartCoroutine(InitPoolCoroutine());
	}

	/// <summary>
	/// Gives a certain amount of people.
	/// </summary>
	/// <param name="n">Number of people to give.</param>
	/// <returns>List of people of length n.</returns>
	public List<GameObject> GivePeople(int n)
	{
		List<GameObject> people;

		if (n <= _people.Count)
		{
			people = _people.GetRange(0, n);
		}
		else
		{
			people = _people.GetRange(0, _people.Count);
			n = _people.Count;
		}
		
		_people.RemoveRange(0, n);
		
		return people;
	}

	/// <summary>
	/// Returns the number of people that is still in the ObjectPoolManager.
	/// </summary>
	/// <returns>The number of people remaining.</returns>
	public int RemainingElement()
	{
		return _people.Count;
	}
	
	private IEnumerator InitPoolCoroutine()
	{
		for (int i = 0; i < _numberOfPeople; i++)
		{
			GameObject person = Instantiate(_indifferentFemalePrefab);

			person.transform.position = transform.position;
			
			_people.Add(person);

			yield return null;
		}
	}
}