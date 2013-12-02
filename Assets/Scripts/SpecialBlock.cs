using UnityEngine;
using System.Collections;

public abstract class SpecialBlock : MonoBehaviour {
	public tk2dSpriteAnimator sprite;
	private bool _bumped = false;
	
	void Update ()
	{
		CheckBump ();	
	}
	
	void CheckBump ()
	{
		for(int i = 0; i < 2; i++)
		{
			if (_bumped == false) {
				RaycastHit[] hits = null;
				hits = Physics.RaycastAll (new Vector3 (transform.position.x - 1f + 1f * i, transform.position.y, transform.position.z), -transform.up, 1.3f);
				if (hits.Length > 0 && checkColliderTag(hits)) { 
					_bumped = true;
					Invoke ("CallSkillDestruction", 0.3f);
				}
			}
		}
	}
	
	void CallSkillDestruction ()
	{
		SendMessage ("PlaySkillDestructionFX");
	}
	
	bool checkColliderTag (RaycastHit[] hits)
	{
		foreach(RaycastHit hit in hits)
		{
			string colliderTag = hit.collider.tag;
			if(colliderTag == "PlayerCollider")
				return true;
		}
		return false;
	}
}
