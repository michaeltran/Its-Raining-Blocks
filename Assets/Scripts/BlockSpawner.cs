using UnityEngine;
using System.Collections;

public class BlockSpawner : MonoBehaviour {
	
	public GameObject prefab;
	private GameObject instObj;
	private GameObject id;
	
	// Use this for initialization
	void Start () {
		//InvokeRepeating ("LaunchBlock", 2f, 2f);
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	//Spawn a default block
	public void LaunchBlock() {
		Instantiate (prefab, this.transform.position, Quaternion.identity);
	}
	
	//Spawn a special block
	public void LaunchBlock(GameObject theObject)
	{
		Instantiate (theObject, this.transform.position, Quaternion.identity);
	}
}
