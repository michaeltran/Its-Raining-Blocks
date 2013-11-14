using UnityEngine;
using System.Collections;

/// <summary>
/// Raycast character controller a controller for 2D platform games.
/// </summary>
public class RaycastCharacterController : MonoBehaviour {
	
	/// <summary>
	/// The feet colliders. These colliders push the characters upwards and also hnadle climbing.
	/// </summary>
	public RaycastCollider[] feetColliders;
	/// <summary>
	/// The head colliders. These characters push the chraacter down when the head is hit.
	/// </summary>
	public RaycastCollider[] headColliders;
	/// <summary>
	/// The sides. These colliders push the character left and right when they hit obstacles.
	/// </summary>
	public RaycastCollider[] sides;
	/// <summary>
	/// The movement details, controls how the chracter moves on the ground.
	/// </summary>
	public MovementDetails movement;
	/// <summary>
	/// The jump details. Controls how the character moves in the air.
	/// </summary>
	public JumpDetails jump;
	/// <summary>
	/// The wall details. Controls how the character interacts with walls.
	/// </summary>
	public WallDetails wall;
	/// <summary>
	/// The slopes details. Controls how the character handles sloped platforms.
	/// </summary>
	public SlopeDetails slopes;
	/// <summary>
	/// The climbing details. Controls how a chracter handles climbables like ropes and ladders.
	/// </summary>
	public ClimbDetails climbing;
	/// <summary>
	/// The ledge hanging details. Controls how a character handles ledges.
	/// </summary>
	public LedgeDetails ledgeHanging;
	/// <summary>
	/// The crouch details. Controls how a chracter handles crouching.
	/// </summary>
	public CrouchDetails crouch;
	/// <summary>
	/// The type of stun to apply.
	/// </summary>
	public StunType stun;
	/// <summary>
	/// The layer of normal objects that the chracter cannot pass through.
	/// </summary>
	public int backgroundLayer;
	/// <summary>
	/// The layer of object which the chracter can jump through but can not fall through.
	/// </summary>
	public int passThroughLayer;
	/// <summary>
	/// The layer of objects the character can climb.
	/// </summary>
	public int climableLayer;
	/// <summary>
	/// Is the controller active. If this is false nothing will move. Use for cutscenes, static animations
	/// character death, pause, etc.
	/// </summary>
	public bool controllerActive = true;
	/// <summary>
	/// If true the animation events will be sent constantly, if false (default) only changes of state will be sent.
	/// </summary>
	public bool sendAnimationEventsEachFrame = false;
	/// <summary>
	/// How far to look ahead when considering if this character is grounded.
	/// </summary>
	public float groundedLookAhead = 0.25f;
	/// <summary>
	/// If the character is moving slower than this they will be considered idle.
	/// </summary>
	public float maxSpeedForIdle = 0.1f;
	/// <summary>
	/// The character input that controls this character.
	/// </summary>
	public RaycastCharacterInput characterInput;
	/// <summary>
	/// Assign a direction checker if you want to do one of two things.
	/// 1) Determine the way the character is facing using a mechanism
	/// other than the default (for example based on mouse position) OR
	/// 2) Switch or change colliders based on the direction the character
	/// is facing (for example if you have an assymetrical character).
	/// </summary>
	public DirectionChecker directionChecker;
	
	/// <summary>
	/// The maximum time a frame can take. Smaller values allow your character to move faster, larger values will tend to make lag
	/// less apparent. Static so it is the same across all components.
	/// WARNING: If this is too large your character can fall through the ground, or move through small platforms.
	/// </summary>
	public static float maxFrameTime = 0.033f;
	

	#region private members
	
	private Platform myParent;
	private Transform myTransform;
	protected float frameTime;
	private Vector3 velocity;
	private int jumpCount = 0;
	private float jumpHeldTimer = 0.0f;
	private float jumpButtonTimer = 0.0f;
	private float fallingTime = 0.0f;
	private float currentDrag = 0.0f;
	private bool startedClimbing = false;
	private float fallThroughTimer = 0.0f;
	
	private bool isLedgeHanging = false;
	private LedgeHangingState ledgeHangState;
	private float ledgeHangTimer = 0.0f;
	private Vector3 ledgeHangOriginalPosition;
	private Vector3 ledgeHangGoalPosition;
	private RC_Direction ledgeHangDirection;
	private RaycastCollider[] highestSideColliders;
	private float ledgeDropTimer;
	
	private bool isLadderTopClimbing;
	private LadderTopState ladderTopClimbState;

	private RC_Direction wallJumpDirection;
	private float wallJumpTimer = 0.0f;
	private bool isWallHolding = false;
	private bool isWallSliding = false;
	private RC_Direction wallSlideDirection;
	private float oppositeDirectionTimer;
	private bool hasPressedDirectionForWallJump;
	private bool hasPressedJumpForWallJump;
	private bool isClimbingUpOrDown = false;

	protected float stunTimer;

	private bool isCrouching;
	private bool isCrouchSliding;
	private float crouchSlideDrag;

	private int groundedFeet;
	
	private CharacterState state;
	private CharacterState potentialState;
	private CharacterState previousState;
	private bool characterStateForced = false;

	private LadderControl dismounting;

	#endregion
	
	#region events
	
	// Events sent for animations. Listen to these to animate your character.
	
	public delegate void CharacterControllerEventDelegate(CharacterState state, CharacterState previousState);
	public event CharacterControllerEventDelegate CharacterAnimationEvent;
	
	#endregion
	
	#region public methods and properties
	
	/// <summary>
	/// Sets the drag for the current frame. Use this to implement slippery or sticky platforms.
	/// </summary>
	/// <param name='drag'>
	/// Drag for the current frame.
	/// </param>
	public void SetDrag(float drag) {
		currentDrag = drag;
	}
	
	/// <summary>
	/// Gets a value indicating whether the cahracter is climbing up or down (as opposed to 
	/// just holding on to a climbable). Value is undefined if StartedClimbing = false;
	/// </summary>
	public bool IsClimbingUpOrDown {
			get { return isClimbingUpOrDown;}
	}
	
