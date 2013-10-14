/* Player Controls
 * Created By: Michael Tran
 * 
 * Controls
 * a or left arrow - Move left
 * d or right arrow - Move right
 * space - Jump
 */
using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
	private float walkingSpeed = 10f;
	private float jumpSpeed = 15f;
	private float gravity = 20f;
	private int moveDirection = 1;
	private Vector3 velocity = Vector3.zero;
	private CharacterController controller;
	private tk2dSpriteAnimator anim;
	
	private float afterHitForceDown = 1f;
	
	float previousFrameTime = 0;
	
	
	
	// Use this for initialization
	void Start ()
	{
		controller = GetComponent<CharacterController> ();
		anim = GetComponent<tk2dSpriteAnimator> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		float currentTime = Time.realtimeSinceStartup;
		float deltaTime = currentTime - previousFrameTime;
		previousFrameTime = currentTime;
		
		//Ground Controls
		if (controller.isGrounded) {
			velocity = new Vector3 (Input.GetAxis ("Horizontal"), 0, 0);
			velocity = transform.TransformDirection (velocity);
			velocity *= walkingSpeed;
			
			//Animation Control
			if (Input.GetAxis ("Horizontal") > 0) {
				PlayWalkingAnimation ();
				moveDirection = 1;
			} else if (Input.GetAxis ("Horizontal") < 0) {
				PlayWalkingAnimation ();
				moveDirection = 0;
			} else {
				PlayIdleAnimation ();
			}
			
			
			if (Input.GetButtonDown ("Jump") && !Input.GetButton ("Fire1")) {
				velocity.y = jumpSpeed;
				PlayJumpAnimation ();
			} //Super Jump Skill
			else if (Input.GetButtonDown ("Jump") && Input.GetButton ("Fire1")) {
				velocity.y = jumpSpeed + 10f;
				PlayJumpAnimation ();
				//TODO: Deduct Mana when Used.
			}
		} //Air Controls
		else if (!controller.isGrounded) {
			velocity.x = Input.GetAxis ("Horizontal");
			velocity = transform.TransformDirection (velocity);
			velocity.x *= walkingSpeed;
		}
		
		if(controller.collisionFlags == CollisionFlags.Above)
		{
			//velocity.y = 0;
			//velocity.y -= afterHitForceDown;
			if(velocity.y > 0)
			{
				velocity.y = -velocity.y;
			}
		}
		
		
		SetDirection();
	
		ApplyGravity (deltaTime);
		
		controller.Move (velocity * deltaTime);
	}
	
	void SetDirection ()
	{
		if (moveDirection == 1 && anim.Sprite.FlipX != false) { //1 = right
			anim.Sprite.FlipX = false;
		}
		if (moveDirection == 0 && anim.Sprite.FlipX != true) { //0 = left
			anim.Sprite.FlipX = true;
		}
	}
	
	void ApplyGravity(float deltaTime)
	{
		velocity.y -= gravity * deltaTime;
	}
	
	void PlayWalkingAnimation ()
	{
		if (!anim.IsPlaying ("walking")) {
			anim.Play ("walking");
		}
	}
	
	void PlayIdleAnimation ()
	{
		anim.Play ("idle");
	}
	
	void PlayJumpAnimation ()
	{
		anim.Play ("jump");
	}
	
	/*
	void PlayerDead ()
	{
		gameOver.SetActive (true);
		GameOver gg = (GameOver)gameOver.GetComponent ("GameOver");
		gg.LevelReset ();
		Destroy (this.gameObject);
	}
	*/
}
