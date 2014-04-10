using UnityEngine;
using System.Collections;
using MadLevelManager;

public class ChangeStageColor : MonoBehaviour {
	public Color color;
	void Start () {
		MadLevelIcon madLevelIcon = this.transform.parent.GetComponent<MadLevelIcon>();
		if(madLevelIcon == null)	
			Debug.Log ("Unable to find MadLevelIcon");
		MadLevelProperty madLevelProperty = GetComponent<MadLevelProperty>();
		if(madLevelProperty._propertyEnabled == true)
			madLevelIcon.tint = color;
	}
}
