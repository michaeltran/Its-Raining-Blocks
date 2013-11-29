﻿using UnityEngine;
using System.Collections;

public class QuestionBlock : MonoBehaviour
{
	public tk2dSpriteAnimator sprite;
	public GameObject healthPotion;
	public GameObject manaPotion;
	private bool _bumped = false;
	
	void Update ()
	{
		CheckBump ();	
	}
	
	void CheckBump ()
	{
		if (_bumped == false) {
			RaycastHit[] hits = null;
			hits = Physics.RaycastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), -transform.up, 1.3f);
			if (hits.Length > 0 && hits [0].collider.tag == "PlayerCollider") { 
				_bumped = true;
				// REMEMBER TO CHECK GROUNDED LATA
				// DISABLE GRAVITY HERE
				//sprite.Play ("question_block_bump");

				Invoke ("DestroyObject", 0.3f);
			}
		}
	}
	
	void DestroyObject ()
	{
		// hp = 0-50
		// mp = 51-100
		int random = Random.Range (0, 100);
		
		Vector3 pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		
		if (random <= 50) {
			Instantiate (healthPotion, pos, Quaternion.identity);
		} else if (random > 50) {
			Instantiate (manaPotion, pos, Quaternion.identity);
		}
		Vector3 target = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 20);
		transform.position = target;
		Destroy (this.gameObject);
	}
}
