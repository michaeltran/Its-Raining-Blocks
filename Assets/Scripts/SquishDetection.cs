using UnityEngine;
using System.Collections;

public class SquishDetection : MonoBehaviour
{
	
	private CharacterController controller;
	
	// Use this for initialization
	void Start ()
	{
		controller = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(controller.isGrounded)
		{
		}
		
		CheckSquashed ();
	
	}
	
	void CheckSquashed ()
	{
		RaycastHit[] hits = null;
		hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), transform.up, 1.3f);
		
		if (hits.Length > 0 && controller.isGrounded && hits[0].collider.tag == "Destructable") {
			Debug.Log("Squish Detected");
			// Take DMG from block
			Status status = (Status)this.gameObject.GetComponent ("Status");
			status.TakeDamage (100);
		}
	}
	
	/*
	void CheckSquashed ()
	{
		RaycastHit[] hits = null;
		hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), -transform.up, 1.2f);
		
		if (hits.Length > 0 && controller.isGrounded) {
			foreach (RaycastHit hit in hits) {
				if (hit.transform.gameObject == player) {
					// Squashed!!
					Status status = (Status)player.GetComponent ("Status");
					//status.TakeDamage ();
				}
			}
		}
	}
	*/
}
