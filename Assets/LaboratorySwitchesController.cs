using UnityEngine;
using System.Collections;

public class LaboratorySwitchesController : MonoBehaviour {
	public LaboratorySwitchController[] switches;
	public AbnormalEffects abnormalEffects;

	private bool startTimer = false;
	private float startTime;
	private float timeToReset = 30f;

	void Update() {
		checkIfAllSwitchesPulled();
		if(startTimer == true)
			timerToReset ();
	}

	void timerToReset() {
		if(((Time.time - startTime) >= timeToReset) == true) {
			endTimer();
		}
	}

	void checkIfAllSwitchesPulled() {
		int numberOff = 0;
		foreach(LaboratorySwitchController _switch in switches) {
			if(_switch.isOff == true) {
				numberOff++;
			}
		}
		if(numberOff >= switches.Length) {
			abnormalEffects.setInvincibility(false);
			if(startTimer == false) {
				setUpTimer();
			}
		} else {
			abnormalEffects.setInvincibility(true);
		}
	}

	void setUpTimer() {
		startTimer = true;
		startTime = Time.time;
		foreach(LaboratorySwitchController _switch in switches) {
			_switch.parentPipe.setUp (Color.red, Color.green, timeToReset);
			_switch.parentPipe.dontCheckPulled = true;
		}
	}

	void endTimer() {
		startTimer = false;
		turnAllSwitchesOn();
	}

	void turnAllSwitchesOn() {
		foreach(LaboratorySwitchController _switch in switches) {
			_switch.changeSprite(_switch._onID);
			_switch.isDisabled = false;
		}
		foreach(LaboratorySwitchController _switch in switches) {
			_switch.parentPipe.dontCheckPulled = false;
			_switch.parentPipe.enableTesla();
		}
	}
}
