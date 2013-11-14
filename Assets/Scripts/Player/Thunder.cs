using UnityEngine;
using System.Collections;

public class Thunder : MonoBehaviour {
	
	public ParticleSystem Poof;
	
	private bool _didDamage;
	
	// Use this for initialization
	void Start () {
		_didDamage = false;
	}
	
	void OnTriggerEnter (Collider other)
	{
		Debug.Log ("Entered: " + other.gameObject.tag);
		if(other.gameObject.CompareTag ("Enemy") && !_didDamage)
		{
			Debug.Log ("entered2");
			_didDamage = true;
			other.gameObject.SendMessage ("TakeDamage", 50);
			Instantiate(Poof, this.gameObject.transform.position, Quaternion.identity);
			Destroy(this.gameObject);
		}
	}
}
