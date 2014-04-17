using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColumnDetector : AbstractDetector
{
	protected void OnTriggerEnter (Collider other)
	{
		DetectDeletedObjects ();

		if (other.gameObject.CompareTag ("Destructable")) {
			_objectsInTrigger.Add (other.gameObject);
		
			if (_objectsInTrigger.Count > 10) {
				for(int i = 0; i < _objectsInTrigger.Count/2; i++)
				{
					GameObject obj = _objectsInTrigger[i];
					Destroy (obj);
					/*
					GameObject obj = _objectsInTrigger[i];
					Vector3 target = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z + 20);
					obj.transform.position = target;
					GarbageCollection gc = (GarbageCollection)obj.GetComponent (typeof(GarbageCollection));
					gc.DestroyObject ();
					*/
				}
			}
		}
	}
}
