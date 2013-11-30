using UnityEngine;
using System.Collections;

public abstract class AbstractStatus : MonoBehaviour
{
	protected float _currentHP;
	protected float _maxHP;
	protected float _HPRegeneration;
	protected float _currentMP;
	protected float _maxMP;
	protected float _MPRegeneration;
	protected bool _isDead = false;
	protected GameObject _vitalBar;
	protected GameObject _manaBar;
	protected GameObject _gameOver;
	protected VitalBarBasic _vitalBarBasic;
	protected ManaBarBasic _manaBarBasic;
	
	protected void HPRegeneration ()
	{
		if (_currentHP < _maxHP) {
			_currentHP += _HPRegeneration * Time.deltaTime;
		}
	}
	
	protected void MPRegeneration ()
	{
		if (_currentMP < _maxMP) {
			_currentMP += _MPRegeneration * Time.deltaTime;
		}
	}
	
	protected void CalculateVitalBar ()
	{
		float x = (float)_currentHP / (float)_maxHP;
		string str = (int)_currentHP + "/" + _maxHP;
		
		_vitalBarBasic.UpdateDisplay (x, str);
	}
	
	protected void CalculateManaBar ()
	{
		float x = (float)_currentMP / (float)_maxMP;
		string str = (int)_currentMP + "/" + _maxMP;
		
		_manaBarBasic.UpdateDisplay (x, str);
	}
}
