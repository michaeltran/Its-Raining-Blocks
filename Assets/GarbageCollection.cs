using UnityEngine;
using System.Collections;

public class GarbageCollection : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void DestroyObject()
	{
		Invoke("_DestroyObject", 5);
	}
	
	void _DestroyObject()
	{
		Destroy(this.gameObject);
	}
}
