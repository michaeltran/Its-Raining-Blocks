using UnityEngine;
using System.Collections;

public class SkillTreePanelController : MonoBehaviour {
	public void setActivePanel(bool active) {
		Collider[] colliders = GetComponentsInChildren<Collider>();
		foreach(Collider col in colliders)
			col.enabled = active;

		UIButton[] uiButtons = GetComponentsInChildren<UIButton>();
		foreach(UIButton uiButton in uiButtons)
			uiButton.UpdateColor(active,true);
	}
}
