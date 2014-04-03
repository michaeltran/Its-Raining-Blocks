using UnityEngine;
using System.Collections;
using MadLevelManager;

public class ScoreController : MonoBehaviour {
	public string stageName = "Haunted House";
	private string levelName;
	private float startTime = 0f;
	private float currentTime = 0f;
	private float endTime = 0f;
	private bool trackScore = true;

	void Start() {
		levelName = MadLevel.currentLevelName;
		startTime = Time.time;
		DontDestroyOnLoad(transform.gameObject);
		Debug.Log(startTime);
	}

	void Update() {
		if(trackScore == true) {
			currentTime = Time.time;
		}
	}

	public float calculateScore() {
		float theScore = 0f;
		theScore += 1000;
		theScore -= (currentTime - startTime)*10;
		return theScore;
	}

	public float getTimeElapsed() {
		return (currentTime - startTime);
	}

	public string getLevelName() {
		return levelName;
	}
}
