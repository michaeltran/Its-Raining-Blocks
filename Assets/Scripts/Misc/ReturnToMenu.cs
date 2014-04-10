using UnityEngine;
using System.Collections;
using MadLevelManager;

public class ReturnToMenu : MonoBehaviour {

	void OnClick()
	{
		GameObject backgroundMusic = GameObject.Find("Background Music");
		if(backgroundMusic != null) {
			Destroy(backgroundMusic);
		}
		MadLevel.LoadFirst();
	}
}
