using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//USE THIS AS A GUIDE TO SCRIPT STAGES
//If you're pro you can script stages to follow the beat using invokerepeating() and cancelinvoke()
//If you REALLY want precision use ALOT of invokes

public class GlobalSpawner : MonoBehaviour
{

	public int numberOfSpawners;
	public float startTime = 2f;
	public float rateOfSpawn = 2f; //in seconds
	public GameObject prefabSpawner;
	private GameObject[] spawners;
	public GameObject test;
	private Queue<GameObject> objectQueue = new Queue<GameObject>();
	
	// Use this for initialization
	void Start ()
	{
		CreateSpawners ();
		spawners = GameObject.FindGameObjectsWithTag ("Spawner");
		
		InvokeRepeating ("SpawnABlock", 2f, 0.25f);
		addToQueue (test);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
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
		objectQueue.Enqueue (theObject);
	}
	
	void SpawnABlock ()
	{
		int randomIndex = Random.Range (0, spawners.Length);
		BlockSpawner other = (BlockSpawner)spawners [randomIndex].GetComponent (typeof(BlockSpawner));
		if (objectQueue.Count > 0) {
			other.LaunchBlock (objectQueue.Dequeue());
		} else {
			other.LaunchBlock ();
		}
	}
}
