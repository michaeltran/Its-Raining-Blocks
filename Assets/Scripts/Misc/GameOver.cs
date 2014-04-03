using UnityEngine;
using System.Collections;
using System.Linq;
using MadLevelManager;

public class GameOver : MonoBehaviour
{
	public float resetAfterDeathTime = 5f;
	public float transitionToResultTime = 5f;
	public tk2dSpriteAnimator playerSprite;
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

	void purgeScene () 
	{
		GameObject[] destructableArray = new GameObject[0];
		GameObject[] explosiveArray = new GameObject[0];
		destructableArray = GameObject.FindGameObjectsWithTag("Destructable");
		explosiveArray = GameObject.FindGameObjectsWithTag("Explosive");

		foreach(GameObject obj in destructableArray) {
			Vector3 target = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z + 20);
			obj.transform.position = target;
			GarbageCollection gc = (GarbageCollection)obj.GetComponent (typeof(GarbageCollection));
			gc.DestroyObject ();
		}

		foreach(GameObject obj in explosiveArray) {
			Destroy(obj);
		}
	}

	void setDisableControls() 
	{
		SimpleCharacterInput sci = GameObject.FindGameObjectWithTag("Player").GetComponent<SimpleCharacterInput>();
		sci.disableControls = true;
	}
	
	void DoReset()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
	
	public void LevelWin ()
	{
		textMesh.text = "Victory is Ours";
		_globalSpawner.gameObject.SendMessage ("setSpawnStuff", false);

		purgeScene();
		setDisableControls();
		PlayerAnimator playerAnimator = playerSprite.gameObject.GetComponent<PlayerAnimator>();
		playerAnimator.enabled = false;
		playerSprite.Play ("victory-pose");


		Invoke ("DoWin", transitionToResultTime);
	}
	
	void DoWin()
	{
		MadLevel.LoadLevelByName("Results");
	}
	
}
