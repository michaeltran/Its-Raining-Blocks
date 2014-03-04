using UnityEngine;
using System.Collections;

public class SkillEventReciever : MonoBehaviour {
	public string skillId = "0";
	public GameObject skillDescriptionPanel;

	private Color _enabledColor = Color.white;
	private Color _disabledColor = Color.grey;
	private GameObject _backButton;
	private UIButton _uiButton;
	private UISprite _checkMark;
	private int _originalDepth = 0;
	private SkillTemplateController _skillTemplateController;
	private SkillTreePanelController _skillTreePanelController;
	private TalentTree _talentTree;

	void Start() {
		_uiButton = transform.GetComponent<UIButton>();
		_checkMark = transform.FindChild("Check").GetComponent<UISprite>();
		_originalDepth = _checkMark.depth;
		_skillTreePanelController = GameObject.FindGameObjectWithTag("SkillTreePanel").GetComponent<SkillTreePanelController>();
		_talentTree = GameObject.Find("Controller").GetComponent<TalentTree>();
		if(_talentTree == null)
			Debug.Log("Controller/TalentTree not found!");
		_backButton = GameObject.Find("SceneBackButton");
		if(_backButton == null)
			Debug.Log("SceneBackButton not found!");
	}

	void Update() {
		if(_talentTree.checkPreRequisites(skillId) == true && _uiButton.defaultColor != _enabledColor) {
			_uiButton.defaultColor = _enabledColor;
			_uiButton.UpdateColor(true,true);
		} else if(_talentTree.checkPreRequisites(skillId) == false && _uiButton.defaultColor != _disabledColor) {
			_uiButton.defaultColor = _disabledColor;
			_uiButton.UpdateColor(true,true);
		}
		if(_talentTree.isLearnt(skillId) == true && _checkMark.depth != 10) {
			_checkMark.depth = 10;
		}
		else if(_talentTree.isLearnt(skillId) == false && _checkMark.depth != _originalDepth){
			_checkMark.depth = _originalDepth;
		}
	}

	void OnClick() {
		skillDescriptionPanel.SetActive(true);
		_skillTreePanelController.setActivePanel(false);

		_skillTemplateController = skillDescriptionPanel.transform.FindChild("SkillTemplate").GetComponent<SkillTemplateController>();
		_skillTemplateController.initializeStuff(skillId);
	}
}
