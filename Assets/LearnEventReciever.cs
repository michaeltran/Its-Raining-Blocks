using UnityEngine;
using System.Collections;

public class LearnEventReciever : MonoBehaviour {
	public int currentId = 0;
	private TalentTree talentTree;

	void Start() {
		talentTree = GameObject.Find("Controller").GetComponent<TalentTree>();
		if(talentTree == null)
			Debug.Log("Unable to find TalentTree in Controller");
	}

	void OnClick() {
		talentTree.getTalent(currentId);
	}
}
