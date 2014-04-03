using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MadLevelManager;

#if !UNITY_3_5
//namespace MadLevelManager {
#endif

public class StageSelectController : MonoBehaviour {
	public MadSprite backToMenuButton;
	public MadSprite skillTreeButton;
	private MadText _ASPText;

	void Start() {
		_ASPText = skillTreeButton.transform.FindChild("ASP/text").GetComponent<MadText>();
		backToMenuButton.onMouseDown += backToMenuButton.onTap = (sprite) => {MadLevel.LoadFirst();};
		skillTreeButton.onMouseDown += skillTreeButton.onTap = (sprite) => {MadLevel.LoadLevelByName ("Skill Tree");};

		_ASPText.text = PlayerData.Instance.data.talentPoints.ToString();
	}
}

#if !UNITY_3_5
//} // namespace
#endif
