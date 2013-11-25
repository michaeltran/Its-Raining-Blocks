using UnityEngine;
using System.Collections;

public class DeleteBomb : MonoBehaviour {

	public float timedelay=1f;
	// Use this for initialization
	void Start () {
		Invoke ("deleteBomb", timedelay);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void deleteBomb()
	{
		Destroy (this.gameObject);	
	}
}