	/// <summary>
	/// Gets or sets the velocity. You can interact with this directly in order to create (for example)
	/// escalators or coveyer belts.
	/// </summary>
	/// <value>
	/// The velocity.
	/// </value>
	public Vector2 Velocity {
		get { return velocity; }	
		set { velocity = value; }
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="RaycastCharacterController"/> has started climbing a climbable
	/// </summary>
	/// <value>
	/// <c>true</c> if started climbing; otherwise, <c>false</c>.
	/// </value>
	public bool StartedClimbing {
		get { return startedClimbing;}
	}
	
	/// <summary>
	/// Dismount the specified Ladder. This is used to stop
	/// autosticking to the same ladder you just dismounted.
	/// </summary>
	/// <param name="ladder">Ladder control to dismount.</param>
	public void Dismount (Ladder ladder){
		Unparent ();
		dismounting = ladder.control;
		startedClimbing = false;
	}

	/// <summary>
	///  Force the character to unparent from the given object.
	/// </summary>
	/// <returns>true if the object was parented to the supplied platform and then successfully unparented.</returns>
	/// <param name="platform">The platform to unparent from.</param>
	public bool Unparent (Platform platform) {
		if (myParent == platform) {
			myParent = null;
			myTransform.parent = null;
			return true;
		}
		return false;
	}
	
	/// <summary>
	/// Get the animation state of the character. You can use this for animation as an alternative to listening to events.
	/// </summary>
	/// <value>
	/// The current animation state.
	/// </value>
	public CharacterState State{
		get { return state; }
		private set {
			if (!characterStateForced && (int) value > (int)potentialState) potentialState = value;
		}
	}
	
	/// <summary>
	/// Forces the state of the character to be the provided value.
	/// </summary>
	/// <param name='state'>
	/// State.
	/// </param>
	public void ForceSetCharacterState(CharacterState state) {
		potentialState = state;
		characterStateForced = true;
	}
	
	/// <summary>
	/// Determines whether this character is grounded.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is grounded; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='offset'>
	/// How much to look ahead from the feet colliders when determining if the character is grounded.
	/// </param>
	public bool IsGrounded(float offset, bool includeClimables = true){
		foreach (RaycastCollider foot in feetColliders) {
			if (foot.IsColliding(1 << backgroundLayer | 1 << passThroughLayer | (includeClimables ? 1 << climableLayer : 0) , offset)) return true;
		}
		return false;
	}
	
	/// <summary>
	/// Gets the parent platform
	/// </summary>
	public Platform MyParent {
		get{return myParent;}
	}


	/// <summary>
	/// Sets the fall through timer (used by passthrough platforms).
	/// </summary>
	public float FallThroughTimer {
		get {return fallThroughTimer;}
		set {fallThroughTimer = value;}
	}
	
	/// <summary>
	/// The current direction being faced. 0 = NONE, 1 = RIGHT
	/// -1 = LEFT. Uses a direction checker if one is assigned
	/// else the character will face the direction that is being held
	/// or the direction they are moving in.
	/// </summary>
	public int CurrentDirection {
		get {
			if (directionChecker != null) return directionChecker.CurrentDirection;
			// Always return ledge hang dir - NEW
			if (isLedgeHanging) {
				if (ledgeHangDirection == RC_Direction.RIGHT) return 1;
				if (ledgeHangDirection == RC_Direction.LEFT) return -1;
			}
			if (characterInput.x > 0.0f) return 1;
			if (characterInput.x < 0.0f) return -1;
			if (velocity.x > 0.0f) return 1;
			if (velocity.x < 0.0f) return -1;
			
			return 0;
		}
	}
	
	/// <summary>
	/// Gets the frame time.
	/// </summary>
	/// <value>
	/// The frame time.
	/// </value>
	public static float FrameTime{
		get { return Mathf.Min (Time.deltaTime, maxFrameTime);	}
	}
	
	/// <summary>
	/// Returns how many of the feet colliders are on the ground. You can use this
	/// to play a "teeter" animation.
	/// </summary>
	public int GroundedFeetCount {
		get { return groundedFeet;}
	}
	
	/// <summary>
	/// Switches the colliders direction. This is a default implementation
	/// a direction checker may choose to do something different. This is primarily
	/// for assymetrical characters.
	/// </summary>
	public void SwitchColliders() {	
		// Head Colliders
		foreach (RaycastCollider c in headColliders) {
			c.offset = new Vector3 (c.offset.x * -1, c.offset.y, c.offset.z);
		}
		// Feet Colliders
		foreach (RaycastCollider c in feetColliders) {
			c.offset = new Vector3 (c.offset.x * -1, c.offset.y, c.offset.z);
		}
		// Side Colliders
		foreach (RaycastCollider c in sides) {
			c.offset = new Vector3 (c.offset.x * -1, c.offset.y, c.offset.z);
			if (c.direction == RC_Direction.LEFT) {
				c.direction = RC_Direction.RIGHT;
			} else if (c.direction == RC_Direction.RIGHT) {
				c.direction = RC_Direction.LEFT;
			}
			RaycastCollider tempCollider = highestSideColliders [0];
			highestSideColliders [0] = highestSideColliders [1];
			highestSideColliders [1] = tempCollider;
		}
		// Ledge hang offset
		if (ledgeHanging.canLedgeHang) {
			ledgeHanging.hangOffset = new Vector3(ledgeHanging.hangOffset.x * -1, ledgeHanging.hangOffset.y, 0.0f);
			// TODO Remove this - We don't need to flip this one as we apply it based on hang direction
			// ledgeHanging.climbOffset = new Vector3(ledgeHanging.climbOffset.x * -1, ledgeHanging.climbOffset.y, 0.0f);
		}
	}
	
	/// <summary>
	/// Stuns the character so it cannot move for the specified time. The exact definition of
	/// "cannot move" depends on the stun settings.
	/// A stun animation event will also be sent.
	/// </summary>
	/// <param name="stunTime">Stun time.</param>
	public void Stun (float stunTime) {
		stunTimer = stunTime;
		isLedgeHanging = false;
	}

	/// <summary>
	/// Gets a value indicating if the character is ledge hanging.
	/// </summary>
	public bool IsLedgeHanging {
		get { return isLedgeHanging;}
	}

	/// <summary>
	/// Gets a value indicating the direciton the character is hanging. Value has no meaning
	/// if the character is not ledge hanging.
	/// </summary>
	public RC_Direction LedgeHangDirection {
		get { return ledgeHangDirection;}
	}

	#endregion
	
	#region Unity hooks
	
	/// <summary>
	/// Unity awake hook.
	/// </summary>
	void Awake () {
		myTransform = transform;	
		velocity = Vector3.zero;
		currentDrag = movement.drag;
		
		// Assign default transforms
		if (feetColliders != null) {
			foreach (RaycastCollider c in feetColliders) {
				if (c.transform == null) c.transform = transform;
			}
		}
		if (headColliders != null) {
			foreach (RaycastCollider c in headColliders) {
				if (c.transform == null) c.transform = transform;	
			}
		}
		if (sides != null) {
			float[ ]highestSideColliderHeight = new float[]{-9999.0f, -9999.0f};
			highestSideColliders = new RaycastCollider[2];
			foreach (RaycastCollider c in sides) {
				if (c.transform == null) c.transform = transform;	
				if (c.direction == RC_Direction.RIGHT && c.offset.y > highestSideColliderHeight[0]) {
					highestSideColliders[0] = c;
					highestSideColliderHeight[0] = c.offset.y;
				} else if (c.direction == RC_Direction.LEFT && c.offset.y > highestSideColliderHeight[1]) {
					highestSideColliders[1] = c;
					highestSideColliderHeight[1] = c.offset.y;
				}
			}
		}	
	}
	
	/// <summary>
	/// Do the movement. This is done in LateUpdate so other objects can alter the character in Update.
	/// </summary>
	void LateUpdate() {
		frameTime = FrameTime;
		if (controllerActive) {
			bool grounded = IsGrounded(groundedLookAhead);
			if (grounded) {
				fallingTime = 0.0f;
			} else {
				fallingTime += frameTime;
			}
			if (ledgeDropTimer > 0 ) {
				ledgeDropTimer -= frameTime;
			}
			if (stunTimer > 0 ) {
				stunTimer -= frameTime;
				State = CharacterState.STUNNED;
			}
			if (ledgeHanging.canLedgeHang && isLedgeHanging) {
				DoLedgeHang();
			}else if (climbing.allowClimbing && isLadderTopClimbing) {
				DoLadderTop();
			} else {
				if (crouch.canCrouch && stunTimer <= 0.0f) {
					DoCrouch();
				} 
				if (stunTimer <= 0.0f || stun == StunType.STOP_INPUT_ONLY ||  stun == StunType.STOP_INPUT_AND_Y_MOVEMENT) {
					MoveInXDirection(grounded);
				}
				if (!isLedgeHanging && (stunTimer <= 0.0f || stun == StunType.STOP_INPUT_ONLY || stun == StunType.STOP_INPUT_AND_X_MOVEMENT)) {
					MoveInYDirection(grounded);
				}
				if (directionChecker != null ) directionChecker.UpdateDirection(this);
			}
			UpdateAnimation();
		}
	}
	
	/// <summary>
	/// Updates the animation state and sends an event.
	/// </summary>
	protected void UpdateAnimation() {
		characterStateForced = false;
		if ((sendAnimationEventsEachFrame || potentialState != state) && potentialState != CharacterState.NONE) {
			previousState = state;
			state = potentialState;
			if (CharacterAnimationEvent != null) CharacterAnimationEvent(state, previousState);
		}
		potentialState = CharacterState.NONE;
	}
	
	/// <summary>
	/// Unity hook, draw the raycast colliders.
	/// </summary>
	void OnDrawGizmos(){
		if (feetColliders != null) {
			foreach (RaycastCollider c in feetColliders) {
				if (c.transform == null) c.transform =  transform ;
				c.DrawRayCast();	
			}
		}
		if (headColliders != null) {
			foreach (RaycastCollider c in headColliders) {
				if (c.transform == null) c.transform = transform;
				c.DrawRayCast();	
			}
		}
		if (sides != null) {
			foreach (RaycastCollider c in sides) {
				if (c.transform == null) c.transform = transform;
				c.DrawRayCast();	
			}
		}	
	}
	
	
	#endregion
	
	protected void MoveInXDirection (bool grounded)
	{

		// Create a copy of character input which we can modify;
		float characterInputX = characterInput.x;
		
		// Ignore input if stunned
		if (stunTimer > 0.0f) characterInputX = 0.0f;
		
		// Ignore input if crouching
		if (isCrouching) characterInputX = 0.0f;

		// Calculate Velocity
		float actualDrag = 1 - (currentDrag * Time.deltaTime);

		// Jump Drag
		if (jumpCount > 0 || !grounded) {
			actualDrag = 1 - (jump.drag * Time.deltaTime);
		}

		// Crouch slide drag		
		if (isCrouchSliding) {
			actualDrag = 1 - (crouchSlideDrag * Time.deltaTime);
		}
		
		// Timer which ignores character input while jumping away from a wall
		if (wall.canWallJump && oppositeDirectionTimer > 0.0f) {
			oppositeDirectionTimer -= frameTime;
			if (wallJumpDirection == RC_Direction.RIGHT) characterInputX = -0.5f;
			else if (wallJumpDirection == RC_Direction.LEFT) characterInputX = 0.5f;
		}
		
		if (characterInputX != 0 && (myParent == null || !myParent.overrideX)) {
			bool walking = false;
			if (jumpCount > 0 || !grounded || (characterInputX > 0 && characterInputX < 1.0f) || (characterInputX < 0 && characterInputX > -1.0f))
				walking = true;
			float newVelocity = velocity.x + (frameTime * movement.acceleration * characterInputX);
			newVelocity = newVelocity * actualDrag;
			// Climbing sideways
			if (climbing.allowClimbing && startedClimbing) {
				velocity.x = newVelocity;
				if (velocity.x > climbing.horizontalSpeed)
					velocity.x = climbing.horizontalSpeed;
				if (velocity.x < -1 * climbing.horizontalSpeed)
					velocity.x = -1 * climbing.horizontalSpeed;
			} else if (walking) {
				// If going too fast just apply drag (don't limit to walk speed else we will get odd jerks)
				if (velocity.x > movement.walkSpeed && characterInputX >= 0.0f || velocity.x < movement.walkSpeed * -1 && characterInputX <= 0.0f) {
					velocity.x = velocity.x * actualDrag;
				} else {
					velocity.x = newVelocity;
					// Limit to walk speed;
					if (velocity.x > movement.walkSpeed)
						velocity.x = movement.walkSpeed;
					if (velocity.x < -1 * movement.walkSpeed)
						velocity.x = -1 * movement.walkSpeed;
				}
			} else {
				velocity.x = newVelocity;
				// Limit to run speed;
				if (velocity.x > movement.runSpeed)
					velocity.x = movement.runSpeed;
				if (velocity.x < -1 * movement.runSpeed)
					velocity.x = -1 * movement.runSpeed;
			}
		} else {
			velocity.x = velocity.x * actualDrag;
		}
		
		// Apply velocity
		if ((myParent == null || !myParent.overrideX) && velocity.x > movement.skinSize || velocity.x * -1 > movement.skinSize) {
			myTransform.Translate (velocity.x * frameTime, 0.0f, 0.0f);		}
		
		float forceSide = 0.0f;
		
		// Ledge Hang variables
		int ledgeHangHitCount = 0;
		Collider ledgeHangCollider = null;
		RaycastCollider actualLedgeCollider = null;
		
		// Wall slide variables
		int wallSlideCount = 0;
		RaycastCollider actualWallCollider = null;
		
		isWallSliding = false;
		isWallHolding = false;
		
		for (int i = 0; i < sides.Length; i++) {
			RaycastHit hitSides;
			float additionalDistance = 0.0f;
			// If crouching and using autoHeightReduction skip any sides higher than provided value
			if (!isCrouching || !crouch.useHeightReduction || sides[i].offset.y <= crouch.ignoredSideCollidersHigherThan) {
				if (ledgeHanging.autoGrab && velocity.y <= 0.0f) additionalDistance = ledgeHanging.autoGrabDistance;
				else if (wall.canWallSlide) additionalDistance = wall.wallSlideAdditionalDistance;
	
				hitSides = sides [i].GetCollision (1 << backgroundLayer, additionalDistance);
				
				// Hit something ...
				if (hitSides.collider != null) {
					
					// Update ledge hang
					if (ledgeHanging.canLedgeHang && !grounded && velocity.y <= 0.0f) {
						if (sides [i].direction == RC_Direction.RIGHT && (sides [i] == highestSideColliders [1] || sides [i] == highestSideColliders [0])) {
							ledgeHangHitCount++;
							ledgeHangDirection = RC_Direction.RIGHT;
							ledgeHangCollider = hitSides.collider;
							if (sides [i] == highestSideColliders [1]) {
								actualLedgeCollider = highestSideColliders [1]; 
							} else {
								actualLedgeCollider = highestSideColliders [0]; 
							}
						} else if (sides [i].direction == RC_Direction.LEFT && (sides [i] == highestSideColliders [1] || sides [i] == highestSideColliders [0])) {
							ledgeHangHitCount++;
							ledgeHangDirection = RC_Direction.LEFT;
							ledgeHangCollider = hitSides.collider;
							if (sides [i] == highestSideColliders [1]) {
								actualLedgeCollider = highestSideColliders [1]; 
							} else {
								actualLedgeCollider = highestSideColliders [0]; 
							}
						}
					}

					/// Update wall slide
					if ((wall.canWallSlide || wall.canWallJump) && !grounded) {
						if ((sides [i] == highestSideColliders [1] || sides [i] == highestSideColliders [0])) { 
							if (hitSides.distance <= sides [i].distance + wall.wallSlideAdditionalDistance) {
								wallSlideCount++;
								wallSlideDirection = sides[i].direction;
								if (sides [i] == highestSideColliders [1]) {
									actualWallCollider = highestSideColliders [1]; 
								} else {
									actualWallCollider = highestSideColliders [0]; 
								}
							}
						}	
					}
					
					// Check for platforms, but only if we are within collider distance + skinSize
					if (hitSides.distance <= sides [i].distance + movement.skinSize) {
						Platform platform = hitSides.collider.gameObject.GetComponent<Platform> ();
						if (platform != null) {
							platform.DoAction (sides [i], this);
						}
					}
				}
			
				// Stop movement, but only if we are within collider distance
				if (hitSides.distance <= sides [i].distance) {
					float tmpForceSide = (hitSides.normal * (sides [i].distance - hitSides.distance)).x;
					if (tmpForceSide > Mathf.Abs (forceSide) || tmpForceSide * -1 > Mathf.Abs (forceSide)) {
						forceSide = tmpForceSide;
						//	TODO Remove this after adequate testing break;
					}
				}
			}
		}
		
		// Check for ledge hang
		if (ledgeHanging.canLedgeHang && ledgeHangHitCount == 1) {
			bool stopLedgeHang = false;
			if (ledgeHangDirection == RC_Direction.LEFT) {
				if (actualLedgeCollider.IsColliding (1 << backgroundLayer, ledgeHanging.edgeDetectionOffset.x + ledgeHanging.autoGrabDistance, ledgeHanging.edgeDetectionOffset.y)) {
					stopLedgeHang = true;
				}
				if (!stopLedgeHang &&
				    ledgeHangCollider.bounds.max.y < myTransform.position.y + actualLedgeCollider.offset.y + ledgeHanging.graspPoint + ledgeHanging.graspLeeway &&
				    ledgeHangCollider.bounds.max.y > myTransform.position.y + actualLedgeCollider.offset.y + ledgeHanging.graspPoint - ledgeHanging.graspLeeway) {
					isLedgeHanging = true;
					jumpCount = 2;
					ledgeHangState = LedgeHangingState.TRANSITION;
					ledgeHangTimer = 0.0f;
					ledgeHangOriginalPosition = myTransform.position;
					ledgeHangGoalPosition = ledgeHanging.hangOffset + new Vector3 (ledgeHangCollider.bounds.max.x, ledgeHangCollider.bounds.max.y, myTransform.position.z);
					// Check for parent
					Platform platform = ledgeHangCollider.gameObject.GetComponent<Platform> ();
					if (platform != null) {
						Transform parentPlatform = platform.ParentOnStand (this);
						if (parentPlatform != null) {
							myParent = platform;
							if (myTransform.parent != parentPlatform) {
								myTransform.parent = parentPlatform;
							}
							ledgeHangGoalPosition += platform.velocity * ledgeHanging.transitionTime;
						} else {
							// Force ladder to unparent if we have a "bobble"
							if (myParent is Ladder) {
								Unparent();
							}
						}
					}
				}
			} else if (ledgeHangDirection == RC_Direction.RIGHT) {
				if (actualLedgeCollider.IsColliding (1 << backgroundLayer, ledgeHanging.edgeDetectionOffset.x + ledgeHanging.autoGrabDistance, ledgeHanging.edgeDetectionOffset.y)) {
					stopLedgeHang = true;
				}
				if (!stopLedgeHang &&
				    ledgeHangCollider.bounds.max.y < myTransform.position.y + actualLedgeCollider.offset.y + ledgeHanging.graspPoint + ledgeHanging.graspLeeway &&
				    ledgeHangCollider.bounds.max.y > myTransform.position.y + actualLedgeCollider.offset.y + ledgeHanging.graspPoint - ledgeHanging.graspLeeway) {
					isLedgeHanging = true;
					jumpCount = 2;
					ledgeHangState = LedgeHangingState.TRANSITION;
					ledgeHangTimer = 0.0f;
					ledgeHangOriginalPosition = myTransform.position;
					ledgeHangGoalPosition = ledgeHanging.hangOffset + new Vector3 (ledgeHangCollider.bounds.min.x, ledgeHangCollider.bounds.max.y, myTransform.position.z);
					// Check for parent
					Platform platform = ledgeHangCollider.gameObject.GetComponent<Platform> ();
					if (platform != null) {
						Transform parentPlatform = platform.ParentOnStand (this);
						if (parentPlatform != null) {
							myParent = platform;
							if (myTransform.parent != parentPlatform) {
								myTransform.parent = parentPlatform;
							}
							ledgeHangGoalPosition += platform.velocity * ledgeHanging.transitionTime;
						}
					}
				}
			}
		}
		
		// Check for wall slide
		if (wallSlideCount > 0 && velocity.y <= 0.0f) {
			isWallHolding = true;
			bool stopWallSlide = true;
			if (actualWallCollider.IsColliding (1 << backgroundLayer, wall.edgeDetectionOffset.x, wall.edgeDetectionOffset.y)) {
				stopWallSlide = false;
			}
			if (!stopWallSlide && wall.canWallSlide && ((wallSlideDirection == RC_Direction.RIGHT && characterInput.x > 0) || (wallSlideDirection == RC_Direction.LEFT && characterInput.x < 0))) {
				isWallSliding = true;
				State = CharacterState.WALL_SLIDING;
			}
		}
		
		// Move
		if (forceSide > movement.skinSize) {	
			myTransform.Translate(Mathf.Max(velocity.x * frameTime * -1, forceSide), 0.0f, 0.0f);		
			wallJumpDirection = RC_Direction.LEFT;
			wallJumpTimer = wall.wallJumpTime;
			hasPressedJumpForWallJump = false;
			hasPressedDirectionForWallJump = false;
			StopCrouch();
		} else if (-1 * forceSide > movement.skinSize) {		
			myTransform.Translate(Mathf.Min(velocity.x * frameTime * -1, forceSide), 0.0f, 0.0f);	
			wallJumpDirection = RC_Direction.RIGHT;
			wallJumpTimer = wall.wallJumpTime;
			hasPressedJumpForWallJump = false;
			hasPressedDirectionForWallJump = false;
			StopCrouch();
		}
		if ((forceSide > 0 && velocity.x < 0) || (forceSide < 0 && velocity.x > 0)) {
			velocity.x = 0.0f;
		}
		
		// Animation 
		if (grounded) {
			if  (		(velocity.x > movement.walkSpeed && characterInput.x > 0.1f) || 
			     (velocity.x < movement.walkSpeed * -1 && characterInput.x < -0.1f)) {
				State = CharacterState.RUNNING;
			} else if (	(velocity.x > maxSpeedForIdle && characterInput.x > 0.1f) || 
			           (velocity.x < -1 * maxSpeedForIdle  && characterInput.x < -0.1f)){
				State = CharacterState.WALKING;
			} else if (	velocity.x > maxSpeedForIdle || velocity.x < -1 * maxSpeedForIdle ){
				State = CharacterState.SLIDING;
			} else {
				State = CharacterState.IDLE;
			}
		} 
		
		// Reset Drag
		currentDrag = movement.drag;
	}
	
	protected void MoveInYDirection (bool grounded) {
		float slope = 0.0f;
		int slopeCount = -1; 
		bool isClimbing = false;
		bool cantClimbDown = false;
		bool hasHitHead = false;
		int climbCount = 0;
		groundedFeet = 0;
		isClimbingUpOrDown = false;
		
		// Create a copy of character input which we can modify;
		float characterInputY = characterInput.y;
		
		// Ignore input if stunned
		if (stunTimer > 0.0f) characterInputY = 0.0f;

		// Limit Velocity
		if (velocity.y < movement.terminalVelocity)
			velocity.y = movement.terminalVelocity;
		
		// Apply velocity
		if ((myParent == null || !myParent.overrideY) && (velocity.y > movement.skinSize || velocity.y * -1 > movement.skinSize)) {
			myTransform.Translate (0.0f, velocity.y * frameTime, 0.0f, Space.World);
		}
		
		if (fallThroughTimer > 0.0f)
			fallThroughTimer -= frameTime;
		
		// Fall/Stop
		bool hasHitFeet = false;
		if ((velocity.y <= 0.0f || startedClimbing ) && ledgeDropTimer <= 0.0f) {
			float maxForce = 0.0f;
			GameObject hitGameObject = null;
			float lastHitDistance = -1;
			float lastHitX = 0.0f;
			
			foreach (RaycastCollider feetCollider in feetColliders) {
				RaycastHit hitFeet = new RaycastHit ();
				RaycastHit hitLadder = new RaycastHit ();
				RaycastHit hitGround = new RaycastHit ();
				float closest = float.MaxValue;
				float closestLadder = float.MaxValue;

				RaycastHit[] feetCollisions = feetCollider.GetAllCollision (1 << backgroundLayer | (fallThroughTimer <= 0.0f ? 1 << passThroughLayer : 0) | (climbing.allowClimbing && fallThroughTimer <= 0.0f ? 1 << climableLayer : 0), slopes.slopeLookAhead);						
				// Get closest collision
				foreach (RaycastHit collision in feetCollisions) {
					// If we got a ladder also keep reference to ground
					if (collision.collider != null && collision.collider.gameObject.layer == climableLayer) {
						if (collision.distance < closestLadder) {
							hitLadder = collision;	
							closestLadder = collision.distance;
						}
					} else if (collision.distance < closest) {
						hitFeet = collision;
						closest = collision.distance;
					}
				}
				
				// If ladder is closest collider
				if (hitLadder.collider != null && hitFeet.collider != null && hitLadder.distance < closest) {
					// Only assign ground if its a true hit, not a slope look ahead hit
					if (hitFeet.distance <= feetCollider.distance && hitFeet.collider.gameObject.layer != climableLayer) {
						hitGround = hitFeet;
						hasHitFeet = true;
					}
					hitFeet = hitLadder;
				}
				
				// If only hitting a ladder
				if (hitLadder.collider != null && hitFeet.collider == null) {
					hitFeet = hitLadder;	
				}
				
				float force = (hitFeet.normal * (feetCollider.distance - hitFeet.distance)).y;	
				// Standing on a something that has an action when you stand on it
				if (hitFeet.collider != null) {
					Platform platform = hitFeet.collider.gameObject.GetComponent<Platform> ();
					if (platform != null && feetCollider.distance >= hitFeet.distance) {
						platform.DoAction (feetCollider, this);
						Transform parentPlatform = platform.ParentOnStand (this);
						if (parentPlatform != null) {
							// Normal parenting (moving platforms etc)
							myParent = platform;
							if (myTransform.parent != parentPlatform) {
								myTransform.parent = parentPlatform;
							}
							hitGameObject = hitFeet.collider.gameObject;
						}
						// Special case for the top of a ladder
						if (platform is TopStepPlatform) {
							hasHitFeet = true;
							maxForce = force;
							hitGameObject = hitLadder.collider.gameObject;
						}
					}
					// Climbing 
					if (climbing.allowClimbing && hitLadder.collider != null && hitLadder.distance <= feetCollider.distance && jumpButtonTimer <= 0.0f) {
						//bool ladder = hitLadder.collider.gameObject.GetComponent<Ladder>();
						//if (ladder == null) hitLadder.collider.gameObject.GetComponent<Rope>(); // && ladder != null
						climbCount++;
						if (!(myParent is TopStepPlatform) && (startedClimbing || (climbing.autoStick && hitGround.collider == null && (dismounting == null || (myParent != null && dismounting != ((Ladder)myParent).control)) || characterInputY != 0))) {
							if (climbCount >= climbing.collidersRequired) {
								startedClimbing = true;
								isClimbing = true;
								hasHitFeet = true;
								dismounting = null;
								// Allow platforms to stop us climbing
								if (myParent == null || !myParent.overrideY) {
									if (characterInputY > 0.0f) {
										// Ensure we dont go above step
										maxForce = climbing.speed * frameTime;
										if (maxForce > force)
											maxForce = force;
										isClimbingUpOrDown = true;
									} else if (!cantClimbDown && characterInputY < 0.0f) {
										maxForce = climbing.speed * frameTime * -1;
										// Ensure we don't go into ground
										isClimbingUpOrDown = true;
										if (hitGround.collider != null) {
											float groundForce = (hitGround.normal * (feetCollider.distance - hitGround.distance)).y;		
											if (maxForce < groundForce)
												maxForce = groundForce;
												isClimbingUpOrDown = false;
												startedClimbing = false;
										}
										
									}
								}
							}
						}
					} else {
						// Calculate slope
						if (slopes.allowSlopes && hitFeet.collider.gameObject.layer != climableLayer) {
							if (lastHitDistance < 0.0f) {
								lastHitDistance = hitFeet.distance;
								lastHitX = feetCollider.offset.x;
								if (slopeCount == -1)
									slopeCount = 0;
							} else {
								slope += Mathf.Atan ((lastHitDistance - hitFeet.distance) / (feetCollider.offset.x - lastHitX)) * Mathf.Rad2Deg;
								slopeCount++;
								lastHitDistance = hitFeet.distance;
								lastHitX = feetCollider.offset.x;
							}
						}
					}
					
					// If we are hitting our feet on the ground we can't climb down a ladder
					if (hitLadder.collider == null && characterInputY < 0.0f && hitFeet.distance <= feetCollider.distance) {
						cantClimbDown = true;
						maxForce = 0.0f;
					}
					// Get force to apply		
					if (force > maxForce && hitLadder.collider == null) {
						// We hit a blocker stop all climbing
						cantClimbDown = true;
						isClimbingUpOrDown = false;
						isClimbing = false;
						startedClimbing = false;
						hasHitFeet = true;
						maxForce = force;
						hitGameObject = hitFeet.collider.gameObject;
					}

					if (hasHitFeet && hitLadder.collider == null) groundedFeet++;
				}
			}
			if (startedClimbing && climbCount < climbing.collidersRequired) {
				startedClimbing = false;
			}
			
			if (hasHitFeet) {
				dismounting = null;
				if ((myParent == null || !myParent.overrideY)) {
					if ((myParent is LadderCollider && ((LadderCollider)myParent).control.ShouldPlayLedgeClimb(this))) {
						if (characterInput.y < 0.0f && myParent is TopStepPlatform && climbCount >= climbing.collidersRequired && groundedFeet == 0) {
							isLadderTopClimbing = true; 
							State = CharacterState.CLIMB_TOP_OF_LADDER_DOWN;
							ladderTopClimbState = LadderTopState.CLIMBING_DOWN;
						} else if (characterInput.y > 0.0f && !(myParent is TopStepPlatform)) {
							isLadderTopClimbing = true; 
							ladderTopClimbState = LadderTopState.CLIMBING_UP;
							// Force position to be exactly right
							myTransform.position = new Vector3(myTransform.position.x, ((LadderCollider)myParent).LedgeClimbHeight ,myTransform.position.z);
						} else {
							myTransform.Translate (0.0f, maxForce, 0.0f, Space.World);	
						}
					} else {
						myTransform.Translate (0.0f, maxForce, 0.0f, Space.World);	
					}
				}
				velocity.y = 0.0f;
				if (myParent != null && hitGameObject != myParent.gameObject && !(myParent is Rope)) {
					Unparent();
				}
				grounded = true;
				fallingTime = 0.0f;
			} else {
				if (myParent == null) {
					ApplyGravity ();
				} else if (!grounded) {
					ApplyGravity ();
					Unparent();
					startedClimbing = false;
				}	
			}
		} else {
			ApplyGravity ();	
		}
		// Force dismount if we dont have enough colliders on the ladder		
		if (climbCount < climbing.collidersRequired && myParent is Ladder) {
			Dismount ((Ladder)myParent);
		}

		// Stop crouch when climbing
		if (isClimbing) StopCrouch();

		// Apply rotation from slopes
		if (slopes.allowSlopes) {
			float actualSlope = (myTransform.localEulerAngles.z % 360.0f) + (slope / (float)slopeCount);
			if (slopeCount > 0 && !isClimbing && (!(actualSlope > slopes.maxRotation && actualSlope < 360.0f - slopes.maxRotation))) {
				myTransform.Rotate (0.0f, 0.0f, slopes.rotationSpeed * (slope / (float)slopeCount));
			} else if (slopeCount == -1 || isClimbing) {
				myTransform.localRotation = Quaternion.RotateTowards (myTransform.localRotation, Quaternion.identity, slopes.rotationSpeed * 10.0f);
			}
			if ((actualSlope > slopes.maxRotation && actualSlope < 360.0f - slopes.maxRotation)) {
				myTransform.localRotation = Quaternion.Euler (0, 0, 0);
			}
		}
		
		// Hitting Head
		if (velocity.y > 0.0f || isClimbing || (myParent != null && myParent.velocity.y > 0.0f)) {
			float maxForce = 0.0f;
			foreach (RaycastCollider headCollider in headColliders) {
				RaycastHit hitHead = headCollider.GetCollision (1 << backgroundLayer);
				float force = (hitHead.normal * (headCollider.distance - hitHead.distance)).y;
				if (hitHead.collider != null) {
					// Action on headbut
					Platform platform = hitHead.collider.gameObject.GetComponent<Platform> ();
					if (platform != null) {
						platform.DoAction (headCollider, this);
					}
					if (force < -1 * movement.skinSize && force < maxForce) {
						hasHitHead = true;
						maxForce = force;
					}
				}
			}
			
			if (hasHitHead) {
				jumpHeldTimer = jump.jumpHeldTime;
				myTransform.Translate (0.0f, maxForce, 0.0f, Space.World);		
				if (velocity.y > 0.0f)
					velocity.y = 0.0f;
			}
			if (!isClimbing) {
				ApplyGravity ();
			} 
		}
		
		// Jump
		if (!hasHitHead || isClimbing) {
			if (characterInput.jumpButtonDown) {
				if ((grounded || myParent != null) && jumpCount == 0 && jumpButtonTimer <= 0.0f) {
					// Special case when launching from a rope
					if (myParent != null && myParent is Rope) {
						float k = Mathf.Abs (Mathf.Cos (Mathf.Deg2Rad * myTransform.rotation.eulerAngles.z));
						k = Mathf.Pow (k, ((Rope)myParent).control.jumpFlattenFactor);
						velocity = new Vector2 ((myParent.velocity.x + myParent.velocity.y) * ((Rope)myParent).control.ropeVelocityFactor, 
						                        jump.jumpVelocity * k);
						jumpCount = 2;
					} else {
						velocity.y = jump.jumpVelocity;
						jumpCount = 1;
					}
					Unparent();
					startedClimbing = false;
					jumpButtonTimer = jump.jumpTimer;
					jumpHeldTimer = 0.0f;
					StopCrouch();
					State = CharacterState.JUMPING;
				} else if (jumpCount == 1 && jump.canDoubleJump) {
					Unparent();
					startedClimbing = false;
					jumpCount++;
					velocity.y = jump.doubleJumpVelocity;
					StopCrouch();
					State = CharacterState.DOUBLE_JUMPING;
				}
			} else if (characterInput.jumpButtonHeld && jumpHeldTimer < jump.jumpHeldTime && jumpCount == 1) {
				velocity.y += jump.jumpFrameVelocity * Time.deltaTime * (jump.jumpHeldTime - jumpHeldTimer);
				jumpHeldTimer += Time.deltaTime;
			}
		}
		if (jumpButtonTimer > 0.0f)
			jumpButtonTimer -= frameTime;
		if (jumpButtonTimer <= 0.0f && grounded) {
			jumpCount = 0;	
		}
		
		// Reset held button timer if we release button
		if (!characterInput.jumpButtonHeld) {
			jumpHeldTimer = jump.jumpHeldTime;
		}
		
		// Wall jump
		if (wallJumpTimer > 0) {
			wallJumpTimer -= frameTime;	
		}
		
		if (wall.canWallJump) {
			// Easy wall jump
			if (wall.easyWallJump) {
				if (characterInput.jumpButtonDown && isWallHolding) {
					Unparent();
					startedClimbing = false;
					velocity.y = jump.jumpVelocity;
					jumpCount = 2;
					wallJumpTimer = 0.0f;
					jumpButtonTimer = jump.jumpTimer;
					jumpHeldTimer = 0.0f;
					StopCrouch();
					State = CharacterState.WALL_JUMPING;
					if (wall.wallJumpOnlyInOppositeDirection) {
						oppositeDirectionTimer = wall.oppositeDirectionTime;
						if (wallJumpDirection == RC_Direction.LEFT)  velocity.x = movement.walkSpeed;
						else if (wallJumpDirection == RC_Direction.RIGHT)  velocity.x =  -1 * movement.walkSpeed;
					}
				}
			}
			// "Hard" wall jump also works for easy wall jump
			if (wallJumpTimer > 0.0f) {
				if ((hasPressedJumpForWallJump && ((wallJumpDirection == RC_Direction.LEFT && characterInput.x > 0) || (wallJumpDirection == RC_Direction.RIGHT && characterInput.x < 0)))
				    
				    || (hasPressedDirectionForWallJump && characterInput.jumpButtonDown)) {
					Unparent();
					startedClimbing = false;
					velocity.y = jump.jumpVelocity;
					jumpCount = 2;
					wallJumpTimer = 0.0f;
					jumpButtonTimer = jump.jumpTimer;
					jumpHeldTimer = 0.0f;
					StopCrouch();
					State = CharacterState.WALL_JUMPING;
					if (wall.wallJumpOnlyInOppositeDirection) {
						oppositeDirectionTimer = wall.oppositeDirectionTime;
						if (wallJumpDirection == RC_Direction.LEFT)  velocity.x = movement.walkSpeed;
						else if (wallJumpDirection == RC_Direction.RIGHT)  velocity.x =  -1 * movement.walkSpeed;
					}
				} else if (!hasPressedJumpForWallJump && characterInput.jumpButtonDown ) {
					hasPressedJumpForWallJump = true;
				} else if (!hasPressedDirectionForWallJump && ((wallJumpDirection == RC_Direction.LEFT && characterInput.x > 0) || (wallJumpDirection == RC_Direction.RIGHT && characterInput.x < 0))) {
					hasPressedDirectionForWallJump = true;
				}
			} else {
				hasPressedJumpForWallJump = false;
				hasPressedDirectionForWallJump = false;
			}
		}
		
		if (myParent != null && myParent.overrideAnimation) {
			State = myParent.GetAnimationState(this);
		} else {
			// Animations
			if ((!IsGrounded (groundedLookAhead, false) && !hasHitFeet) || jumpButtonTimer > 0.0f) {
				State = CharacterState.AIRBORNE;
			}
			
			if (velocity.y < jump.fallVelocity) {
				State = CharacterState.FALLING;
			}
			
			if (startedClimbing){
				if (isClimbingUpOrDown) {
					State = CharacterState.CLIMBING;	
				} else {
					State = CharacterState.HOLDING;	
				}
			}
		}
	}
	
	public void DoCrouch() {
		bool hasHitHead = false;
		bool inTheAir = !(jumpCount == 0 && State != CharacterState.FALLING && State != CharacterState.AIRBORNE && State != CharacterState.JUMPING);
		// If we are sliding we need to check if we would hit head were we to stand, if we would
		// we can't uncrouch nor reduce slide velocity
		if (isCrouchSliding) {
			foreach (RaycastCollider headCollider in headColliders) {
				float additionalDistance = 0 ;
				if (crouch.useHeightReduction) additionalDistance = crouch.heightReductionFactor * headCollider.distance;
				else additionalDistance = crouch.headDetectionDistance;
				RaycastHit hitHead = headCollider.GetCollision (1 << backgroundLayer, additionalDistance);
				//float force = (hitHead.normal * (headCollider.distance - hitHead.distance)).y;
				if (hitHead.collider != null) {
					hasHitHead = true;
					break;
				}
			}
		}

		// if not crouching
		if (!isCrouching) {
			if (characterInput.y < 0 && (crouch.canCrouchJump || !inTheAir)) {
				StartCrouch();
			}
		}
		// If already crouching
		else {
			if (characterInput.y >= 0 && !hasHitHead) {
				StopCrouch ();
			}
		}
		
		if (isCrouching) {
			// Check for crouch slide
			if (crouch.canCrouchSlide && (isCrouchSliding && (velocity.x > crouch.minVelocityForStopSlide || velocity.x < -1 * crouch.minVelocityForStopSlide)||
										 velocity.x > crouch.minVelocityForSlide || velocity.x < -1 * crouch.minVelocityForSlide)) {
				isCrouchSliding = true;
			
				if (hasHitHead) {
					crouchSlideDrag = 0;
				} else {
					crouchSlideDrag = crouch.slideDrag;
				}
				State = CharacterState.CROUCH_SLIDING;
			} else {
				// If we aren't allowed to slide when crouching or if going to slow
				// then crouching freezes x movement unless we ar ein the air
				if (!inTheAir) velocity.x = 0.0f;
				State = CharacterState.CROUCHING;
			}
		}
	}

	private void StartCrouch() {
		// Started crouching
		if (!isCrouching && crouch.useHeightReduction) {
			foreach(RaycastCollider c in headColliders) {
				c.distance = c.distance / crouch.heightReductionFactor;
				c.offset.y -= (c.distance * 0.5f);
			}
		}
		isCrouching = true;
	}

	private void StopCrouch () {
		// Stop crouching
		if (isCrouching && crouch.useHeightReduction)  {
			foreach(RaycastCollider c in headColliders) {
				c.offset.y += (c.distance * 0.5f);
				c.distance = c.distance * crouch.heightReductionFactor;
			}
		}
		isCrouching = false;
		isCrouchSliding = false;
	}
	
	/// <summary>
	/// Controls movement while hanging from a ledge.
	/// </summary>
	private void DoLedgeHang () {
		switch (ledgeHangState) {
		case LedgeHangingState.TRANSITION:
			velocity.y = 0.0f;
			ledgeHangTimer += frameTime;
			State = CharacterState.LEDGE_HANGING;
			myTransform.position = Vector3.Slerp (ledgeHangOriginalPosition, ledgeHangGoalPosition, ledgeHangTimer / ledgeHanging.transitionTime);
			
			if (ledgeHangTimer > ledgeHanging.transitionTime) {
				ledgeHangTimer = 0.0f;
				ledgeHangState = LedgeHangingState.HANG;
				myTransform.position = ledgeHangGoalPosition;
			}
			break;
		case LedgeHangingState.HANG:
			// Fall Off
			if (characterInput.x > 0.0f && ledgeHangDirection == RC_Direction.LEFT) {
				isLedgeHanging = false;
				ledgeDropTimer = ledgeHanging.ledgeDropTime;
				State = CharacterState.FALLING;
			} else if (characterInput.x < 0.0f && ledgeHangDirection == RC_Direction.RIGHT) {
				isLedgeHanging = false;
				ledgeDropTimer = ledgeHanging.ledgeDropTime;
				State = CharacterState.FALLING;
			}// Drop Off
			else if (characterInput.y < 0.0f) {
				isLedgeHanging = false;
				ledgeDropTimer = ledgeHanging.ledgeDropTime;
				State = CharacterState.FALLING;
			} 
			// Jump Off
			else if (characterInput.jumpButtonDown) {
				if (ledgeHanging.jumpOnlyInOppositeDirection) {
					oppositeDirectionTimer = ledgeHanging.oppositeDirectionTime;
					if (ledgeHangDirection == RC_Direction.LEFT)
						velocity.x = movement.walkSpeed;
					else if (ledgeHangDirection == RC_Direction.RIGHT)
						velocity.x = -1 * movement.walkSpeed;
				}
				isLedgeHanging = false;
				velocity.y = ledgeHanging.jumpVelocity;
				jumpCount = 2;
				Unparent();
				startedClimbing = false;
				jumpButtonTimer = jump.jumpTimer;
				jumpHeldTimer = 0.0f;
				ledgeDropTimer = ledgeHanging.ledgeDropTime;
				if (ledgeHanging.jumpVelocity > 0.0f) {
					State = CharacterState.JUMPING;
				} else {
					State = CharacterState.FALLING;
				}
			}
			// Climb
			else if (characterInput.y > 0.0f) {
				ledgeHangState = LedgeHangingState.CLIMBING;
				ledgeHangTimer = 0.0f;
			}
			break;
		case LedgeHangingState.CLIMBING:
			ledgeHangTimer += frameTime;
			if (ledgeHangTimer > ledgeHanging.climbTime) {
				ledgeHangTimer = 0.0f;
				State = CharacterState.LEDGE_CLIMB_FINISHED;
				ledgeHangState = LedgeHangingState.FINISHED;
			} else {
				State = CharacterState.LEDGE_CLIMBING;
			}
			break;
		case LedgeHangingState.FINISHED: 
			isLedgeHanging = false;
			if (ledgeHangDirection == RC_Direction.RIGHT) {
				myTransform.Translate (ledgeHanging.climbOffset);
			} else if (ledgeHangDirection == RC_Direction.LEFT) {
				myTransform.Translate (new Vector3(-1 * ledgeHanging.climbOffset.x, ledgeHanging.climbOffset.y, ledgeHanging.climbOffset.z));
			}
			State = CharacterState.IDLE;
			break;
		}
	}
	
	/// <summary>
	/// Controls movement while climbing a ladder top
	/// </summary>
	private void DoLadderTop ()
		{
			switch (ladderTopClimbState) {
		
				case LadderTopState.CLIMBING_UP:
					ledgeHangTimer += frameTime;
					if (ledgeHangTimer > climbing.climbTopAnimationTime) {
						ledgeHangTimer = 0.0f;
						State = CharacterState.IDLE;
						ladderTopClimbState = LadderTopState.FINISHED_UP;
					} else {
						State = CharacterState.CLIMB_TOP_OF_LADDER_UP;
					}
					break;
				case LadderTopState.FINISHED_UP: 
					isLadderTopClimbing = false;
					myTransform.Translate (0.0f, climbing.climbOffset.y, 0.0f);
					startedClimbing = false;
					State = CharacterState.IDLE;
					Unparent();
					break;
				case LadderTopState.CLIMBING_DOWN:
					myTransform.Translate (0.0f, -1 * climbing.climbOffset.y, 0.0f);
					State = CharacterState.CLIMB_TOP_OF_LADDER_DOWN;
					ladderTopClimbState = LadderTopState.CLIMBING_DOWN_ACTION;
					break;
				case LadderTopState.CLIMBING_DOWN_ACTION:
					ledgeHangTimer += frameTime;
					if (ledgeHangTimer > climbing.climbTopDownAnimationTime) {
						ledgeHangTimer = 0.0f;
						State = CharacterState.CLIMB_TOP_OF_LADDER_DOWN;
						ladderTopClimbState = LadderTopState.CLIMBING_DOWN_PAUSE;
					} else {
						State = CharacterState.CLIMB_TOP_OF_LADDER_DOWN;
					}
					break;
				case LadderTopState.CLIMBING_DOWN_PAUSE:
					// TODO This small delay helps smooth out the characters downwards climb but its not a pleasant solution, fix it!
					ledgeHangTimer += frameTime;		
					if (ledgeHangTimer > 0.25f) {
						ledgeHangTimer = 0.0f;
						State = CharacterState.CLIMBING;
						ladderTopClimbState = LadderTopState.FINISHED_DOWN;
						
					} else {
						State = CharacterState.CLIMBING;
						
					}
					break;
				case LadderTopState.FINISHED_DOWN: 
					isLadderTopClimbing = false;
					// Nudge down a bit more TODO Does this need a variable?
					startedClimbing = true;
					State = CharacterState.CLIMBING;
					Unparent () ;
					break;
			}
	}
	
	/// <summary>
	/// Unparent from platform.
	/// </summary>
	private void Unparent () {
		myParent = null;
		myTransform.parent = null;
	}

	private void ApplyGravity ()
	{
		if (wall.canWallSlide && isWallSliding) {
			if (velocity.y == movement.terminalVelocity * wall.wallSlideGravityFactor) {
				
			} else if (velocity.y > movement.terminalVelocity * wall.wallSlideGravityFactor) {
				// Slow down
				velocity.y += (frameTime * Physics.gravity.y);
				if (velocity.y < movement.terminalVelocity * wall.wallSlideGravityFactor) velocity.y = movement.terminalVelocity * wall.wallSlideGravityFactor;
			} else {	
				// Speed up
				velocity.y -= (frameTime * Physics.gravity.y);
				if (velocity.y > movement.terminalVelocity * wall.wallSlideGravityFactor) velocity.y = movement.terminalVelocity * wall.wallSlideGravityFactor;
			}
		} else {
			velocity.y += (frameTime * Physics.gravity.y);
		}
	}
	
}	

/// <summary>
/// Raycast Directions.
/// </summary>
public enum RC_Direction {UP, DOWN, LEFT, RIGHT};

/// <summary>
/// Movement details.
/// </summary>
[System.Serializable]
public class MovementDetails {
	/// <summary>
	/// Speed the character walks at.
	/// </summary>
	public float walkSpeed = 3.0f;
	/// <summary>
	/// Speed the chracter runs at.
	/// </summary>
	public float runSpeed = 5.0f;
	/// <summary>
	/// The acceleration to apply when input is left or right.
	/// </summary>
	public float acceleration = 75.0f;	
	/// <summary>
	/// The drag to apply each frame.
	/// </summary>
	public float drag = 1.15f;	
	/// <summary>
	/// The terminal velocity in the y direction. 
	/// WARNING: If this is too large you will be able to fall through platforms. Make sure maxFrameTime * terminalVelcoty < feetCollider.distance.
	/// </summary>
	public float terminalVelocity = -12.0f;
	/// <summary>
	/// Minimum movement distance, used to stop shaking.
	/// </summary>
	public float skinSize = 0.001f;
}

/// <summary>
/// Slope details.
/// </summary>
[System.Serializable]
public class SlopeDetails {
	/// <summary>
	/// Set to true if the character can handle slopes.
	/// </summary>
	public bool allowSlopes = false;
	/// <summary>
	/// How far to look ahead when determinin gif the character is on a slope. Large values will make the character tilt in the air.
	/// </summary>
	public float slopeLookAhead = 0.5f;
	/// <summary>
	/// How fast to rotate to the sloped position (and back again)
	/// </summary>
	public float rotationSpeed = 0.25f;
	/// <summary>
	/// The maximum permissable rotation in degrees.
	/// </summary>
	public float maxRotation = 45f;
}

/// <summary>
/// Jump details. Note that jumps are also affected by the Physics.gravity setting.
/// </summary>
[System.Serializable]
public class JumpDetails {
	/// <summary>
	/// Set to true to enable double jump.
	/// </summary>
	public bool canDoubleJump = false;
	/// <summary>
	/// Set to true to enable wall jumps.
	/// </summary>
	/// <summary>
	/// How fast the jump is.
	/// </summary>
	public float jumpVelocity = 10.0f;	
	/// <summary>
	/// How fast the doublejump is.
	/// </summary>	
	public float doubleJumpVelocity = 8.0f;	
	/// <summary>
	/// Controls how long you are considered to be jumping. If this is too small you wont be able
	/// to jump away from climables. If its too large you wont be able to quickly jump twice in a row.
	/// </summary>
	public float jumpTimer = 0.2f;
	/// <summary>
	/// How long after pressing jump it can be held down to add extra force. use this if you want jumps to
	/// get bigger when you hold jump.
	/// </summary>
	public float jumpHeldTime = 0.25f;
	/// <summary>
	/// How much extra acceleration is added when you hold the jump button down. Set to zero for fixed height jumps.
	/// </summary>
	public float jumpFrameVelocity = 25.0f;
	/// <summary>
	/// The amount of drag in the air. When you jump you move/change direction at walk speed. By setting the drag 
	/// low you will get extra distance when you do a running jump.
	/// </summary>
	public float drag = 1.005f;
	/// <summary>
	/// The velocity required before the fall event replaces the airbourne event (negative number).
	/// </summary>
	public float fallVelocity = -1.0f;
}

/// <summary>
/// Wall details. Controls wall jump and wall slide.
/// </summary>
[System.Serializable]
public class WallDetails {
	public bool canWallJump = false;
	/// <summary>
	/// Can the character slide down the wall 
	/// </summary>
	public bool canWallSlide = false;
	/// <summary>
	/// If true, character needs to jump while holding against a wall to wall jump. Otherwise they need
	/// to also time it so that they hold against the wall and then hit the opposite direction as they jump.
	/// </summary>
	public bool easyWallJump = true;
	/// <summary>
	/// If true when wall jumping you always jump away from wall. If false you can also jump directly upwards.
	/// Only has an effect when easyWallJump is enabled as if easyWallJump = false, you must hold away and therefore
	/// jump away
	/// from wall.
	/// </summary>
	public bool wallJumpOnlyInOppositeDirection = true;
	/// <summary>
	/// The reduciton in gravity when the character is wall sliding (only used if wallSlide = true). Set to
	/// 0 to make it stick.
	/// </summary>
	public float wallSlideGravityFactor = 0.33f;
	/// <summary>
	/// The time where user input is ingored in the x direction when wall jupming away from a wall.
	/// </summary>
	public float oppositeDirectionTime = 0.5f;
	/// <summary>
	/// Additional distance to add to side collider hit checks when determining if we are wall sliding.
	/// </summary>
	public float wallSlideAdditionalDistance = 0.05f;
	/// <summary>
	/// Amount off leeway between pushing against a wall, and being able to wall jump 
	/// by holding opposite direction and pressing jump.
	/// </summary>
	public float wallJumpTime = 0.33f;
	/// <summary>
	/// The offset for the extra raycast used for edge detection.
	/// Note the y value is the y offset but that the x value is used as a scalar
	/// distance (i.e. changes based on direction).
	/// </summary>
	public Vector2 edgeDetectionOffset;
}

/// <summary>
/// Climb details.
/// </summary>
[System.Serializable]
public class ClimbDetails {
	/// <summary>
	/// If true you will autoamtically stick to climables when you touch them. Otherwise you 
	/// will need to press up or down to stick.
	/// </summary>
	public bool autoStick = false;
	/// <summary>
	/// ALlow climbing if true.
	/// </summary>
	public bool allowClimbing = true;
	/// <summary>
	/// The vertical speed at which ladders are climbed.
	/// </summary> 
	public float speed = 2.5f;

