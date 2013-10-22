using UnityEngine;
using System.Collections;

public class EnemyStatus : MonoBehaviour {
	public ParticleSystem Poof;
	public ParticleSystem GetHitAnimation;
	public AudioClip GetHitSound;
	public AudioClip DeathSound;
	
	private float currentHP;
	private float maxHP;
	private float _HPRegeneration;
	private bool isDead;
	
	private GameObject gameOver;
	private GameObject vitalBar;
	
	// Use this for initialization
	void Start ()
	{
		vitalBar = GameObject.FindGameObjectWithTag ("EnemyVitalBar");
		gameOver = GameObject.FindGameObjectWithTag ("GameOver");
		
		maxHP = 150f;
		currentHP = maxHP;
		_HPRegeneration = 0.5f;
		
		isDead = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckAlive();
		if(!isDead)
		{
			HPRegeneration ();
		}
		CalculateVitalBar();
	}
	
	void HPRegeneration() {
		if(currentHP < maxHP)
		{
			currentHP += _HPRegeneration * Time.deltaTime;
		}
	}
	
	void CalculateVitalBar () {
		VitalBarBasic vit = (VitalBarBasic)vitalBar.gameObject.GetComponent ("VitalBarBasic");

		float x = (float)currentHP / (float)maxHP;
		
		vit.UpdateDisplay(x);
	}
	
	void CheckAlive()
	{
		if(currentHP < 1)
		{
			isDead = true;
			
			Instantiate(Poof, this.gameObject.transform.position, Quaternion.identity);
			
			AudioSource.PlayClipAtPoint (DeathSound, transform.position);
			
			ChangeScene ();
			Destroy(this.gameObject);
		}
	}
	
	public void TakeDamage(int damageTaken)
	{
		AudioSource.PlayClipAtPoint(GetHitSound, transform.position);
		currentHP -= damageTaken;
		if (currentHP < 0)
		{
			currentHP = 0;
		}
		else{
			Instantiate(GetHitAnimation, this.gameObject.transform.position, Quaternion.identity);
		}
	}
	
	public void ChangeScene()
	{
		gameOver.gameObject.SendMessage ("LevelWin");
	}
	
}
