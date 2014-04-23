using UnityEngine;
using System.Collections;

public class EnemyThrowProjectile : MonoBehaviour {
	public tk2dSpriteAnimator spriteAnimator;
	public GameObject projectile;
	
	void Start() {
		InvokeRepeating ("SpawnProjectile", 5f, 5f);
	}
	
	void SpawnProjectile() {
		if(spriteAnimator != null) {
			spriteAnimator.Play ("Attack");
		}
		Vector3 pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z-5);
		Instantiate(projectile, pos, Quaternion.identity);
	}
}
