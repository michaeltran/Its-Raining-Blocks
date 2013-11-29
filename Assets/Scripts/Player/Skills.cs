using UnityEngine;
using System.Collections;

public class Skills : MonoBehaviour
{
	#region Public Variables
	public GameObject fireBall;
	public GameObject thunder;
	public GameObject sidewaysFireBall;
	public GameObject iceBolt;
	public AudioClip TimeSlowSound;
	public tk2dSpriteAnimator playerSprite;
	#endregion
	#region Private Variables
	private float _originalTimeScale;
	private float _originalFixedDeltaTime;
	private Status _status;
	private GrayscaleEffect _grayscaleEffect;
	private GameObject _camera;
	private tk2dSpriteAnimationClip _lastPlayedClip;
	private RaycastCharacterController _rcc;
	#endregion
	#region Skill Variables
	private float _slowMotionSpeed = 0.5f;
	private float _slowMotionTime = 1f;
	#endregion
	#region Skill Costs
	private int _timeSlowCost = 5;
	private int _upwardsFireballCost = 15;
	private int _sidewaysFireballCost = 5;
	private int _iceBoltCost = 5;
	private int _thunderCost = 20;
	#endregion

	void Start ()
	{
		_originalTimeScale = Time.timeScale;
		_originalFixedDeltaTime = Time.fixedDeltaTime;
		
		_status = (Status)this.gameObject.GetComponent<Status> ();
		_camera = GameObject.FindGameObjectWithTag ("MainCamera");
		_grayscaleEffect = (GrayscaleEffect)_camera.gameObject.GetComponent<GrayscaleEffect> ();
		_rcc = (RaycastCharacterController)this.gameObject.GetComponent ("RaycastCharacterController");
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			doThePause ();
		}
	}
	
	#region Pause
	void doThePause ()
	{
		Pause.Instance.IsPaused = !Pause.Instance.IsPaused;
		if (Pause.Instance.IsPaused) {
			Time.timeScale = 0f;
		} else {
			Time.timeScale = _originalTimeScale;
		}
	}
	#endregion
	
	#region TimeSlow Skill
	//This skill slows down time.
	void TimeSlowStart ()
	{
		if (_status.requestMana (_timeSlowCost)) {
			PlayDefaultSpellCastAnimation();
			AudioSource.PlayClipAtPoint (TimeSlowSound, transform.position);
			_grayscaleEffect.effectAmount = 1;
			Time.timeScale = _slowMotionSpeed;
			Time.fixedDeltaTime = Time.timeScale * 0.02f;
			Invoke ("TimeSlowEnd", _slowMotionTime);
		}
	}

	void TimeSlowEnd ()
	{
		_grayscaleEffect.effectAmount = 0;
		Time.timeScale = _originalTimeScale;
		Time.fixedDeltaTime = _originalFixedDeltaTime;
	}
	#endregion
	
	#region FireBall Skill
	//This skill shoots a fireball upwards
	void FireBall ()
	{
		if (_status.requestMana (_upwardsFireballCost)) {
			PlayDefaultSpellCastAnimation();
			Vector3 startPosition = new Vector3 (transform.position.x, transform.position.y + 2.5f, transform.position.z);
			Instantiate (fireBall, startPosition, fireBall.transform.rotation);
		}
	}
	#endregion

	#region Sideways FireBall Skill
	//This skill shoots a fireball sideways
	void SidewaysFireBall ()
	{
		if (_status.requestMana (_sidewaysFireballCost)) {
			PlayDefaultSpellCastAnimation();
			Vector3 startPosition = new Vector3 (transform.position.x + 2.5f * _rcc.CurrentDirection, transform.position.y, transform.position.z);
			Quaternion rotation = sidewaysFireBall.transform.rotation;
			rotation.z = rotation.z * _rcc.CurrentDirection;
			Instantiate (sidewaysFireBall, startPosition, rotation);
		}
	}
	#endregion
	
	#region Icebolt Skill
	//This skill shoots a icebolt sideways
	void IceBolt ()
	{
		if (_status.requestMana (_iceBoltCost)) {
			PlayDefaultSpellCastAnimation();
			Vector3 startPosition = new Vector3 (transform.position.x, transform.position.y + 2.5f, transform.position.z);
			Instantiate (iceBolt, startPosition, iceBolt.transform.rotation);
		}
	}
	#endregion
	
	#region Thunder Skill
	//This skill calls down thunder
	void Thunder ()
	{
		if (_status.requestMana (_thunderCost)) {
			PlayDefaultSpellCastAnimation();
			Vector3 startPosition = new Vector3 (this.gameObject.transform.position.x, thunder.transform.position.y, this.gameObject.transform.position.z);
			Instantiate (thunder, startPosition, thunder.transform.rotation);
		}
	}
	#endregion
	
	#region Animations
	void PlayDefaultSpellCastAnimation ()
	{
		_lastPlayedClip = playerSprite.CurrentClip;
		playerSprite.Play("default-spellcast");
		playerSprite.AnimationCompleted = SpellCastCompleteDelegate;
	}
	
	void SpellCastCompleteDelegate (tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
	{
		playerSprite.Play (_lastPlayedClip);
	}
	#endregion
}
