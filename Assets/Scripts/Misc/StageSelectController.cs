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
	public MadSprite nextWorld;
	public MadSprite previousWorld;
	public GameObject[] stages;
	public GameObject[] stagesLabel;
	private MadText _ASPText;

	void Start() {
		_ASPText = skillTreeButton.transform.FindChild("ASP/text").GetComponent<MadText>();
		backToMenuButton.onMouseDown += backToMenuButton.onTap = (sprite) => {MadLevel.LoadFirst();};
		skillTreeButton.onMouseDown += skillTreeButton.onTap = (sprite) => {MadLevel.LoadLevelByName ("Skill Tree");};
		nextWorld.onMouseDown += nextWorld.onTap = (sprite) => {setLaboratoryWorld();};
		previousWorld.onMouseDown += previousWorld.onTap = (sprite) => {setHauntedHouseWorld();};
		_ASPText.text = PlayerData.Instance.data.talentPoints.ToString();
	}

	void setHauntedHouseWorld() {
		stages[0].SetActive(true);
		stagesLabel[0].SetActive(true);
		stages[1].SetActive(false);
		stagesLabel[1].SetActive(false);
	}

	void setLaboratoryWorld() {
		stages[0].SetActive(false);
		stagesLabel[0].SetActive(false);
		stages[1].SetActive(true);
		stagesLabel[1].SetActive(true);
	}
}

#if !UNITY_3_5
//} // namespace
#endif
