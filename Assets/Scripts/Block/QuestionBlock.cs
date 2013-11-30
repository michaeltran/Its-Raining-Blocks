using UnityEngine;
using System.Collections;

public class QuestionBlock : SpecialBlock
{
	public GameObject healthPotion;
	public GameObject manaPotion;
	
	void DestroyObject ()
	{
		// hp = 0-50
		// mp = 51-100
		int random = Random.Range (0, 100);
		
		Vector3 pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		
		if (random <= 50) {
			Instantiate (healthPotion, pos, Quaternion.identity);
		} else if (random > 50) {
			Instantiate (manaPotion, pos, Quaternion.identity);
		}
		Vector3 target = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 20);
		transform.position = target;
		Destroy (this.gameObject);
	}
}
