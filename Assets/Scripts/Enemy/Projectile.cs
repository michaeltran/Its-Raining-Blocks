using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public AudioClip Sound;
	public float speed = 10f;
	public int damageDealt = 10;
	public ParticleSystem HitFX;
	
	
	private float _speed;
	private bool _didDamage;

	void Start () {
		AudioSource.PlayClipAtPoint(Sound, transform.position);
		_speed = 10f;
		_didDamage = false;
	}
	
	void Update () {
		//Old way of moving skills
		//rigidbody.AddForce(Vector3.up*speed);
		transform.Translate(-Vector3.up * speed * Time.deltaTime);
	}
	
	void OnTriggerEnter (Collider other)
	{
		if(other.gameObject.CompareTag ("Enemy") && !_didDamage)
		{
			_didDamage = true;
			other.gameObject.SendMessage ("TakeDamage", damageDealt);
			if(HitFX != null) { Instantiate(HitFX, other.transform.position, Quaternion.identity); }
		}
	}
}