using UnityEngine;
using System.Collections;

public class DeleteBomb : MonoBehaviour
{
	public float timedelay = 1f;
	
	void Start ()
	{
		Invoke ("deleteBomb", timedelay);
	}
	
	void deleteBomb ()
	{
		Destroy (this.gameObject);	
	}
}
