using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour
{
	
	public int currentHP;
	private int maxHP;
	private int currentMP;
	private int maxMP;
	private bool isDead;
	
	private GameObject gameOver;
	
	// Use this for initialization
	void Start ()
	{
		gameOver = GameObject.FindGameObjectWithTag ("GameOver");
		
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
	}
	
	void CheckAlive()
	{
		if(currentHP < 1)
		{
			isDead = true;
			PlayerDead ();
		}
	}
	
	public void TakeDamage()
	{
		currentHP = 0;
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
