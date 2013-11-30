using UnityEngine;
using System.Collections;

public class GarbageCollection : MonoBehaviour {

	public float timeToDestroy = 5f;
	public bool callImmediately = false;
	
	void Start()
	{
		if(callImmediately == true)
		{
			DestroyObject ();
		}
	}
	
	public void DestroyObject()
	{
		Invoke("_DestroyObject", timeToDestroy);
	}
	
	void _DestroyObject()
	{
		Destroy(this.gameObject);
	}
}
