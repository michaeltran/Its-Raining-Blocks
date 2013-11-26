using UnityEngine;
using System.Collections;

public class EnemyStatus : MonoBehaviour {
	public ParticleSystem Poof;
	public ParticleSystem GetHitAnimation;
	public AudioClip GetHitSound;
	public AudioClip DeathSound;
	
	private float _currentHP;
	private float _maxHp;
	private float _HPRegeneration;
	private bool _isDead;
	private GameObject _gameOver;
	private GameObject _vitalBar;
	
	void Start ()
	{
		_vitalBar = GameObject.FindGameObjectWithTag ("EnemyVitalBar");
		_gameOver = GameObject.FindGameObjectWithTag ("GameOver");
		
		_maxHp = 150f;
		_currentHP = _maxHp;
		_HPRegeneration = 0.5f;
		_isDead = false;
	}
	
	void Update ()
	{
		CheckAlive();
		if(!_isDead) 
		{
			HPRegeneration ();
		}
		CalculateVitalBar();
	}
	
	void HPRegeneration() {
		if(_currentHP < _maxHp)
		{
			_currentHP += _HPRegeneration * Time.deltaTime;
		}
	}
	
	void CalculateVitalBar () {
		VitalBarBasic vit = (VitalBarBasic)_vitalBar.gameObject.GetComponent ("VitalBarBasic");

		float x = (float)_currentHP / (float)_maxHp;
		
		vit.UpdateDisplay(x);
	}
	
	void CheckAlive()
	{
		if(_currentHP < 1)
		{
			_isDead = true;
			
			Instantiate(Poof, this.gameObject.transform.position, Quaternion.identity);
			
			AudioSource.PlayClipAtPoint (DeathSound, transform.position);
			
			ChangeScene ();
			Destroy(this.gameObject);
		}
	}
	
	public void TakeDamage(int damageTaken)
	{
		AudioSource.PlayClipAtPoint(GetHitSound, transform.position);
		_currentHP -= damageTaken;
		if (_currentHP < 0)
		{
			_currentHP = 0;
		}
		else{
			Instantiate(GetHitAnimation, this.gameObject.transform.position, Quaternion.identity);
		}
	}
	
	public void ChangeScene()
	{
		_gameOver.gameObject.SendMessage ("LevelWin");
	}
	
}
