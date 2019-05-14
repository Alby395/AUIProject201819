using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ObjectPoolManager : MonoBehaviour
{
	public static ObjectPoolManager Instance { get; private set; }
    public bool Ready { get; private set; }
    
    private int _numberOfPeople;
    private int _kind;
    private int _indifferent;
    private int _serious;
    
	[Header("Prefabs")]
	[SerializeField] private List<GameObject> kindPrefabs;
	[SerializeField] private List<GameObject> indifferentPrefabs;
	[SerializeField] private List<GameObject> seriousPrefabs;
	
    private List<Person> _people;
	private int _seatedPeople;

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
		_people = new List<Person>();
		
		StartCoroutine(InitPoolCoroutine());
	}

	/// <summary>
	/// Returns a given person to the ObjectPoolManager
	/// </summary>
	/// <param name="person">Person that is returning in the ObjectPoolManager</param>
	public void ReturnToPool(Person person)
    {
	    person.transform.position = transform.position;
	    person.gameObject.SetActive(false);
	    _seatedPeople--;
    }

	/// <summary>
	/// Gives a certain amount of people.
	/// </summary>
	/// <param name="n">Number of people to give.</param>
	/// <returns>List of people of length n.</returns>
	public List<Person> GivePeople(int n)
	{
		List<Person> people;

		if (n <= _numberOfPeople - _seatedPeople)
		{
			people = _people.GetRange(_seatedPeople, n);
		}
		else
		{
			people = _people.GetRange(_seatedPeople, _numberOfPeople - _seatedPeople);
			n = _people.Count;
		}

		_seatedPeople += n;
		
		return people;
	}
	
	/// <summary>
	/// Checks whether there are people that are not assigned
	/// </summary>
	/// <returns>True if there are people available, false otherwise</returns>
	public bool RemainingElement()
	{
		return _seatedPeople < _numberOfPeople;
	}
	
	/// <summary>
	/// Coroutine that initializes the pool
	/// </summary>
	/// <returns></returns>
	private IEnumerator InitPoolCoroutine()
	{
		
		_numberOfPeople = MainMenuManager.Instance.GetAudience();
		_kind = MainMenuManager.Instance.GetKind();
		_indifferent = MainMenuManager.Instance.GetIndifferent();
		_serious = MainMenuManager.Instance.GetSerious();
		
		Debug.Log("Audience: " + _numberOfPeople);
		Debug.Log("Kind: " + _kind);
		Debug.Log("Indifferent: " + _indifferent);
		Debug.Log("Serious: " + _serious);

		_kind += _numberOfPeople - (_kind + _indifferent + _serious);
		
		for (int i = 0; i < _numberOfPeople; i++)
		{
			GameObject person = Instantiate(PickNextPrefab());

			person.transform.position = transform.position;
			person.SetActive(false);
			
			_people.Add(person.GetComponent<Person>());

			yield return null;
		}

		for (int i = 0; i < _people.Count; i++)
		{
			Person temp = _people[i];
			int randomIndex = Random.Range(i, _people.Count);
			_people[i] = _people[randomIndex];
			_people[randomIndex] = temp;

			yield return null;
		}

		Ready = true;
	}

	/// <summary>
	/// Picks the next Prefab that is going to be instantiated
	/// </summary>
	/// <returns>The prefab to instantiate</returns>
	private GameObject PickNextPrefab()
	{
		if (_kind > 0)
		{
			return kindPrefabs[Random.Range(0, kindPrefabs.Count)];
		}
		
		if (_indifferent > 0)
		{
			return indifferentPrefabs[Random.Range(0, indifferentPrefabs.Count)];
		}

		return seriousPrefabs[Random.Range(0, seriousPrefabs.Count)];

     }

	/// <summary>
	/// Gives the list of all the people in the pool
	/// </summary>
	/// <returns>The list of people in the pool.</returns>
	public List<Person> GetPool()
	{
		return _people;
	}

	/// <summary>
	/// Starts the coroutine that destroys every person in the pool.
	/// </summary>
	public void DestroyEverything()
	{
		StartCoroutine(DestroyEverythingCoroutine());
	}

	/// <summary>
	/// Coroutine that destroys every person in the pool.
	/// </summary>
	/// <returns></returns>
	private IEnumerator DestroyEverythingCoroutine()
	{
		yield return new WaitForSeconds(5f);
		for (int i = 0; i < _people.Count; i++)
		{
			Destroy(_people[i].gameObject);

			yield return null;
		}
	}
}