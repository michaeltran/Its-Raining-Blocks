using UnityEngine;
using System.Collections;

public class TestEventReciever : MonoBehaviour {
	public GameObject test;
	private SkillTemplateController skillTemplateController;
	public int skillId = 0;
	private TalentTree talentTree;

	void Start() {
		talentTree = GameObject.Find("Controller").GetComponent<TalentTree>();
	}

	void Update() {
		//TODO: NOT EFFICIENT DAWG
		if(talentTree.checkPreRequisites(skillId) == true)
		{
			UISprite uiSprite = transform.FindChild("Background").GetComponent<UISprite>();
			uiSprite.color = Color.white;
		}
	}

	void OnClick() {
		test.SetActive(true);

		skillTemplateController = test.transform.FindChild("SkillTemplate").GetComponent<SkillTemplateController>();
		skillTemplateController.initializeStuff(skillId);
	}
}