	/// <summary>
	/// If the ladder allows climbing horizontally use this speed.
	/// </summary> 
	public float horizontalSpeed = 1.5f;

	/// <summary>
	/// How many feet colliders are required to be on the ladder before it can be climbed. cannot be larger
	/// than total eet colliders. Larger numbers make ladders "thinner", i.e. you have to be closer to the
	/// centre of the ladder to climb them.
	/// </summary> 
	public int collidersRequired = 3;
	
	/// <summary>
	/// How much to accentuate the x velocity of the rope when you launch off of it.
	/// </summary>
	public float ropeVelocityFactor = 1.33f;
	/// <summary>
	/// How much force to impart to the rope when swinging.
	/// </summary>
	public float ropeSwingForce = 1.5f;
	
	/// <summary>
	/// The time it takes to play the climbing the ladder top animation
	/// </summary>
	public float climbTopAnimationTime = 1.5f;

	/// <summary>
	/// The time it takes to play the climbing the ladder top downanimation
	/// </summary>
	public float climbTopDownAnimationTime = 1.5f;

	/// <summary>
	/// The difference between the characters root position
	/// at the top of a climb, and during the normal state (idle, walking, etc).
	/// </summary>
	public Vector3 climbOffset;
}

/// <summary>
/// Ledge details. Control what happens when you hang off a ledge.
/// </summary>
[System.Serializable]
public class LedgeDetails {
	/// <summary>
	/// Can character hang and climb ledge.
	/// </summary>
	public bool canLedgeHang = true;
	
