using UnityEngine;
using System.Collections;
using MadLevelManager;

public class ReturnToMenu : MonoBehaviour {

	void OnClick()
	{
		MadLevel.LoadFirst();
	}
}
