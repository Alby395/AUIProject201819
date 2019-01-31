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
	public int GetRemainingElement()
	{
		return _numberOfPeople - _seatedPeople;
	}

	public bool RemainingElement()
	{
		return _seatedPeople == _numberOfPeople;
	}
	
	private IEnumerator InitPoolCoroutine()
	{

		_numberOfPeople = MainMenuManager.Instance.GetAudience();
		_kind = MainMenuManager.Instance.GetKind();
		_indifferent = MainMenuManager.Instance.GetIndifferent();
		_serious = MainMenuManager.Instance.GetSerious();
		
		for (int i = 0; i < _numberOfPeople; i++)
		{
			GameObject person = Instantiate(PickNextPrefab());

			person.transform.position = transform.position;
			person.SetActive(false);
			
			_people.Add(person);

			yield return null;
		}

		for (int i = 0; i < _people.Count; i++)
		{
			GameObject temp = _people[i];
			int randomIndex = Random.Range(i, _people.Count);
			_people[i] = _people[randomIndex];
			_people[randomIndex] = temp;

			yield return null;
		}
		
		Ready = true;
	}

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
}