	/// <summary>
	/// If true the character will try to grab ledges near them without user input.
	/// If false the user needs to hold towards the ledge to grab.
	/// </summary>
	public bool autoGrab;
	
	/// <summary>
	/// The distance used to calcualte autograbs, should be very small or
	/// else the character will grab ledges when they shouldn't be able 
	/// to.
	/// </summary>
	public float autoGrabDistance = 0.1f;
	
	/// <summary>
	/// When a ledge hang starts the edge detection offset is added to the ledge hang raycast 
	/// and if an impassable object is detected it will cancel the hang. 
	/// Note the y value is the y offset but that the x value is used as a scalar
	/// distance (i.e. changes based on direction).
	/// </summary>
	public Vector2 edgeDetectionOffset;
	
	/// <summary>
	/// If false the character can jump directly upwards from a ledge hang. Otherwise
	// they can jump only jump only in the opposite direction to which they are holding.
	/// </summary>
	public bool jumpOnlyInOppositeDirection = true;
	
	/// <summary>
	/// The time where user input is ingored in the x direction when wall jupming away from a ledge.
	/// </summary>
	public float oppositeDirectionTime = 0.5f;
	
	/// <summary>
	/// Velocity imparted by a jump from a ledge hang. If this is zero jump
	/// will simply cause the cahracter to fall from the ledge (and the jump
	/// animation will not be played).
	/// </summary>
	public float jumpVelocity = 7.0f;
	
