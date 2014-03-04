using UnityEngine;
using System.Collections;

public class LearnEventReciever : MonoBehaviour {
	public string currentId = "";
	private TalentTree _talentTree;
	private UIButton _uiButton;

	void Start() {
		_uiButton = this.gameObject.GetComponent<UIButton>();
		_talentTree = GameObject.Find("Controller").GetComponent<TalentTree>();
		if(_talentTree == null)
			Debug.Log("Controller-TalentTree not found.");
	}

	void Update() {
		if(_talentTree.isLearnt(currentId) == true 
		   && _uiButton.isEnabled == true)
			_uiButton.isEnabled = false;
		else if (_talentTree.isLearnt(currentId) == false 
		         && _talentTree.checkPreRequisites(currentId) == true 
		         && _uiButton.isEnabled == false)
			_uiButton.isEnabled = true;
		else if (_talentTree.isLearnt(currentId) == false 
		         && _talentTree.checkPreRequisites(currentId) == false 
		         && _uiButton.isEnabled == true)
			_uiButton.isEnabled = false;
	}

	void OnClick() {
		_talentTree.learnTalent(currentId);
	}
}
