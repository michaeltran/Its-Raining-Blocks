using UnityEngine;
using System.Collections;
using MadLevelManager;

public class CreditEventReciever : MonoBehaviour {

	void OnClick ()
	{
		MadLevel.LoadLevelByName("Credits");
	}
}
