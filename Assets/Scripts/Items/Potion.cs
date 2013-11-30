using UnityEngine;
using System.Collections;

public abstract class Potion : MonoBehaviour
{
    public AudioClip potionGrab;
	public float fallSpeed = 10f;
	
	void Update()
	{
		if(checkGrounded () == false)
			transform.Translate (-Vector3.up * fallSpeed * Time.deltaTime);
	}
	
	bool checkGrounded()
	{
		RaycastHit[] hits = null;
		hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), -transform.up, 0.7f);
		if(hits.Length > 0 && checkTag (hits))
		{
			return true;
		}
		return false;
	}
	
	bool checkTag(RaycastHit[] hits)
	{
		foreach(RaycastHit hit in hits)
		{
			string colliderTag = hit.collider.tag;
			if (colliderTag == "Destructable" || colliderTag == "StageBorder" || colliderTag == "Player")
				return true;
		}
		return false;
	}
}