	/// <summary>
	/// The point where the character hangs from.
	/// </summary>
	public Vector3 hangOffset;
	
	/// <summary>
	/// How far above the highest collider is the grasp point (i.e. the cahracters
	/// hands when grasping for a ledge).
	/// </summary>
	public float graspPoint = 0.4f;
	
	/// <summary>
	/// How much leeway to use when cacluating the grasp. A bigger number
	/// makes it easier to ledge hang, but it will looks incorrect
	/// if the value is too large.
	/// </summary>
	public float graspLeeway = 0.1f;
	
	/// <summary>
	/// The time it takes to get to the hang position in seconds.
	/// </summary>
	public float transitionTime;
	
	/// <summary>
	/// The time it takes to climb in seconds.
	/// </summary>
	public float climbTime;
	
	/// <summary>
	/// The difference between the characters root position
	/// at the top of a climb, and during the normal state (idle, walking, etc).
	/// </summary>
	public Vector3 climbOffset;
	
	/// <summary>
	/// The time after dropping off a ledge where feet colliders will be ignored.The lower the better
	/// but you may need to increase if you have tiled or complex geometry.
	/// </summary>
	public float ledgeDropTime = 0.15f;
	
}

/// <summary>
/// Crouch details. Control what happens when you press down.
/// </summary>
[System.Serializable]
public class CrouchDetails {
	/// <summary>
	/// Can character crouch.
	/// </summary>
	public bool canCrouch = true;

