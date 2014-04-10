using UnityEngine;
using System.Collections;

public class LaboratoryGlobalSpawner : GlobalSpawner {

	override public void _Start()
	{
		InvokeRepeating ("SpawnABlock", startTime, rateOfSpawn);
	}
	
	override public void SpawnABlock ()
	{
		if (_spawnStuff) {
			//int[] validChoices = {0,1,2,3,5,6,7,8,9,10,11,12,13,14,15,17,18,19,20};
			int[] validChoices = {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20};
			int randomIndex = GetRandom(validChoices);
			BlockSpawner other = (BlockSpawner)_spawners [randomIndex].GetComponent (typeof(BlockSpawner));
			if (_objectQueue.Count > 0) {
				other.LaunchBlock (_objectQueue.Dequeue ());
			} else {
				other.LaunchBlock ();
			}
		}
	}

	private int GetRandom(int[] validChoices)
	{
		return validChoices[Random.Range(0, validChoices.Length)];
	}
}
