using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour
{
	public AudioClip PlayerDeathSound;
	public bool GodMode = false;
	private float _currentHP;
	private float _maxHP;
	private float _HPRegeneration;
	private float _currentMP;
	private float _maxMP;
	private float _MPRegeneration;
	private bool _isDead;
	private GameObject _vitalBar;
	private GameObject _manaBar;
	private GameObject _gameOver;
	private VitalBarBasic _vitalBarBasic;
	private ManaBarBasic _manaBarBasic;
	
	void Start ()
	{
		_gameOver = GameObject.FindGameObjectWithTag ("GameOver");
		_vitalBar = GameObject.FindGameObjectWithTag ("VitalBar");
		_manaBar = GameObject.FindGameObjectWithTag ("ManaBar");
		_vitalBarBasic = (VitalBarBasic)_vitalBar.gameObject.GetComponent ("VitalBarBasic");
		_manaBarBasic = (ManaBarBasic)_manaBar.gameObject.GetComponent ("ManaBarBasic");
		
		_maxHP = 100f;
		_currentHP = _maxHP;
		_HPRegeneration = 0.5f;
		
		_maxMP = 100f;
		_currentMP = _maxMP;
		_MPRegeneration = 1f;
		
		_isDead = false;
	}
	
	void Update ()
	{
		if (!_isDead) {
			CheckAlive ();
			HPRegeneration ();
			MPRegeneration ();
		}
		CalculateVitalBar ();
		CalculateManaBar ();
		
	}
	
	void HPRegeneration ()
	{
		if (_currentHP < _maxHP) {
			_currentHP += _HPRegeneration * Time.deltaTime;
		}
	}
	
	void MPRegeneration ()
	{
		if (_currentMP < _maxMP) {
			_currentMP += _MPRegeneration * Time.deltaTime;
		}
	}
	
	void CalculateVitalBar ()
	{
		float x = (float)_currentHP / (float)_maxHP;
		string str = (int)_currentHP + "/" + _maxHP;
		
		_vitalBarBasic.UpdateDisplay (x, str);
	}
	
	void CalculateManaBar ()
	{
		float x = (float)_currentMP / (float)_maxMP;
		string str = (int)_currentMP + "/" + _maxMP;
		
		_manaBarBasic.UpdateDisplay (x, str);
	}
	
	void CheckAlive ()
	{
		if (_currentHP < 1 && GodMode == false) {
			_isDead = true;
			PlayerDead ();
		}
	}
	
	public void TakeDamage (int damageTaken)
	{
		_currentHP -= damageTaken;
		if (_currentHP < 0) {
			_currentHP = 0;
		}
	}
	
	public void Heal (int amountHealed)
	{
		_currentHP += amountHealed;
		if (_currentHP > _maxHP) {
			_currentHP = _maxHP;
		}
	}
	
	public void HealMana (int amountHealed)
	{
		_currentMP += amountHealed;
		if (_currentMP > _maxMP) {
			_currentMP = _maxMP;
		}
	}
	
	public bool requestMana (int amount)
	{
		bool requestFullfilled;
		if (amount > _currentMP && GodMode == false) {
			requestFullfilled = false;
		} else {
			_currentMP -= amount;
			if (_currentMP < 0)
				_currentMP = 0;
			requestFullfilled = true;
		}
		return requestFullfilled;
	}
	
	void PlayerDead ()
	{
		AudioSource.PlayClipAtPoint (PlayerDeathSound, transform.position);
		
		_gameOver.gameObject.SendMessage ("LevelReset");
		
		Vector3 target = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 20);
		transform.position = target;
	}
}
