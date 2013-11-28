using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour {
	
	public RaycastCharacterController controller;
	public RaycastCharacterInput characterInput;
	public tk2dSpriteAnimator playerSprite;
	public AudioClip JumpSound;
	
	
	private float tSoundEnd;
	private int currentDirection;
	private string suffix = "";
	
	void Start() {
		// Register listeners
		controller.CharacterAnimationEvent += new RaycastCharacterController.CharacterControllerEventDelegate (CharacterAnimationEvent);
	}
	
	/// <summary>
	/// Respond to an animation event.
	/// </summary>
	public void CharacterAnimationEvent (CharacterState state, CharacterState previousState) {
		if (!playerSprite.IsPlaying("Change")) {	
			switch (state) {	
				case CharacterState.IDLE: Idle(); break;    
				case CharacterState.SLIDING: Slide(previousState); break;    
				case CharacterState.WALKING: Walk(); break; 
				case CharacterState.RUNNING: Walk(); break;  
				case CharacterState.JUMPING: Jump(); break; 
				case CharacterState.AIRBORNE: Jump(); break; 
				case CharacterState.FALLING: Fall(); break; 
				case CharacterState.DOUBLE_JUMPING: Jump(); break;  
				case CharacterState.WALL_JUMPING: Jump(); break;    
				case CharacterState.CROUCHING: Crouch(); break;    
				case CharacterState.CROUCH_SLIDING: Crouch(); break;    
				case CharacterState.STUNNED: Stun(); break;    
			}
		}
	}
	
	protected void Idle () {
		playerSprite.Play("idle" + suffix);
		CheckDirection();
	}
	
	protected void Slide (CharacterState previousState) {
		if (previousState == CharacterState.AIRBORNE || previousState == CharacterState.FALLING) {
			playerSprite.Play("idle" + suffix);
		}
	}
	
	protected void Walk () {
		playerSprite.Play("walking" + suffix);
		CheckDirection();
	}
	
	protected void Crouch () {
	}
	
	protected void Jump() {
		playerSprite.Play("jump" + suffix);
		if(characterInput.jumpButtonHeld) { AudioSource.PlayClipAtPoint (JumpSound, transform.position); }
		CheckDirection();
	}
	
	protected void Fall() {
	}
	
	
	protected void Stun () {
	}

	protected void CheckDirection ()
	{
		if (controller.CurrentDirection != currentDirection) {
			currentDirection = controller.CurrentDirection;
			
			if (currentDirection > 0) {
				playerSprite.transform.localScale = new Vector3 (1, 1, 1);
				
			} else if (currentDirection < 0) {
				playerSprite.transform.localScale = new Vector3 (-1, 1, 1);
				
			}   
		}
	}
	
}