	/// <summary>
	/// Can character slide along in a crouch position.
	/// </summary>
	public bool canCrouchSlide = true;

	/// <summary>
	/// If true the character can maintain a crouch position while jumping.
	/// </summary>
	public bool canCrouchJump = false;

	/// <summary>
	/// How fast character needs to be moving in the x direction
	/// before a crouch slide is initiated.
	/// </summary>
	public float minVelocityForSlide = 3.0f;
	
	/// <summary>
	/// Once crouch sliding how slow does the character need to be going for the 
	/// slide to stop.
	/// </summary>
	public float minVelocityForStopSlide = 1.75f;

	/// <summary>
	///  How much drag is applied when crouch sliding.
	/// </summary>
	public float slideDrag = 0.6f;

	/// <summary>
	/// If true we will automatically shrink the characters head colliders by the
	/// the height reduction factor. The alternative is to attach the head colliders
	/// to transforms, or listen for events and handle it compeltely in your own code.
	/// </summary>
	public bool useHeightReduction;

	/// <summary>
	/// If use heightRecution = true then this is amount the characters head colliders 
	/// will be reduced by. Specifically the head collider distance is scaled by this factor
	/// and then the offset downwards by (0.5f * new distance)
	/// </summary>
	public float heightReductionFactor = 2f;

