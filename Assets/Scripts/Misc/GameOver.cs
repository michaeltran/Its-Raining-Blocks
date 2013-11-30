using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour
{
	public float resetAfterDeathTime = 5f;
	public float transitionToResultTime = 5f;
	private Vector3 _originalPosition;
	private Vector3 _hidingPosition;
	private GameObject _globalSpawner;
	private tk2dTextMesh textMesh;
	
	void Start ()
	{
		_globalSpawner = GameObject.FindGameObjectWithTag ("GlobalSpawner");
		textMesh = GetComponent<tk2dTextMesh>();
		textMesh.text = "";
	}
	
	public void LevelReset ()
	{
		textMesh.text = "Game Over Man, Game Over";
		_globalSpawner.gameObject.SendMessage ("setSpawnStuff", false);
		Invoke ("DoReset", resetAfterDeathTime);
	}
	
	void DoReset()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
	
	public void LevelWin ()
	{
		textMesh.text = "Victory is Ours";
		_globalSpawner.gameObject.SendMessage ("setSpawnStuff", false);
		Invoke ("DoWin", transitionToResultTime);
	}
	
	void DoWin()
	{
		Application.LoadLevel ("Results");
	}
	
}
