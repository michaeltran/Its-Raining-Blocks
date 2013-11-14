using UnityEngine;
using System.Collections;

public class BombAreaDamage : MonoBehaviour {


	public int damage=0;
	private CharacterController cont;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Destructable"))
		{
			Destroy(other);
		}
		if (other.gameObject.CompareTag ("Player"))
		{
			other.gameObject.SendMessage("TakeDamage",damage);
		}
		
		
	}
}
