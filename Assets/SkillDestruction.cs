using UnityEngine;
using System.Collections;

public class SkillDestruction : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Destructable"))
		{
			other.gameObject.SendMessage ("PlaySkillDestructionFX");
		}
	}
}
