using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public float speed = 10f;
	private float _speed;

	// Use this for initialization
	void Start () {
		_speed = 10f;
	}
	
	// Update is called once per frame
	void Update () {
		rigidbody.AddForce(Vector3.up*speed);
	}
}
