using UnityEngine;
using System.Collections;

public class LaboratorySwitchController : MonoBehaviour {
	public float timeToReset = 2f;
	public LaboratoryPipe parentPipe;
	private tk2dSprite _sprite;
	private float startTime = 0f;
	private float currentTime = 0f;
	[HideInInspector]public int _offID;
	[HideInInspector]public int _onID;
	[HideInInspector]public bool isOff = false;
	[HideInInspector]public bool isDisabled = false;

	public void setDisabled(bool set) {
		isDisabled = set;
		endTimer ();
	}

	void Start() {
		_sprite = GetComponent<tk2dSprite>();
		_offID = _sprite.GetSpriteIdByName("off");
		_onID = _sprite.GetSpriteIdByName("on");
	}

	void Update() {
		if(isDisabled == false && isOff == true) {
			currentTime = Time.time;
			if((currentTime - startTime) > timeToReset) {
				changeSprite (_onID);
				endTimer ();
				isOff = false;
			}
		}
	}

	void OnTriggerEnter(Collider collider) {
		if(isDisabled == false && collider.tag == "PlayerCollider") {
			if(_sprite.spriteId == _onID) {
				changeSprite(_offID);
				startTimer ();
				isOff = true;
			}
		}
	}

	public void changeSprite(int id) {
		_sprite.spriteId = id;
	}

	void startTimer() {
		startTime = Time.time;
		currentTime = Time.time;
		parentPipe.setUp (Color.red, parentPipe.originalColor, timeToReset);
	}

	void endTimer() {
		startTime = 0f;
		currentTime = 0f;
		//parentPipe.setUp (Color.green,Color.red,timeToReset);
	}
}
