using UnityEngine;
using System.Collections;

public class PlayClickEventReciever : MonoBehaviour {

	void OnClick()
	{
		Debug.Log ("Play Clicked.");
		Application.LoadLevel ("HauntedHouse");
	}
}
