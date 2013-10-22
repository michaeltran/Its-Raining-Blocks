using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour
{
	public float resetAfterDeathTime = 2.5f;
	public float transitionToResultTime = 2.5f;
	private Vector3 originalPosition;
	private Vector3 hidingPosition;
	private GameObject globalSpawner;
	
	void Start ()
	{
		globalSpawner = GameObject.FindGameObjectWithTag ("GlobalSpawner");
		originalPosition = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
		hidingPosition = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z + 20);
		this.gameObject.transform.position = hidingPosition;
	}
	
	public void LevelReset ()
	{
		this.gameObject.transform.position = originalPosition;
		globalSpawner.gameObject.SendMessage ("setSpawnStuff", false);
		Invoke ("DoReset", resetAfterDeathTime);
	}
	
	void DoReset()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
	
	public void LevelWin ()
	{
		globalSpawner.gameObject.SendMessage ("setSpawnStuff", false);
		Invoke ("DoWin", transitionToResultTime);
	}
	
	void DoWin()
	{
		Application.LoadLevel ("Results");
	}
	
}
