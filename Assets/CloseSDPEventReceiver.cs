using UnityEngine;
using System.Collections;

public class CloseSDPEventReceiver : MonoBehaviour {
	private GameObject _backButton;
	private SkillTreePanelController _skillTreePanelController;

	void Start() {
		_backButton = GameObject.Find("SceneBackButton");
		_skillTreePanelController = GameObject.FindGameObjectWithTag("SkillTreePanel").GetComponent<SkillTreePanelController>();
	}

	void OnClick() {
		_skillTreePanelController.setActivePanel(true);
		//TODO: Find a better way to call this, lol
		this.transform.parent.transform.parent.gameObject.SetActive(false);
	}
}
