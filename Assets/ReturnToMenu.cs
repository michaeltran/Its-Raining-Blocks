using UnityEngine;
using System.Collections;

public class ReturnToMenu : MonoBehaviour {

	void OnClick()
	{
		Application.LoadLevel ("MainMenu");
	}
}