	/// <summary>
	/// If use heightRecution = true then any side collider higher (determined by its
	/// yOffset only) than this value will be ignored while crouching.
	/// </summary>
	public float ignoredSideCollidersHigherThan = 0f;
	
	/// <summary>
	/// If you are not usinng the automatic height recution (i.e. heightReduction = false),
	/// then this value will be added to your head collider distance when determining if
	/// character is able to stand up. This is used to prevent the character standing up while
	/// sliding under a platform.
	/// </summary>
	public float headDetectionDistance;
}

public enum StunType {
	// Stop the player inputs but still allow character to move
	STOP_INPUT_ONLY, 
	// Stop the player inputs and X movement, but still allow character to rise/fall
	STOP_INPUT_AND_X_MOVEMENT, 
	// Stop the player inputs and Y movement, but still allow character to slide
	STOP_INPUT_AND_Y_MOVEMENT, 
	// Stop the player inputs and all movement
	STOP_INPUT_AND_ALL_MOVEMENT
}

/// <summary>
/// Raycast collider. The main collision object.
/// </summary>
[System.Serializable]
public class RaycastCollider {
	/// <summary>
	/// The transform to use when calculating this collision. By defaults it s the mian transform
	/// This is the easiest way to change the shape of your character. For example you can attach to
	/// a bone, or transform a collider up or down if your character is ducking/standing.
	/// </summary>
	public Transform transform;
	/// <summary>
	/// The offset from the centre of the transform.
	/// </summary>
	public Vector3 offset;
	/// <summary>
	/// The distance of the collider. Longer colliders tend to give better behaviour.
	/// </summary> 
	public float distance = 1;	
	/// <summary>
	/// The direction of the collider.
	/// </summary>
	public RC_Direction direction;
	
