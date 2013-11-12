using UnityEngine;
using System.Collections;

public class CustomPhysics : MonoBehaviour {
	public float gravity = -10f;
	public float maxSpeed = 10f;
	
	void FixedUpdate () {
		if(rigidbody.velocity.magnitude < maxSpeed)
		{
			rigidbody.AddRelativeForce(Vector3.up * gravity);
		}
	}
}
