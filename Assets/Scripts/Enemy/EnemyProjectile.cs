using UnityEngine;
using System.Collections;

public class EnemyProjectile : MonoBehaviour {
	
	public Rigidbody projectile;
	float speed = 0.0001f;
	
	void Update()
	{
		if(Input.GetButtonDown("Fire1"))
		{
			Rigidbody clone;
			clone = (Rigidbody)Instantiate (projectile, GameObject.Find ("Player").transform.position, transform.rotation);
			
			clone.velocity = (GameObject.Find ("Player").transform.position - transform.position).normalized * speed;
			//clone.velocity.z = 0;
			
			Destroy(clone.gameObject, 10);
		}
	}
	
	/*
	public Transform target;
	private float delayAmount = 2f;
	private float forceMultiplier = 1.5f;
	private float timeDelay = 0.0;
	
	void Update() {
		Vector3 targetVector = target.position;
		Vector3 sourceVector = transform.position;
		Vector3 newVector = Vector3((target.position.x - transform.position.x), (target.position.y - transform.position.y), (target.position.z - transform.position.z));
		
		float emitterYPos = transform.position.y;
		float Ay = Physics.gravity.y;
		float t = Mathf.Abs ((2*emitterYPos) / (Ay));
		
		float targetXPos = target.position.x - transform.position.x;
		float targetZPos = target.position.z - transform.position.z;
		
		float Ax = 0;
		
		float Vix = targetXPos * (Mathf.Abs (Ay/ (2*emitterYPos)));
		float Viz = targetZPos * (Mathf.Abs (Ay/ (2*emitterYPos)));
		
		if(Time.time > timeDelay)
		{
			timeDelay = Time.time + delayAmount;
			
			
		}
	}
	*/
	
}
