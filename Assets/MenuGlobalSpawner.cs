using UnityEngine;
using System.Collections;

public class MenuGlobalSpawner : GlobalSpawner
{
	override public void _Start ()
	{
		InvokeRepeating ("SpawnABlock", startTime, rateOfSpawn);
	}
}
