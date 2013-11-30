using UnityEngine;
using System.Collections;

public class HauntedHouseGlobalSpawner : GlobalSpawner {
	public GameObject enemyShieldBlock;
	private GameObject batSpawner;
	
	override public void _Start ()
	{
		batSpawner = GameObject.Find ("BatSpawner");
		InvokeRepeating ("SpawnABlock", startTime, rateOfSpawn);
		InvokeRepeating ("addSpecialBlockToSpawn", 2.5f, 5f);
		InvokeRepeating ("addExplosiveToSpawn", 1f, 10f);
		InvokeRepeating ("SpawnABat", 0f, 25f);
		InvokeRepeating ("addEnemyShieldBlockToSpawn", 10f, 45f);
	}
	
	void SpawnABat ()
	{
		if(batSpawner != null)
			batSpawner.SendMessage("LaunchObject");
	}
	
	void addEnemyShieldBlockToSpawn ()
	{
		if(enemyShieldBlock != null)
			addToQueue(enemyShieldBlock);
	}
}
