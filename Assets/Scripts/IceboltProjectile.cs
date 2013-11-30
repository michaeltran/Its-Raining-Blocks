using UnityEngine;
using System.Collections;

public class IceboltProjectile : MonoBehaviour
{
	public AudioClip Sound;
	public float speed = 10f;
	public int damageDealt = 10;
	public ParticleSystem HitFX;
	private float _speed;
	private bool _didDamage;

	void Start ()
	{
		if (Sound != null) {
			AudioSource.PlayClipAtPoint (Sound, transform.position);
		}
		_speed = 10f;
		_didDamage = false;
	}
	
	void Update ()
	{
		transform.Translate (-Vector3.up * speed * Time.deltaTime);
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.CompareTag ("Enemy") && !_didDamage) {
			_didDamage = true;
			other.gameObject.SendMessage ("Freeze");
			if (HitFX != null) {
				Instantiate (HitFX, other.transform.position, Quaternion.identity);
			}
		}
	}
}
