using UnityEngine;
using System.Collections;

public class ExplosionSound : MonoBehaviour 
{
	
	public AudioClip Sound;
	public float Delay=0f;
	
	// Use this for initialization
	void Start () 
	{
		Invoke ("InvokeExplosionSound", Delay);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	void InvokeExplosionSound()
	{
		AudioSource.PlayClipAtPoint(Sound, transform.position);		
	}
}
