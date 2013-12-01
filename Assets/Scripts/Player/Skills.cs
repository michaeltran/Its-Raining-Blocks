using UnityEngine;
using System.Collections;

public class Skills : MonoBehaviour
{
	/*
	 * Instructions for adding new skills:
	 * Method must return a bool value for success/nonsuccess
	 * You must add the skill into the pickSkill method for it to be usable from the action bars
	 * When adding a skill to the action bars, use the same function string as the actual function to keep everything in order
	*/
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
	private GameObject _camera;
	private Status _status;
	private GrayscaleEffect _grayscaleEffect;
	private tk2dSpriteAnimationClip _lastPlayedClip;
	private System.Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> _AnimationCompleted;
	private RaycastCharacterController _rcc;
	private ParticleSystem _insufficientManaCFX;
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
	private int _thunderCost = 25;
	#endregion

	void Start ()
	{
		_originalTimeScale = Time.timeScale;
		_originalFixedDeltaTime = Time.fixedDeltaTime;
		
		_AnimationCompleted = playerSprite.AnimationCompleted;
		_status = (Status)GetComponent<Status> ();
		_camera = GameObject.FindGameObjectWithTag ("MainCamera");
		_grayscaleEffect = (GrayscaleEffect)_camera.gameObject.GetComponent<GrayscaleEffect> ();
		_rcc = (RaycastCharacterController)GetComponent ("RaycastCharacterController");
		_insufficientManaCFX = transform.FindChild ("CFX_Text_Insufficient_Mana").particleSystem;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			doThePause ();
		}
	}
	
	#region Pick
	public bool PickSkill (string skillString)
	{
		switch (skillString) {
		case "TimeSlowStart":
			return TimeSlowStart ();
		case "FireBall":
			return FireBall ();
		case "SidewaysFireBall":
			return SidewaysFireBall ();
		case "IceBolt":
			return IceBolt ();
		case "Thunder":
			return Thunder ();
		case "MagicShield":
			return MagicShield ();
		default:
			return false;
		}
	}
	#endregion
	
	
	#region Pause
	void doThePause ()
	{
		Pause.Instance.IsPaused = !Pause.Instance.IsPaused;
		if (Pause.Instance.IsPaused) {
			_status.GodMode = !_status.GodMode;
			Time.timeScale = 0f;
		} else {
			Time.timeScale = _originalTimeScale;
		}
	}
	#endregion
	
	#region TimeSlow Skill
	//This skill slows down time.
	bool TimeSlowStart ()
	{
		if (_status.requestMana (_timeSlowCost)) {
			PlayDefaultSpellCastAnimation ();
			AudioSource.PlayClipAtPoint (TimeSlowSound, transform.position);
			_grayscaleEffect.effectAmount = 1;
			Time.timeScale = _slowMotionSpeed;
			Time.fixedDeltaTime = Time.timeScale * 0.02f;
			Invoke ("TimeSlowEnd", _slowMotionTime);
		} else {
			InsufficientMana ();
			return false;
		}
		return true;
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
	bool FireBall ()
	{
		if (_status.requestMana (_upwardsFireballCost)) {
			PlayDefaultSpellCastAnimation ();
			Vector3 startPosition = new Vector3 (transform.position.x, transform.position.y + 2.5f, transform.position.z);
			Instantiate (fireBall, startPosition, fireBall.transform.rotation);
		} else {
			InsufficientMana ();
			return false;
		}
		return true;
	}
	#endregion

	#region Sideways FireBall Skill
	//This skill shoots a fireball sideways
	bool SidewaysFireBall ()
	{
		if (_status.requestMana (_sidewaysFireballCost)) {
			PlayDefaultSpellCastAnimation ();
			Vector3 startPosition = new Vector3 (transform.position.x + 2.5f * _rcc.CurrentDirection, transform.position.y, transform.position.z);
			Quaternion rotation = sidewaysFireBall.transform.rotation;
			rotation.z = rotation.z * _rcc.CurrentDirection;
			Instantiate (sidewaysFireBall, startPosition, rotation);
		} else {
			InsufficientMana ();
			return false;
		}
		return true;
	}
	#endregion
	
	#region Icebolt Skill
	//This skill shoots a icebolt sideways
	bool IceBolt ()
	{
		if (_status.requestMana (_iceBoltCost)) {
			PlayDefaultSpellCastAnimation ();
			Vector3 startPosition = new Vector3 (transform.position.x, transform.position.y + 2.5f, transform.position.z);
			Instantiate (iceBolt, startPosition, iceBolt.transform.rotation);
		} else {
			InsufficientMana ();
			return false;
		}
		return true;
	}
	#endregion
	
	#region Thunder Skill
	//This skill calls down thunder
	bool Thunder ()
	{
		if (_status.requestMana (_thunderCost)) {
			PlayDefaultSpellCastAnimation ();
			Vector3 startPosition = new Vector3 (this.gameObject.transform.position.x, thunder.transform.position.y, this.gameObject.transform.position.z);
			Instantiate (thunder, startPosition, thunder.transform.rotation);
		} else {
			InsufficientMana ();
			return false;
		}
		return true;
	}
	#endregion
	
	#region Magic Shield Skill
	bool MagicShield ()
	{
		return false;
	}
	#endregion
	
	#region Animations
	void PlayDefaultSpellCastAnimation ()
	{
		_lastPlayedClip = playerSprite.CurrentClip;
		playerSprite.Play ("default-spellcast");
		playerSprite.AnimationCompleted = SpellCastCompleteDelegate;
	}
	
	void SpellCastCompleteDelegate (tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
	{
		playerSprite.Play (_lastPlayedClip);
		playerSprite.AnimationCompleted = _AnimationCompleted;
	}
	
	void InsufficientMana ()
	{
		if(_insufficientManaCFX.isPlaying == false)
			_insufficientManaCFX.Play ();
	}
	#endregion
}
