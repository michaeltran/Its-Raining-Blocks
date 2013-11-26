using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
	private float xSpeed;
	private float ySpeed;
	private float xVelocity;
	private float yVelocity;
	private float gravity;
	
	private bool onFloor;
	protected CharacterController control;

	// Use this for initialization
	void Start ()
	{
		control = GetComponent<CharacterController> ();
		if (control == null) {
			Debug.LogError ("No character controller set for '" + name + "");
		}
		xSpeed = 20f;
		ySpeed = 50f;
		
		yVelocity = 0;
		xVelocity = 0;
		
		gravity = -3f;
		
		onFloor = false;
	}

	
	
	// Update is called once per frame
	void Update ()
	{
		
		Fall ();
		if (Input.GetAxisRaw ("Horizontal") != 0) {
			
			xVelocity = Input.GetAxisRaw ("Horizontal") * xSpeed ;
			xVelocity *= Time.deltaTime;
			
		}
		else {
			xVelocity = 0;
		}
		
		if (Input.GetAxisRaw ("Vertical") > 0 && onFloor) {
			yVelocity = Input.GetAxisRaw ("Vertical") *ySpeed;
			yVelocity *= Time.deltaTime;
		}
		
		
		Vector3 move = new Vector3(xVelocity, yVelocity, 0);
		control.Move(move);
		
	}
	
	void Fall ()
	{
		RaycastHit[] hits = null;
		
		hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), -transform.up, 1.2f);
		
		if (hits.Length == 0) {
			onFloor = false;
		} else {
			onFloor = true;
		}
		
		if (!onFloor) {
			yVelocity += gravity * Time.deltaTime;
		} else {
			yVelocity = 0;
		}
		
		
		Debug.Log ("hits: " + hits.Length);
	}
	
	void Jump ()
	{
	}
}
