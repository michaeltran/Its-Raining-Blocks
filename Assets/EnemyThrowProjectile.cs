using UnityEngine;
using System.Collections;

public class EnemyThrowProjectile : MonoBehaviour {

	public GameObject projectile;
	
	void Start() {
		InvokeRepeating ("SpawnProjectile", 5f, 5f);
	}
	
	void SpawnProjectile() {
		Vector3 pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z-5);
		Instantiate(projectile, pos, Quaternion.identity);
	}
}
