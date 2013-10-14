using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour
{
	
	private int currentHP;
	private int maxHP;
	private int currentMP;
	private int maxMP;
	private bool isDead;
	
	private GameObject vitalBar;
	private GameObject manaBar;
	private GameObject gameOver;
	
	// Use this for initialization
	void Start ()
	{
		gameOver = GameObject.FindGameObjectWithTag ("GameOver");
		vitalBar = GameObject.FindGameObjectWithTag ("VitalBar");
		manaBar = GameObject.FindGameObjectWithTag ("ManaBar");
		
		maxHP = 100;
		currentHP = 100;
		
		maxMP = 100;
		currentMP = 100;
		
		isDead = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckAlive();
		CalculateVitalBar();
		CalculateManaBar();
	}
	
	void CalculateVitalBar () {
		VitalBarBasic vit = (VitalBarBasic)vitalBar.gameObject.GetComponent ("VitalBarBasic");

		float x = (float)currentHP / (float)maxHP;
		string str = currentHP + "/" + maxHP;
		
		vit.UpdateDisplay(x, str);
	}
	
	void CalculateManaBar() {
		ManaBarBasic mana = (ManaBarBasic)manaBar.gameObject.GetComponent ("ManaBarBasic");
		
		float x = (float)currentMP / (float)maxMP;
		string str = currentMP + "/" + maxMP;
		
		mana.UpdateDisplay(x, str);
	}
	
	void CheckAlive()
	{
		if(currentHP < 1)
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
	
	public bool requestMana(int amount)
	{
		bool requestFullfilled;
		if(amount > currentMP)
		{
			requestFullfilled = false;
		}
		else
		{
			currentMP -= amount;
			requestFullfilled = true;
		}
		return requestFullfilled;
	}
	
	void PlayerDead ()
	{
		Time.fixedDeltaTime = Time.timeScale * 0.02f;
		GameOver gg = (GameOver)gameOver.GetComponent ("GameOver");
		gg.LevelReset ();
		
		Vector3 target = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 20);
		this.transform.position = target;
	}
}
