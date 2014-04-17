using UnityEngine;
using System.Collections;

public class AbnormalEffects : MonoBehaviour
{
	public bool shieldBlockExists = false;
	private bool _isFrozen = false;
	private GameObject _invincibilityAura;
	private GameObject _frozenAura;
	private EnemyStatus _enemyStatus;
	private hoMove _hM;
	
	void Start ()
	{
		_hM = GetComponent<hoMove>();
		_enemyStatus = GetComponent<EnemyStatus>();
		_invincibilityAura = transform.FindChild ("CFX_EnemyInvincibilityAura").gameObject;
		_frozenAura = transform.FindChild ("CFX_FrozenAura").gameObject;
	}
	
	void Update()
	{
		DetectEnemyShieldBlock ();
	}
	
	public void Freeze ()
	{
		_hM.Pause ();
		_isFrozen = true;
		_frozenAura.SetActive (true);
		Invoke ("Unfreeze", 5f);
	}
	
	void Unfreeze ()
	{
		_hM.Resume ();
		_isFrozen = false;
		_frozenAura.SetActive (false);
	}
	
	void DetectEnemyShieldBlock ()
	{
		if(shieldBlockExists == true) {
			if(GameObject.Find("EnemyShieldBlock") != null || GameObject.Find ("EnemyShieldBlock(Clone)") != null) {
				setInvincibility(true);
			}	
			else {
				setInvincibility(false);
			}
		}
	}

	public void setInvincibility(bool value) {
		_enemyStatus._Invincible = value;
		if(_invincibilityAura != null)
			_invincibilityAura.SetActive(value);
	}
}
