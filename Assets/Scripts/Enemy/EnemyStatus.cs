using UnityEngine;
using System.Collections;

public class EnemyStatus : AbstractStatus
{
	public ParticleSystem deathFX;
	public ParticleSystem getHitAnimation;
	public AudioClip getHitSound;
	public AudioClip deathSound;
	public float setHP = 150f;
	private bool _invincible = false;
	
	public bool _Invincible {
		get { return _invincible; }
		set { _invincible = value; }
	}
	
	void Start ()
	{
		_vitalBar = GameObject.FindGameObjectWithTag ("EnemyVitalBar");
		_gameOver = GameObject.FindGameObjectWithTag ("GameOver");
		_vitalBarBasic = (VitalBarBasic)_vitalBar.gameObject.GetComponent ("VitalBarBasic");
		
		_maxHP = setHP;
		_currentHP = _maxHP;
		_HPRegeneration = 0.5f;
	}
	
	void Update ()
	{
		CheckAlive ();
		if (!_isDead) {
			HPRegeneration ();
		}
		CalculateVitalBar ();
	}
	
	void CalculateVitalBar ()
	{
		float x = (float)_currentHP / (float)_maxHP;
		_vitalBarBasic.UpdateDisplay (x);
	}
	
	void CheckAlive ()
	{
		if (_currentHP < 1) {
			_isDead = true;
			
			Instantiate (deathFX, transform.position, Quaternion.identity);
			
			AudioSource.PlayClipAtPoint (deathSound, transform.position);
			
			ChangeScene ();
			Destroy (this.gameObject);
		}
	}
	
	public void TakeDamage (int damageTaken)
	{
		if (_invincible == false) {
			AudioSource.PlayClipAtPoint (getHitSound, transform.position);
			_currentHP -= damageTaken;
			if (_currentHP < 0) {
				_currentHP = 0;
			} else {
				Instantiate (getHitAnimation, transform.position, Quaternion.identity);
			}
		} 
	}
	
	public void ChangeScene ()
	{
		_gameOver.gameObject.SendMessage ("LevelWin");
	}
}
