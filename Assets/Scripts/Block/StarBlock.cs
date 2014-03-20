using UnityEngine;
using System.Collections;

public class StarBlock : SpecialBlock
{
	public GameObject healthPotion;
	public GameObject manaPotion;
	public GameObject bomb;
	
	void DestroyObject ()
	{
		// hp = 0-45
		// mp = 46-89
		// bomb = 90-100
		int random = Random.Range (0, 100);
		
		Vector3 pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		
		if (random <= 45) {
			Instantiate (healthPotion, pos, Quaternion.identity);
		} else if (46 <= random && random <= 89) {
			Instantiate (manaPotion, pos, Quaternion.identity);
		} else if (random >= 90) {
			Instantiate (bomb, pos, Quaternion.identity);
		}
		Vector3 target = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 20);
		transform.position = target;
		Destroy (this.gameObject);
	}
}
