using UnityEngine;
using System.Collections;

public class NaviFollow : MonoBehaviour {

	public float speed = 5f;
	public Transform Target;
	private RaycastCharacterController _rcc;
	
	void Start() {
		_rcc = (RaycastCharacterController)Target.transform.gameObject.GetComponent ("RaycastCharacterController");
	}
	
	void Update() {
		if(Target != null)
		{
			Vector3 targetDir = Target.position - transform.position;
			Vector3 targetArea = new Vector3(Target.position.x-2 * _rcc.CurrentDirection, Target.position.y+2, Target.position.z);
			transform.position = Vector3.MoveTowards (transform.position, targetArea, speed * Time.deltaTime);
			transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2 (targetDir.y, targetDir.x) * Mathf.Rad2Deg + 90);
		}
	}
	
}
