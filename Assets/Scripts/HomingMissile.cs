using UnityEngine;
using System.Collections;

public class HomingMissile : MonoBehaviour {

	public float speed;
	public Transform Target;
	
	void Update() {
		if(Target != null)
		{
			float step = speed*Time.deltaTime;
			Vector3 targetDir = Target.position - transform.position;
			transform.position = Vector3.MoveTowards (transform.position, Target.position, step);
			transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2 (targetDir.y, targetDir.x) * Mathf.Rad2Deg + 90);
		}
	}
	
}
