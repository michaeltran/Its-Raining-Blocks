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

	void Start() {
		backToMenuButton.onMouseDown += backToMenuButton.onTap = (sprite) => {MadLevel.LoadFirst();};
		skillTreeButton.onMouseDown += skillTreeButton.onTap = (sprite) => {MadLevel.LoadLevelByName ("Skill Tree");};
	}
}

#if !UNITY_3_5
//} // namespace
#endif
