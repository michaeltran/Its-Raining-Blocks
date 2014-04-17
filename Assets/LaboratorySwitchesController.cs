using UnityEngine;
using System.Collections;

public class LaboratorySwitchesController : MonoBehaviour {
	public LaboratorySwitchController[] switches;
	public AbnormalEffects abnormalEffects;

	void Update() {
		checkIfAllSwitchesPulled();
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
		} else {
			abnormalEffects.setInvincibility(true);
		}
	}
}
