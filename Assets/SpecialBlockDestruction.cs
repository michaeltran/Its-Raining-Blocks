using UnityEngine;
using System.Collections;

public class SpecialBlockDestruction : BlockDestruction {
	
	public void PlaySkillDestructionFX()
	{
		if(BlockDestructionSound != null) { AudioSource.PlayClipAtPoint(BlockDestructionSound, transform.position); }
		if(DestructionFX != null) {Instantiate(DestructionFX, transform.position, Quaternion.identity); }
		SendMessage ("DestroyObject");
	}
}
