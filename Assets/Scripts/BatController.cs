using UnityEngine;
using System.Collections;

public class BatController : MonoBehaviour
{
	public GameObject[] itemDrops;
	private tk2dSpriteAnimator anim;
	private hoMove _hoMove;
	
	void Start ()
	{
		anim = GetComponent<tk2dSpriteAnimator> ();
		_hoMove = GetComponent<hoMove> ();
	}
	
	void Update ()
	{
		try {
			float targetDir = _hoMove.waypoints [_hoMove.currentPoint].transform.position.x - transform.position.x;
			if (targetDir > 0)
				SetDirection (0);
			else if (targetDir < 0)
				SetDirection (1);
		} catch {
		}
	}
	
	void SetDirection (int moveDirection)
	{
		if (moveDirection == 1 && anim.Sprite.FlipX != false) { //1 = right
			anim.Sprite.FlipX = false;
		}
		if (moveDirection == 0 && anim.Sprite.FlipX != true) { //0 = left
			anim.Sprite.FlipX = true;
		}
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag ("OffensiveSkill")) {
			DropItem ();
			Death ();
		}
	}
	
	void DropItem ()
	{
		if (itemDrops.Length > 0) {
			int randomIndex = Random.Range (0, itemDrops.Length);
			Instantiate (itemDrops [randomIndex], transform.position, Quaternion.identity);
		}
	}
	
	void Death ()
	{
		Destroy (this.gameObject);
	}
}
