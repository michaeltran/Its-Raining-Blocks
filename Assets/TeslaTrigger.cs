using UnityEngine;
using System.Collections;

public class TeslaTrigger : MonoBehaviour {
	public AudioClip HitSound;
	public ParticleSystem hitFX;
	private bool _didDamage = false;
	private float _maxTime = 3f;
	private float _startTime = 0f;
	private float _currentTime = 0f;

	void Start() {
		_startTime = Time.time;
	}

	void Update() {
		_currentTime = Time.time;
		if((_currentTime - _startTime) > _maxTime) {
			_startTime = Time.time;
			_didDamage = false;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if(other.gameObject.tag == "PlayerCollider" && _didDamage == false)
		{
			_didDamage = true;
			if(hitFX != null)
				Instantiate (hitFX, other.transform.position, Quaternion.identity);
			if(HitSound != null)
				AudioSource.PlayClipAtPoint(HitSound, transform.position);
			GameObject target = other.transform.parent.gameObject;
			target.gameObject.SendMessage ("TakeDamage", 15);
		}
	}
}
