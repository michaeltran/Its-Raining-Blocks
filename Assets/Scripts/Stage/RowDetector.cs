using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RowDetector : MonoBehaviour
{
	private List<GameObject> _objectsInTrigger = new List<GameObject> ();
	
	void DetectDeletedObjects()
	{
		_objectsInTrigger.RemoveAll (item => item == null);
	}
	
	void OnTriggerEnter (Collider other)
	{
		DetectDeletedObjects();
		
		if (other.gameObject.CompareTag ("Destructable")) {
			_objectsInTrigger.Add (other.gameObject);
		
			if (_objectsInTrigger.Count > 19) {
				foreach (GameObject obj in _objectsInTrigger) {
					Vector3 target = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z+20);
					obj.transform.position = target;
					GarbageCollection gc = (GarbageCollection) obj.GetComponent (typeof(GarbageCollection));
					gc.DestroyObject();
				}
			
			}
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.CompareTag ("Destructable")) {
			_objectsInTrigger.Remove (other.gameObject);
		}
	}
}
