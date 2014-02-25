using UnityEngine;
using System.Collections;
using MadLevelManager;

public class SkillTree_BackButtonEventReciever : MonoBehaviour {

	void OnClick()
	{
		MadLevel.LoadPrevious (MadLevel.Type.Other);
	}
}
