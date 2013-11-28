using UnityEngine;
using System.Collections;

public class CheckAreaDamage : MonoBehaviour
{
	private bool _didDamage = false;
	private float _damageTime = 0.01f;
	
	void Start()
	{
		Invoke ("SetDidDamageTrue", _damageTime);
	}
	
	public bool GetDidDamage ()
	{
		if (_didDamage == false) {
			_didDamage = true;
			return false;	
		}
		return _didDamage;
	}
	
	public bool _GetDidDamage()
	{
		return _didDamage;
	}
	
	void SetDidDamageTrue()
	{
		_didDamage = true;	
	}
}
