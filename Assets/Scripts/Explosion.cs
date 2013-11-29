using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
	public GameObject bombExplosion;
	Vector3 location;
	
	//public AudioClip ExplosionSound;
	//public int ExplosionCount=1;

	void Update ()
	{
		CheckGrounded ();
	}
	
	void CheckGrounded ()
	{
		RaycastHit[] hits = null;
		hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), -transform.up, 1.4f);
		
		if (hits.Length > 0) {
			if (DetectColliderTag (hits)) {
				AreaDamage ();
				//PlaySound();
				Destroy (this.gameObject);
			}
		}
	}
	
	bool DetectColliderTag (RaycastHit[] hits)
	{
		foreach(RaycastHit hit in hits)
		{
			string colliderTag = hit.collider.tag;
			if (colliderTag == "Destructable" || colliderTag == "StageBorder" || colliderTag == "Player")
				return true;
		}
		return false;
	}
	
	void AreaDamage ()
	{
		Instantiate (bombExplosion, this.gameObject.transform.position, Quaternion.identity);	
	}
	
/*	void PlaySound()
	{
		AudioSource.PlayClipAtPoint(ExplosionSound, transform.position);
		if (ExplosionCount >1)
		{
			for(int i=0; i<ExplosionCount-1; i++)
			{
				
				//AudioSource.PlayClipAtPoint (ExplosionSound, transform.position);
				Invoke("InvokeExplosionSound",0.5f);
				//timeDelay+=.25f;
			}
		}
	}
	
	void InvokeExplosionSound()
	{
		AudioSource.PlayClipAtPoint(ExplosionSound, transform.position);		
	}*/
}