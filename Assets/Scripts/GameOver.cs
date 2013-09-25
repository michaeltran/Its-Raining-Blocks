using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour
{
	private float resetAfterDeathTime = 2.5f;
	private Vector3 originalPosition;
	private Vector3 hidingPosition;
	
	// Use this for initialization
	void Start ()
	{
		//this.gameObject.SetActive(false);
		originalPosition = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
		hidingPosition = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z + 20);
		this.gameObject.transform.position = hidingPosition;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public void LevelReset ()
	{
		//this.gameObject.SetActive(true);
		this.gameObject.transform.position = originalPosition;
		Invoke ("DoReset", resetAfterDeathTime);
	}
	
	void DoReset()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
	
}
