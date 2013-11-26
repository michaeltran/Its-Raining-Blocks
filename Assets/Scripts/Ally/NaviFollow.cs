using UnityEngine;
using System.Collections;

public class NaviFollow : MonoBehaviour {

	public float speed;
	public Transform Target;
	
	void Update() {
		if(Target != null)
		{
			float step = speed*Time.deltaTime;
			Vector3 targetDir = Target.position - transform.position;
			Vector3 targetArea = new Vector3(Target.position.x-2, Target.position.y+2, Target.position.z);
			transform.position = Vector3.MoveTowards (transform.position, targetArea, step);
			transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2 (targetDir.y, targetDir.x) * Mathf.Rad2Deg + 90);
		}
	}
	
}
