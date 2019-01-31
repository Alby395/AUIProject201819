using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TheaterManager : MonoBehaviour
{
	public static TheaterManager Instance { get; private set; }

	[Header("Light related variables")] [SerializeField]
	private List<Light> spotlights;

	[SerializeField] private List<Row> rows;

	[SerializeField] private int initializedRows = 2;

	private int _seatsPerRow;
	private int _currentLevel;
	private int _maxLevel;
	
	private Coroutine _coroutine;
	private bool _add;
	private bool _changing;
	
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
		_seatsPerRow = rows[0].SeatPerRow();
		StartCoroutine(InitTheater());
	}

	private IEnumerator InitTheater()
	{
		int row = 0;
		int decrease = 1;
		_maxLevel = -2;
		
		while (!ObjectPoolManager.Instance.Ready)
			yield return null;

		while (ObjectPoolManager.Instance.RemainingElement() && row < 4)
		{
			List<GameObject> people = ObjectPoolManager.Instance.GivePeople(_seatsPerRow);
			if (people.Count < _seatsPerRow)
				yield return StartCoroutine(rows[row++].AssignRandom(people));
			else
				yield return StartCoroutine(rows[row++].AssignAll(people));

			_maxLevel++;
		}
	
		while (ObjectPoolManager.Instance.RemainingElement())
		{
			List<GameObject> people = ObjectPoolManager.Instance.GivePeople(_seatsPerRow - decrease);

			yield return StartCoroutine(rows[row++].AssignRandom(people));

			decrease++;

			_maxLevel++;
		}

		for (row = 0; row < initializedRows; row++)
		{
			yield return StartCoroutine(rows[row].ActivatePeople());
		}
	}
	
	public IEnumerator IncreaseLevel()
	{
		_changing = true;

		spotlights[_currentLevel - 1].enabled = false;
		spotlights[_currentLevel].enabled = true;
		
		yield return StartCoroutine(rows[_currentLevel + initializedRows].ActivatePeople());

		_currentLevel++;

		_changing = false;
	}
	
	public IEnumerator DecreaseLevel()
	{
		_changing = false;

		spotlights[_currentLevel].enabled = false;
		
		_currentLevel--;
		
		spotlights[_currentLevel].enabled = true;
		
		yield return StartCoroutine(rows[_currentLevel].FreeRow());		
	}
	
	public bool HighestLevel()
	{
		return _currentLevel == _maxLevel;
	}

	public bool LowestLevel()
	{
		return _currentLevel == 0;
	}

	public bool Working()
	{
		return _changing;
	}
}
