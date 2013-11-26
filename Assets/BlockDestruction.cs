using UnityEngine;
using System.Collections;

public class BlockDestruction : MonoBehaviour {

	public ParticleSystem DestructionFX;
	public AudioClip BlockDestructionSound;
	
	public void PlaySkillDestructionFX()
	{
		if(BlockDestructionSound != null) { AudioSource.PlayClipAtPoint(BlockDestructionSound, transform.position); }
		if(DestructionFX != null) {Instantiate(DestructionFX, transform.position, Quaternion.identity); }
		Destroy (this.gameObject);
	}
}