	private Vector3 staticOffset;	
	
	public bool IsColliding() {
		return Physics.Raycast (transform.position + transform.localRotation * offset, transform.localRotation * GetVectorForDirection(), distance);
	}
	
	public bool IsColliding(int layerMask) {
		return Physics.Raycast (transform.position + transform.localRotation * offset, transform.localRotation * GetVectorForDirection(), distance, layerMask);
	}
	
	public bool IsColliding(int layerMask, float extraDistance, float yOffset) {
		return Physics.Raycast (transform.position + transform.localRotation * new Vector3(offset.x, offset.y + yOffset, offset.z), 
		                        transform.localRotation * GetVectorForDirection(), distance + extraDistance, layerMask);
	}
	
	public bool IsColliding(int layerMask, float skinSize) {
		return Physics.Raycast (transform.position + transform.localRotation * offset, transform.localRotation * GetVectorForDirection(), distance + skinSize, layerMask);
	}
	
	public RaycastHit GetCollision() {
		RaycastHit hit;
		Physics.Raycast (transform.position + transform.localRotation * offset, transform.localRotation * GetVectorForDirection(), out hit, distance);
		return hit;	
	}
	
	public RaycastHit GetCollision(int layerMask) {
		RaycastHit hit;
		Physics.Raycast (transform.position + transform.localRotation * offset, transform.localRotation * GetVectorForDirection(), out hit, distance, layerMask);
		return hit;	
	}
	
	public RaycastHit GetCollision(int layerMask, float extraDistance) {
		RaycastHit hit;
		Physics.Raycast (transform.position + transform.localRotation * offset, transform.localRotation * GetVectorForDirection(), out hit, distance + extraDistance, layerMask);
		return hit;	
	}
	
	public RaycastHit[] GetAllCollision(int layerMask) {
		return Physics.RaycastAll (transform.position + transform.localRotation * offset, transform.localRotation * GetVectorForDirection(), distance, layerMask);
	}
	
	public RaycastHit[] GetAllCollision(int layerMask, float extraDistance) {
		return Physics.RaycastAll (transform.position + transform.localRotation * offset, transform.localRotation * GetVectorForDirection(), distance + extraDistance, layerMask);
	}
	
	public void DrawRayCast ()
	{
		if (transform != null) {
			switch (direction) {
			case RC_Direction.DOWN: Gizmos.color = Color.green; break;
			case RC_Direction.RIGHT: Gizmos.color = Color.red;  break;
			case RC_Direction.LEFT: Gizmos.color = Color.yellow;break;
			case RC_Direction.UP: Gizmos.color = Color.magenta; break;
			}
			Vector3 position = transform.position + transform.localRotation * offset;
			
			Gizmos.DrawLine (position, position + ((transform.localRotation * GetVectorForDirection()) * distance));
		}
	}
	
	public Vector3 GetVectorForDirection(){
		switch (direction) {
		case RC_Direction.DOWN: return Vector3.up * -1;
		case RC_Direction.RIGHT: return Vector3.right;
		case RC_Direction.LEFT: return Vector3.right * -1;
		case RC_Direction.UP: return Vector3.up;
		}
		return Vector3.zero;
	}
	
}

/// <summary>
/// Character animation states. The int value represents the animation priority.
/// If you insert new animations in here be sure to give them a unique priority value.
/// </summary>
public enum CharacterState {
	
	NONE 			=  -1,
	
	IDLE 			=  00,
	WALKING			=  10,
	RUNNING 		=  20,
	SLIDING 		=  30,			// If you don't want sliding animations change the maxSpeedForIdle parameter
	
	AIRBORNE		= 110,			// This is sent when you are in the air but are moving up or haven't reached the fall velocity
	FALLING 		= 120,
	AIRBORNE_CROUCH	= 130,			// This is sent if you are crouching during airborne or falling state
	
	WALL_SLIDING	= 160,
	
	
	CROUCHING		= 180,
	CROUCH_SLIDING 	= 190,

	HOLDING 		= 210,  		// On a ladder/climable other than a rope
	CLIMBING		= 220,  		// As above but moving up and down
	CLIMB_TOP_OF_LADDER_UP = 231,	// At the very top of ladder climbing up
	CLIMB_TOP_OF_LADDER_DOWN = 232,	// At the very top of ladder climbing down
	LEDGE_HANGING	= 240,
	LEDGE_CLIMBING	= 250,
	LEDGE_CLIMB_FINISHED = 260,
	ROPE_CLIMBING	= 270,
	ROPE_HANGING	= 280,
	ROPE_SWING		= 290,			// Note this is sent only when the player initiates a swing (1 frame)
									// if you need to check moving on rope vs hanging use character.myParent.velocity


	JUMPING 		= 410,			// This is sent on the frame you start your jump
	DOUBLE_JUMPING 	= 420,			// This is sent on the frame you start your double jump
	WALL_JUMPING 	= 430,			// This is sent on the frame you start your wall jump

	PUSHING			= 510,
	PULLING			= 520,

	STUNNED 		= 1000
};

public enum LedgeHangingState {
	TRANSITION,
	HANG,
	CLIMBING,
	FINISHED
}

public enum LadderTopState {
	CLIMBING_UP,
	FINISHED_UP,
	CLIMBING_DOWN,
	CLIMBING_DOWN_ACTION,
	CLIMBING_DOWN_PAUSE,
	FINISHED_DOWN
}


/// <summary>
/// Character controller animation event delegate.
/// </summary>
public class CharacterControllerEventDelegate {
	
}