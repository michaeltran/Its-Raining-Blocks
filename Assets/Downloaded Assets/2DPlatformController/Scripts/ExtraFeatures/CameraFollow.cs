using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public bool oneDirectionOnly;

	void Update () {
		float delta = target.position.x - transform.position.x;
		if (!oneDirectionOnly || delta > 0.0f) {
			transform.Translate(delta * Time.deltaTime, 0.0f, 0.0f);
		}
	}
}
