using UnityEngine;
using System.Collections;
using MadLevelManager;

public class ResultsController : MonoBehaviour {
	public UILabel uiLabel;
	private ScoreController scoreController;
	private bool completed = false;
	private UISprite star1;
	private UISprite star2;
	private UISprite star3;
	private int _SPEarned = 0;

	void Start() {
		star1 = GameObject.Find("Star1").GetComponent<UISprite>();
		star2 = GameObject.Find("Star2").GetComponent<UISprite>();
		star3 = GameObject.Find("Star3").GetComponent<UISprite>();
		GameObject temp = GameObject.Find("ScoreController");
		if(temp == null) {
			Debug.Log ("Unable to find ScoreController GameObject.");
			this.gameObject.SetActive(false);
		}
		scoreController = temp.GetComponent<ScoreController>();
		setStars();
		setText ();
		increaseSP();
		Destroy(temp);
	}

	void setText() {
		string theString = "";
		theString += "You have successfully beaten the [99FF00]" + scoreController.stageName + "[-] stage!\n\n";
		theString += "Time Elapsed: " + (scoreController.getTimeElapsed()).ToString ("F2") + "\n\n";
		theString += "\n\n";
		theString += "Score: " + (scoreController.calculateScore()).ToString("F0") +  "\n\n";
		theString += "Skill points earned: " + _SPEarned + "\n\n";

		uiLabel.text = theString;
	}

	void increaseSP() {
		PlayerData.Instance.data.talentPoints += _SPEarned;
		PlayerData.Instance.Save();
	}

	void setStars() {
		_EarnStar("star_1");
		star1.spriteName = "star1";
		MarkLevelCompleted();

		if(scoreController.calculateScore() >= 200) {
			_EarnStar("star_2");
			star2.spriteName = "star1";
			MarkLevelCompleted();
		}

		if(scoreController.calculateScore() >= 600) {
			_EarnStar("star_3");
			star3.spriteName = "star1";
			MarkLevelCompleted();
		}	
	}

	void _EarnStar(string name) {
		if(MadLevelProfile.GetLevelBoolean (scoreController.getLevelName(), name) == false)
		{
			_SPEarned++;
		}
		EarnStar(name);
	}

	void EarnStar(string name) {
		// set level boolean sets level property of type boolean
		MadLevelProfile.SetLevelBoolean(scoreController.getLevelName(), name, true);
	}
	
	void MarkLevelCompleted() {
		// sets the level completed
		// by default next level will be unlocked
		MadLevelProfile.SetCompleted(scoreController.getLevelName(), true);
		
		// manual unlocking looks like this:
		// MadLevelProfile.SetLocked("level_name", false);
		
		// remembering that level is completed
		completed = true;
	}
}
