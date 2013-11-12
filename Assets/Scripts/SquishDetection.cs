using UnityEngine;
using System.Collections;

public class SquishDetection : MonoBehaviour
{
	public ParticleSystem Poof;
	
	// Update is called once per frame
	void Update ()
	{	
		CheckSquashed ();
	}
	
	
	void CheckSquashed ()
	{
		RaycastHit[] hits = null;
		hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), transform.up, 1.3f);
		
		if (hits.Length > 0 && CheckGrounded () && hits[0].collider.tag == "Destructable") {
			// Take DMG from block
			Status status = (Status)this.gameObject.GetComponent ("Status");
			status.TakeDamage (40);
			Instantiate(Poof, this.gameObject.transform.position, Quaternion.identity);
			Vector3 startPosition = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y+40, this.gameObject.transform.position.z);
			this.gameObject.transform.position = startPosition;
		}
	}
	
	bool CheckGrounded ()
	{
		RaycastHit[] hits = null;
		hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), -transform.up, 1.4f);
		
		if(hits.Length > 0)
		{
			string colliderTag = hits[0].collider.tag;
			if(colliderTag == "Destructable" || colliderTag == "Untagged")
			{
				return true;
			}
		}
		return false;
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
