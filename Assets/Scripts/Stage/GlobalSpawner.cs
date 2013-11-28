using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//USE THIS AS A GUIDE TO SCRIPT STAGES
//If you're pro you can script stages to follow the beat using invokerepeating() and cancelinvoke()
//If you REALLY want precision use ALOT of invokes

public abstract class GlobalSpawner : MonoBehaviour
{
	public int numberOfSpawners = 21;		// default number of columns per stage is 21
	public float startTime = 2f;			// time before the first block spawns
	public float rateOfSpawn = 0.3f;		// time between spawns
	public GameObject prefabSpawner;		// spawners
	public GameObject[] specialBlocks;
	public GameObject[] healthPotions;
	public GameObject[] manaPotions;
	public GameObject[] explosives;
	private bool _spawnStuff = true;
	private GameObject[] _spawners;
	private Queue<GameObject> _objectQueue = new Queue<GameObject> ();
	
	void Start ()
	{
		CreateSpawners ();
		_spawners = GameObject.FindGameObjectsWithTag ("Spawner");
		_Start ();
	}
	
	abstract public void _Start ();
	
	void addExplosiveToSpawn ()
	{ 
		int randomIndex = Random.Range (0, explosives.Length);
		addToQueue (explosives [randomIndex]);	
	}

	void addSpecialBlockToSpawn ()
	{
		int randomIndex = Random.Range (0, specialBlocks.Length);
		addToQueue (specialBlocks [randomIndex]);
	}
	
	void addHealthPotionToSpawn ()
	{
		int randomIndex = Random.Range (0, healthPotions.Length);
		addToQueue (healthPotions [randomIndex]);
	}
	
	void addManaPotionToSpawn ()
	{
		int randomIndex = Random.Range (0, manaPotions.Length);
		addToQueue (manaPotions [0]);
	}
	
	void CreateSpawners ()
	{
		Vector3 position = new Vector3 (3.65f, 35f, 0f);
		for (int i = 0; i < numberOfSpawners; i++) {
			Instantiate (prefabSpawner, position, Quaternion.identity);
			position.x += 2.5f;
		}
	}
	
	void addToQueue (GameObject theObject)
	{
		_objectQueue.Enqueue (theObject);
	}
	
	void SpawnABlock ()
	{
		if (_spawnStuff) {
			int randomIndex = Random.Range (0, _spawners.Length);
			BlockSpawner other = (BlockSpawner)_spawners [randomIndex].GetComponent (typeof(BlockSpawner));
			if (_objectQueue.Count > 0) {
				other.LaunchBlock (_objectQueue.Dequeue ());
			} else {
				other.LaunchBlock ();
			}
		}
	}
	
	void setSpawnStuff (bool temp)
	{
		_spawnStuff = temp;
	}
}
