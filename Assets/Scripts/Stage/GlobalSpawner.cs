﻿using UnityEngine;
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
	public GameObject[] specialBlock;
	public GameObject[] healthPotions;
	public GameObject[] manaPotions;
	public GameObject[] explosives;
	private bool spawnStuff = true;
	private GameObject[] spawners;
	private Queue<GameObject> objectQueue = new Queue<GameObject>();
	
	void Start ()
	{
		CreateSpawners ();
		spawners = GameObject.FindGameObjectsWithTag ("Spawner");
		
		InvokeRepeating ("SpawnABlock", 2f, 0.3f);
		InvokeRepeating ("addSpecialBlockToSpawn", 2.5f, 5f);
		//InvokeRepeating ("spawnExplosive", 1f, 3f);
	}
	void spawnExplosive()
	{ // 0- Bomb, 1- Bunker Buster, 2-Napalm
		int random = Random.Range(0,3);
		addToQueue (explosives[random]);	
	}
	void addSpecialBlockToSpawn ()
	{
		int randomIndex = Random.Range (0, specialBlock.Length);
		addToQueue (specialBlock[randomIndex]);
	}
	
	void addHealthPotionToSpawn ()
	{
		addToQueue (healthPotions[0]);
	}
	
	void addManaPotionToSpawn ()
	{
		addToQueue (manaPotions[0]);
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
		if (spawnStuff)
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
	
	void setSpawnStuff (bool temp)
	{
		spawnStuff = temp;
	}
}
