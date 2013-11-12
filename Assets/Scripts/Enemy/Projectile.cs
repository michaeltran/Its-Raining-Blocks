using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public AudioClip Sound;
	public float speed = 10f;
	public int damageDealt = 10;
	public ParticleSystem Poof;
	
	
	private float _speed;
	private bool _didDamage;

	// Use this for initialization
	void Start () {
		AudioSource.PlayClipAtPoint(Sound, transform.position);
		_speed = 10f;
		_didDamage = false;
	}
	
	// Update is called once per frame
	void Update () {
		rigidbody.AddForce(Vector3.up*speed);
	}
	
	void OnTriggerEnter (Collider other)
	{
		Debug.Log ("Entered: " + other.gameObject.tag);
		if(other.gameObject.CompareTag ("Enemy") && !_didDamage)
		{
			_didDamage = true;
			other.gameObject.SendMessage ("TakeDamage", damageDealt);
			Instantiate(Poof, other.transform.position, Quaternion.identity);
			//Destroy(this.gameObject);
		}
	}
}


/*
using UnityEngine;
using System.Collections;

/// <summary>
/// A very simple projectile that moves across the screen and triggers
/// the damage method on a HitBox if it collides with one.
/// </summary>
public class Projectile : MonoBehaviour {

	public float speed = -4.0f;
	public float minX = - 50.0f;

	void Update () {
		transform.Translate(speed * RaycastCharacterController.FrameTime, 0,0 );
		if (transform.position.x < minX) GameObject.Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other) {
		HitBox health = other.gameObject.GetComponent<HitBox>();
		if (health != null) health.Damage(1);

		GameObject.Destroy(gameObject);
	}
} 
*/