﻿using UnityEngine;
using System.Collections;

public class EnemyProjectile : MonoBehaviour {

	public float speed;
	public AudioClip HitSound;
	
	private GameObject _target;
	private Transform Target;
	private Vector3 targetDir;
	private bool _didDamage = false;
	
	void Start() {
		_target = GameObject.FindGameObjectWithTag("Player");
		if(_target != null)
		{
			Target = _target.gameObject.transform;
			targetDir = Target.position - transform.position;
			transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2 (targetDir.y, targetDir.x) * Mathf.Rad2Deg + 90);
		}
	}
	
	void Update() {
		float step = speed*Time.deltaTime;
		transform.Translate(-Vector3.up * speed * Time.deltaTime);
	}
	
    void OnTriggerEnter (Collider other)
    {
        // If the colliding gameobject is the player...
        if(other.gameObject.tag == "PlayerCollider" && _didDamage == false)
        {
			_didDamage = true;
			AudioSource.PlayClipAtPoint(HitSound, transform.position);
			GameObject target = other.transform.parent.gameObject;
			target.gameObject.SendMessage ("TakeDamage", 15);
        }
    }
}
