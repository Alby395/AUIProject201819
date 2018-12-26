using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ObjectPoolManager : MonoBehaviour
{
	public static ObjectPoolManager Instance { get; private set; }
    public bool Ready { get; private set; }
	[Range(8, 40)]
	[SerializeField] private int _numberOfPeople;
	
	[Header("Prefabs")]
	[SerializeField] private GameObject _indifferentFemalePrefab;
	[SerializeField] private GameObject _indifferentMalePrefab;
	[SerializeField] private GameObject _kindFemalePrefab;
	[SerializeField] private GameObject _kindMalePrefab;
	[SerializeField] private GameObject _seriousMalePrefab;
	[SerializeField] private GameObject _seriousFemalePrefab;
	
    private List<GameObject> _people;
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
		_people = new List<GameObject>();

		StartCoroutine(InitPoolCoroutine());
	}

	/// <summary>
	/// Returns a given person to the ObjectPoolManager
	/// </summary>
	/// <param name="person">Person that is returning in the ObjectPoolManager</param>
	public void ReturnToPool(GameObject person)
    {
	    person.transform.position = transform.position;
	    person.SetActive(false);
	    _seatedPeople--;
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
	/// Returns the number of people that is still in the ObjectPoolManager.
	/// </summary>
	/// <returns>The number of people remaining.</returns>
	public int RemainingElement()
	{
		return _numberOfPeople - _seatedPeople;
	}
	
	private IEnumerator InitPoolCoroutine()
	{
		for (int i = 0; i < _numberOfPeople; i++)
		{
			
			GameObject person = Instantiate(PickNextPrefab());

			person.transform.position = transform.position;
			person.SetActive(false);
			
			_people.Add(person);

			yield return null;
		}

		Ready = true;
	}

	private GameObject PickNextPrefab()
	{
		float type = Random.value;
		float gender = Random.value;

		Debug.Log("Type: " + type);
		Debug.Log("Gender: " + gender);
		
		if (type <= 0.33f)
		{
			if(gender <= 0.5f)
				return _indifferentFemalePrefab;
			
			return _indifferentMalePrefab;
		}
		
		if (type >= 0.66f)
		{
			if(gender <= 0.5f)
				return _kindFemalePrefab;
			
			//return _kindMalePrefab;
		}
		
		if(gender <= 0.5f)
				return _seriousFemalePrefab;
			
			return _seriousMalePrefab;
	}
}