using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColumnDetector : MonoBehaviour
{
	private int objects;
	private List<GameObject> objectsInTrigger = new List<GameObject> ();
	
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnTriggerEnter (Collider other)
	{
		objects++;
		
		if (other.gameObject.CompareTag ("Destructable")) {
			objectsInTrigger.Add (other.gameObject);
		
			if (objectsInTrigger.Count > 10) {
				
				for(int i = 0; i < objectsInTrigger.Count/2; i++)
				{
					GameObject obj = objectsInTrigger[i];
					Vector3 target = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z + 20);
					obj.transform.position = target;
					GarbageCollection gc = (GarbageCollection)obj.GetComponent (typeof(GarbageCollection));
					gc.DestroyObject ();
				}
				/*
				foreach (GameObject obj in objectsInTrigger) {
					Vector3 target = new Vector3 (obj.transform.position.x, obj.transform.position.y, obj.transform.position.z + 20);
					obj.transform.position = target;
					GarbageCollection gc = (GarbageCollection)obj.GetComponent (typeof(GarbageCollection));
					gc.DestroyObject ();
				}*/
			
			}
		}
	}
	
	void removeObjects()
	{
		
	}
	
	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.CompareTag ("Destructable")) {
			objects--;
			objectsInTrigger.Remove (other.gameObject);
		}
	}
}
