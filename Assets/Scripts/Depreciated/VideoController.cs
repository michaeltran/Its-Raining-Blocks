using UnityEngine;
using System.Collections;

public class VideoController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MovieTexture movie = renderer.material.mainTexture as MovieTexture;
		movie.Play ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
