using UnityEngine;
using System.Collections;

public class BlockSpawner : MonoBehaviour
{
	public GameObject prefab;
	
	//Spawn a default block
	public void LaunchBlock ()
	{
		Instantiate (prefab, this.transform.position, Quaternion.identity);
	}
	
	//Spawn a special block
	public void LaunchBlock (GameObject theObject)
	{
		Instantiate (theObject, this.transform.position, Quaternion.identity);
	}
}
