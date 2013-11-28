using UnityEngine;
using System.Collections;

public class AbnormalEffects : MonoBehaviour {

	private bool _isFrozen = false;
	private hoMove _hM;
	
	void Start() {
		//RaycastCharacterController rcc = (RaycastCharacterController)this.gameObject.GetComponent ("RaycastCharacterController");
		_hM = (hoMove)this.gameObject.GetComponent ("hoMove");
	}
	
	public void Freeze() {
		_hM.Pause();
		_isFrozen = true;
		Invoke("Unfreeze", 5f);
	}
	
	void Unfreeze() {
		_hM.Resume();
		_isFrozen = false;
	}
}
