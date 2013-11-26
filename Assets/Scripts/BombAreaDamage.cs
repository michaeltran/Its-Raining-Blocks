using UnityEngine;
using System.Collections;

public class BombAreaDamage : MonoBehaviour {


	public int damageTaken;
	public float timeDelay=0f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Destructable"))
		{
			Destroy(other.gameObject);
		}
		if (other.gameObject.CompareTag ("PlayerCollider"))
		{
			GameObject parent= this.transform.parent.gameObject;
			CheckAreaDamage cad = (CheckAreaDamage) parent.GetComponent ("CheckAreaDamage");
			if (cad._DidDamage()==false)
			{
				//other.transform.parent.Behaviour.Monobehaviour.Invoke("InvokeDamage",timeDelay);
				other.gameObject.SendMessageUpwards("TakeDamage",damageTaken);
			}
		}
		
	}
	void InvokeDamage()
	{
		
		//other.gameObject.SendMessageUpwards("TakeDamage",damageTaken);
	}
	
}


