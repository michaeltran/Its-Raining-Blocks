using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractDetector : MonoBehaviour {

	protected List<GameObject> _objectsInTrigger = new List<GameObject> ();
	
	protected void DetectDeletedObjects()
	{
		_objectsInTrigger.RemoveAll (item => item == null);
	}
	
	protected void OnTriggerExit (Collider other)
	{
		if (other.gameObject.CompareTag ("Destructable")) {
			_objectsInTrigger.Remove (other.gameObject);
		}
	}
}
