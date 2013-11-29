using UnityEngine;
using System.Collections;

public class AbnormalEffects : MonoBehaviour {

	private bool _isFrozen = false;
	private GameObject _frozenAura;
	private hoMove _hM;
	
	void Start() {
		_hM = (hoMove)this.gameObject.GetComponent ("hoMove");
		_frozenAura = transform.FindChild("CFX_FrozenAura").gameObject;
	}
	
	public void Freeze() {
		_hM.Pause();
		_isFrozen = true;
		_frozenAura.SetActive(true);
		Invoke("Unfreeze", 5f);
	}
	
	void Unfreeze() {
		_hM.Resume();
		_isFrozen = false;
		_frozenAura.SetActive(false);
	}
}
