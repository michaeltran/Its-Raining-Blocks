using UnityEngine;
using System.Collections;

public class EnemyStatus : MonoBehaviour {
	public ParticleSystem Poof;
	
	private float currentHP;
	private float maxHP;
	private float _HPRegeneration;
	private bool isDead;
	
	private GameObject vitalBar;
	
	// Use this for initialization
	void Start ()
	{
		vitalBar = GameObject.FindGameObjectWithTag ("EnemyVitalBar");
		
		maxHP = 100f;
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
			Debug.Log ("Enemy downnn");
			Instantiate(Poof, this.gameObject.transform.position, Quaternion.identity);
			
			Destroy(this.gameObject);
		}
	}
	
	void GGNORE()
	{
		
	}
	
	public void TakeDamage(int damageTaken)
	{
		currentHP -= damageTaken;
		if (currentHP < 0)
		{
			currentHP = 0;
		}
	}
	
}
