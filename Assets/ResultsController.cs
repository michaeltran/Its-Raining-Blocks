using UnityEngine;
using System.Collections;
using MadLevelManager;

public class ResultsController : MonoBehaviour {
	public UILabel uiLabel;
	private ScoreController scoreController;
	private bool completed = false;

	void Start() {
		GameObject temp = GameObject.Find("ScoreController");
		if(temp == null) {
			Debug.Log ("Unable to find ScoreController GameObject.");
			this.gameObject.SetActive(false);
		}
		scoreController = temp.GetComponent<ScoreController>();
		setText ();
		setStars();
		Destroy(temp);
	}

	void setText() {
		string theString = "";
		theString += "You have successfully beaten the [99FF00]" + scoreController.stageName + "[-] stage!\n\n";
		theString += "Time Elapsed: " + (scoreController.getTimeElapsed()).ToString ("F2") + "\n\n";
		theString += "\n\n";
		theString += "Score: " + (scoreController.calculateScore()).ToString("F0") +  "\n\n";

		uiLabel.text = theString;
	}

	void setStars() {
		if(scoreController.calculateScore() >= 100) {
			EarnStar("star_1");
			MarkLevelCompleted();
		}

		if(scoreController.calculateScore() >= 200) {
			EarnStar("star_2");
			MarkLevelCompleted();
		}

		if(scoreController.calculateScore() >= 300) {
			EarnStar("star_3");
			MarkLevelCompleted();
		}	}

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
