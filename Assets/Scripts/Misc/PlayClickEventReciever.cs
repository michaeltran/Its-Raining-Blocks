using UnityEngine;
using System.Collections;
using MadLevelManager;

public class PlayClickEventReciever : MonoBehaviour {

	void OnClick()
	{
		MadLevel.LoadNext(MadLevel.Type.Other);
	}
}
