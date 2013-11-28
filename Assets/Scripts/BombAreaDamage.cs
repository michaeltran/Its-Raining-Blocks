using UnityEngine;
using System.Collections;

public class BombAreaDamage : MonoBehaviour
{
	public int damageTaken;
	public float timeDelay = 0f;
	
	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.CompareTag ("Destructable")) {
			CheckAreaDamage cad = (CheckAreaDamage)transform.parent.GetComponent ("CheckAreaDamage");
			if(cad._GetDidDamage() == false)
			{
				Destroy (other.gameObject);
			}
		}
		StartCoroutine (Delay ());
		if (other.gameObject.CompareTag ("PlayerCollider")) {
			CheckAreaDamage cad = (CheckAreaDamage)transform.parent.GetComponent ("CheckAreaDamage");
			if (cad.GetDidDamage () == false) {
				//other.transform.parent.Behaviour.Monobehaviour.Invoke("InvokeDamage",timeDelay);
				//StartCoroutine(Delay());
				other.gameObject.SendMessageUpwards ("TakeDamage", damageTaken);
				//InvokeDamage(other,timeDelay);
			}
		}
		
	}
/*	void InvokeDamage(Collider other, float time)
	{	
		other.gameObject.SendMessageUpwards("TakeDamage",damageTaken);
	}*/
	
	IEnumerator Delay ()
	{
		yield return new WaitForSeconds(timeDelay);	
	}
	
}


