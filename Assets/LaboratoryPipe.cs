using UnityEngine;
using System.Collections;

public class LaboratoryPipe : MonoBehaviour {
	public LaboratorySwitchController[] switches;
	public GameObject[] tesla;
	private tk2dSprite sprite;
	private float speed = 0.1f;
	private Color currentColor = Color.green;
	private Color targetColor = Color.red;
	private bool change = false;

	void Start () {
		sprite = GetComponent<tk2dSprite>();
	}

	void Update () {
		checkIfSwitchesPulled();
		if(change == true)
			sprite.color = Color.Lerp (sprite.color, targetColor, Time.deltaTime*speed);
	}

	public void setUp(Color start, Color end, float time) {
		currentColor = start;
		sprite.color = currentColor;
		targetColor = end;
		speed = 1f/time;
		change = true;
	}

	void checkIfSwitchesPulled() {
		int numberOff = 0;
		foreach(LaboratorySwitchController _switch in switches) {
			if(_switch.isOff == true) {
				numberOff++;
			}
		}
		if(numberOff >= switches.Length) {
			disableSwitches();
		}
	}

	void disableSwitches() {
		foreach(LaboratorySwitchController _switch in switches) {
			_switch.setDisabled(true);
		}
		sprite.color = currentColor;
		targetColor = currentColor;
		foreach(GameObject obj in tesla) {
			obj.SetActive(false);
		}
	}
}
