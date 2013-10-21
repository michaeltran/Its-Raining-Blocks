using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public float speed = 10f;
	public ParticleSystem Poof;
	
	private float _speed;
	private bool _didDamage;

	// Use this for initialization
	void Start () {
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
			other.gameObject.SendMessage ("TakeDamage", 50);
			Instantiate(Poof, other.transform.position, Quaternion.identity);
			//Destroy(this.gameObject);
		}
	}
}
