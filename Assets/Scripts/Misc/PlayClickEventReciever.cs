using UnityEngine;
using System.Collections;

public class PlayClickEventReciever : MonoBehaviour {

	void OnClick()
	{
		Application.LoadLevel ("HauntedHouse");
	}
}
