using UnityEngine;
using System.Collections;

public class HauntedHouseGlobalSpawner : GlobalSpawner {

	override public void _Start ()
	{
		InvokeRepeating ("SpawnABlock", startTime, rateOfSpawn);
		InvokeRepeating ("addSpecialBlockToSpawn", 2.5f, 5f);
		InvokeRepeating ("addExplosiveToSpawn", 1f, 10f);
	}
}
