using UnityEngine;
using System.Collections;

public class SquishDetection : MonoBehaviour
{
	public AudioClip SquishSound;
	public ParticleSystem SquishPoof;
	
	private RaycastCharacterController _rcc;
	private Status _status;
	
	void Start ()
	{
		_rcc = (RaycastCharacterController)this.gameObject.GetComponent ("RaycastCharacterController");
		_status = (Status)this.gameObject.GetComponent ("Status");
	}
	
	void Update ()
	{	
		CheckSquashed ();
	}

	void CheckSquashed ()
	{
		RaycastHit[] hits = null;
		hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), transform.up, 1.4f);
		
		if (hits.Length > 0 && _rcc.IsGrounded (0.1f) && hits [0].collider.tag == "Destructable") {
			// Take DMG from block
			AudioSource.PlayClipAtPoint (SquishSound, transform.position);
			_status.TakeDamage (40);
			
			Vector3 poofPosition = new Vector3 (transform.position.x, transform.position.y, -9);
			Instantiate (SquishPoof, poofPosition, Quaternion.identity);
			
			Vector3 startPosition = new Vector3 (transform.position.x, transform.position.y + 40, transform.position.z);
			transform.position = startPosition;
		}
	}
	
	bool CheckHits (RaycastHit[] hits)
	{
		foreach(RaycastHit hit in hits)
		{
			if(hit.collider.tag == "Destructable") { return true; }
		}
		return false;
	}
	
	#region OLD CODE
	/*
	 * OLD - FOR REGULAR MOVEMENT
	bool CheckGrounded ()
	{
		Debug.Log ("checking grounded");
		RaycastHit[] hits = null;
		hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), -transform.up, 1.4f);
		
		if(hits.Length > 0) {
			foreach (RaycastHit hit in hits)
			{
				string colliderTag = hit.collider.tag;
				Debug.Log (colliderTag);
				if(colliderTag == "Destructable" || colliderTag == "StageBorder") {
					return true;
				}
			}
		}
		return false;
	}
	*/
	
	
	/*
	 * OLD - FOR CHARACTER CONTROLLER BASED MOVEMENT
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
	#endregion
}
