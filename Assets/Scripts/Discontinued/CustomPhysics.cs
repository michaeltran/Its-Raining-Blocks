using UnityEngine;
using System.Collections;

public class CustomPhysics : MonoBehaviour {
		private float gravity = 10f;
		private CharacterController controller;
	private Vector3 velocity = Vector3.zero;
	// Use this for initialization
	void Start () {
		//this.collider.material=null;
		controller = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		velocity.y -= gravity * Time.deltaTime;
		
		controller.Move (velocity * Time.deltaTime);
	}
}
