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
		if(other.gameObject.CompareTag ("Enemy") && !_didDamage)
		{
			_didDamage = true;
			other.gameObject.SendMessage ("TakeDamage", damageDealt);
			Instantiate(Poof, other.transform.position, Quaternion.identity);
			//Destroy(this.gameObject);
		}
	}
}