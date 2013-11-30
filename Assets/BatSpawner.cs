using UnityEngine;
using System.Collections;

public class BatSpawner : MonoBehaviour {

	public GameObject prefab;
	
	public void LaunchObject ()
	{
		Instantiate (prefab, this.transform.position, Quaternion.identity);
	}
	
	public void LaunchObject (GameObject theObject)
	{
		Instantiate (theObject, this.transform.position, Quaternion.identity);
	}
}
