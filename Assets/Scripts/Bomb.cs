using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour 
{

	public GameObject bombExplosion;
	public GameObject bbExplosion;
	public GameObject naplmExplosion;
	public GameObject prefabSpawner;
	private GameObject[] spawners;
	public int numberOfSpawners;
	Vector3 location;
	// Use this for initialization
	void Start () 
	{
		CreateSpawners ();
		spawners = GameObject.FindGameObjectsWithTag ("Spawner");
		
		InvokeRepeating ("SpawnABlock", 3f, 3f);
		
		
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		checkLandedOnFloor();
	//	checkLandedOnChar();
	}
	
	void CreateSpawners ()
	{
		Vector3 position = new Vector3 (3.65f, 35f, 0f);
		for (int i = 0; i < numberOfSpawners; i++) {
			Instantiate (prefabSpawner, position, Quaternion.identity);
			position.x += 2.5f;
		}
	}
	
	void SpawnABlock ()
	{
		int rand = Random.Range (1,3); //1- bomb,  2- bunker buster, 3- napalm
		BlockSpawner other = (BlockSpawner)spawners[rand].GetComponent(typeof(BlockSpawner));

		other.LaunchBlock ();
		
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
	
	void checkLandedOnFloor()
	{
		location = this.transform.position;
		if (Physics.Raycast (location,transform.up, -1))
		{
			areaDamage();
		}
	}
	
	void areaDamage()
	{
		int rand = Random.Range (1,3); //1- bomb,  2- bunker buster, 3- napalm
		switch (rand)
		{
			case (1):
			Instantiate (bombExplosion, this.gameObject.transform.position,Quaternion.identity);
			break;
			
			case (2):
			Instantiate (bbExplosion, this.gameObject.transform.position, Quaternion.identity);
			break;
			
			case (3):
			Instantiate (naplmExplosion, this.gameObject.transform.position, Quaternion.identity);
			break;
		}
	}

}