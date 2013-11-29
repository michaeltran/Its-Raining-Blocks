using UnityEngine;
using System.Collections;

public class BombAreaDamage : MonoBehaviour
{
	public int damageTaken;
	private CheckAreaDamage cad;
	
	void Start ()
	{
		cad = (CheckAreaDamage)transform.parent.GetComponent ("CheckAreaDamage");
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.CompareTag ("Destructable")) {
			if (cad.GetCanDestroy ()) {
				Destroy (other.gameObject);
			}
		}
		if (other.gameObject.CompareTag ("PlayerCollider")) {
			if (cad.GetDidDamage () == false) {
				other.gameObject.SendMessageUpwards ("TakeDamage", damageTaken);
			}
		}
	}
}


