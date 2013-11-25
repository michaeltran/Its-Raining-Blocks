using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour 
{

	public GameObject bombExplosion;
	//public GameObject bbExplosion;
	//public GameObject napalmExplosion;
	//public GameObject prefabSpawner;
	Vector3 location;
	
	public AudioClip ExplosionSound;
	public int ExplosionCount=1	;
	public float timeDelay=0f;
	// Use this for initialization
	void Start () 
	{
	
		
		
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	//	checkLandedOnFloor();
	//	checkLandedOnChar();
		CheckGrounded ();
	}
	

/*	void checkLandedOnChar()
	{
		RaycastHit[] hits = null;
		hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), transform.up, 1.3f);
		
		if (hits.Length > 0  && hits[0].collider.tag == "Destructable")
		{
			// Take DMG from block
			Instantiate(Poof, this.gameObject.transform.position, Quaternion.identity);
			Vector3 startPosition = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y+40, this.gameObject.transform.position.z);
			this.gameObject.transform.position = startPosition;
		}
		areaDamage();
	}*/
	
/*	void checkLandedOnFloor()
	{
		/*location = this.transform.position;
		if (Physics.Raycast(location,transform.up, -1))
		{
			areaDamage();
		}
		RaycastHit[] hits = null;
		hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), transform.up, 1.3f);
		
		if (hits.Length > 0 && CheckGrounded () && (hits[0].collider.tag == "Destructable"||hits[0].collider.tag=="PlayerCollider")) {
			AudioSource.PlayClipAtPoint (ExplosionSound, transform.position);
			areaDamage ();
		}
	}*/
	
	void CheckGrounded ()
	{
		RaycastHit[] hits = null;
		hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), -transform.up, 1.4f);
		
		if(hits.Length > 0)
		{
			string colliderTag = hits[0].collider.tag;
			if(colliderTag == "Destructable" ||colliderTag == "Untagged"||colliderTag=="Player")
			{
				areaDamage();
				Destroy (this.gameObject);
			}
		}
	}
	
	void InvokeExplosionSound()
	{
		AudioSource.PlayClipAtPoint(ExplosionSound, transform.position);
		//AudioSource.
		
	}
	void areaDamage()
	{
		Instantiate(bombExplosion, this.gameObject.transform.position, Quaternion.identity);	
		//Destroy(bombExplosion);
		
		AudioSource.PlayClipAtPoint(ExplosionSound, transform.position);
		if (ExplosionCount >1)
		{
			for(int i=0; i<ExplosionCount-1; i++)
			{
				
				//AudioSource.PlayClipAtPoint (ExplosionSound, transform.position);
				Invoke("InvokeExplosionSound",timeDelay);
				timeDelay+=.25f;
			}
		}
	/*	int rand = Random.Range (1,3); //1- bomb,  2- bunker buster, 3- napalm
		switch (rand)
		{
			case (1):
			Instantiate (bombExplosion, this.gameObject.transform.position,Quaternion.identity);
			break;
			
			case (2):
			Instantiate (bbExplosion, this.gameObject.transform.position, Quaternion.identity);
			break;
			
			case (3):
			Instantiate (napalmExplosion, this.gameObject.transform.position, Quaternion.identity);
			break;
		}*/
	}

}