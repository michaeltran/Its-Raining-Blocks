using UnityEngine;
using System.Collections;

public class TrollTesla : MonoBehaviour {
	public float maxSpeed = 30f;
	public float changeTime = 1f;
	private hoMove homove;

	void Start () {
		homove = GetComponent<hoMove>();
		InvokeRepeating("trollSpeed", 0f, changeTime);
	}

	void trollSpeed() {
		if(homove != null) {
			float random = Random.Range(1f, maxSpeed);
			//homove.speed = random;
			homove.ChangeSpeed(random);
		}
	}
}
