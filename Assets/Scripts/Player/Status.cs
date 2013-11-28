using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour
{
	public AudioClip PlayerDeathSound;
	public bool GodMode = false;
	
	private float currentHP;
	private float maxHP;
	private float _HPRegeneration;
	private float currentMP;
	private float maxMP;
	private float _MPRegeneration;
	private bool isDead;
	
	private GameObject vitalBar;
	private GameObject manaBar;
	private GameObject gameOver;
	
	void Start ()
	{
		gameOver = GameObject.FindGameObjectWithTag ("GameOver");
		vitalBar = GameObject.FindGameObjectWithTag ("VitalBar");
		manaBar = GameObject.FindGameObjectWithTag ("ManaBar");
		
		maxHP = 100f;
		currentHP = maxHP;
		_HPRegeneration = 0.5f;
		
		maxMP = 100f;
		currentMP = maxMP;
		_MPRegeneration = 1f;
		
		isDead = false;
	}
	
	void Update ()
	{
		if(!isDead)
		{
			CheckAlive();
			HPRegeneration ();
			MPRegeneration ();
		}
		CalculateVitalBar();
		CalculateManaBar();
		
	}
	
	void HPRegeneration() {
		if(currentHP < maxHP)
		{
			currentHP += _HPRegeneration * Time.deltaTime;
		}
	}
	
	void MPRegeneration() {
		if(currentMP < maxMP)
		{
			currentMP += _MPRegeneration * Time.deltaTime;
		}
	}
	
	void CalculateVitalBar () {
		VitalBarBasic vit = (VitalBarBasic)vitalBar.gameObject.GetComponent ("VitalBarBasic");

		float x = (float)currentHP / (float)maxHP;
		string str = (int)currentHP + "/" + maxHP;
		
		vit.UpdateDisplay(x, str);
	}
	
	void CalculateManaBar() {
		ManaBarBasic mana = (ManaBarBasic)manaBar.gameObject.GetComponent ("ManaBarBasic");
		
		float x = (float)currentMP / (float)maxMP;
		string str = (int)currentMP + "/" + maxMP;
		
		mana.UpdateDisplay(x, str);
	}
	
	void CheckAlive()
	{
		if(currentHP < 1 && GodMode == false)
		{
			isDead = true;
			PlayerDead ();
		}
	}
	
	public void TakeDamage(int damageTaken)
	{
		currentHP -= damageTaken;
		if (currentHP < 0)
		{
			currentHP = 0;
		}
	}
	
	public void Heal(int amountHealed)
	{
		currentHP += amountHealed;
		if (currentHP > maxHP)
		{
			currentHP = maxHP;
		}
	}
	
	public void HealMana(int amountHealed)
	{
		currentMP += amountHealed;
		if (currentMP > maxMP)
		{
			currentMP = maxMP;
		}
	}
	
	public bool requestMana(int amount)
	{
		bool requestFullfilled;
		if(amount > currentMP && GodMode == false)
		{
			requestFullfilled = false;
		}
		else
		{
			currentMP -= amount;
			if(currentMP < 0)
				currentMP = 0;
			requestFullfilled = true;
		}
		return requestFullfilled;
	}
	
	void PlayerDead ()
	{
		//Time.fixedDeltaTime = Time.timeScale * 0.02f;
		
		AudioSource.PlayClipAtPoint (PlayerDeathSound, transform.position);
		
		GameOver gg = (GameOver)gameOver.GetComponent ("GameOver");
		gg.LevelReset ();
		
		Vector3 target = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 20);
		this.transform.position = target;
	}
}
