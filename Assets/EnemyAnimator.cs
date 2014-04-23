using UnityEngine;
using System.Collections;

public class EnemyAnimator : MonoBehaviour {
	public tk2dSpriteAnimator enemySprite;

	void Update () {
		GameObject target = GameObject.FindGameObjectWithTag("Player");
		if(target.transform.position.x < this.transform.position.x) {
			enemySprite.transform.localScale = new Vector3 (1, 1, 1);
		}
		else {
			enemySprite.transform.localScale = new Vector3 (-1, 1, 1);
		}
	}
}
