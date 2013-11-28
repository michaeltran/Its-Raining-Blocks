using UnityEngine;
using System.Collections;

public class Skills : MonoBehaviour
{
	public GameObject fireBall;
	public GameObject thunder;
	public GameObject sidewaysFireBall;
	public GameObject iceBolt;
	public AudioClip TimeSlowSound;
	public tk2dSpriteAnimator playerSprite;
	private float _originalTimeScale;
	private float _originalFixedDeltaTime;
	private float _slowMotionSpeed = 0.25f;
	private Status _status;
	private GrayscaleEffect _grayscaleEffect;
	private GameObject _camera;

	void Start ()
	{
		_originalTimeScale = Time.timeScale;
		_originalFixedDeltaTime = Time.fixedDeltaTime;
		
		_status = (Status)this.gameObject.GetComponent<Status> ();
		_camera = GameObject.FindGameObjectWithTag ("MainCamera");
		_grayscaleEffect = (GrayscaleEffect)_camera.gameObject.GetComponent<GrayscaleEffect> ();
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
		if (_status.requestMana (25)) {
			playerSprite.Play("default-spellcast");
			AudioSource.PlayClipAtPoint (TimeSlowSound, transform.position);
			_grayscaleEffect.effectAmount = 1;
			Time.timeScale = _slowMotionSpeed;
			Time.fixedDeltaTime = Time.timeScale * 0.02f;
			Invoke ("TimeSlowEnd", 2f);
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
		if (_status.requestMana (15)) {
			playerSprite.Play("default-spellcast");
			Vector3 startPosition = new Vector3 (transform.position.x, transform.position.y + 2.5f, transform.position.z);
			Instantiate (fireBall, startPosition, fireBall.transform.rotation);
		}
	}
	#endregion

	#region Sideways FireBall Skill
	//This skill shoots a fireball sideways
	void SidewaysFireBall ()
	{
		if (_status.requestMana (15)) {
			playerSprite.Play("default-spellcast");
			RaycastCharacterController rcc = (RaycastCharacterController)this.gameObject.GetComponent ("RaycastCharacterController");
			Vector3 startPosition = new Vector3 (transform.position.x + 2.5f * rcc.CurrentDirection, transform.position.y, transform.position.z);
			Quaternion rotation = sidewaysFireBall.transform.rotation;
			rotation.z = rotation.z * rcc.CurrentDirection;
			Instantiate (sidewaysFireBall, startPosition, rotation);
		}
	}
	#endregion
	
	#region Icebolt Skill
	//This skill shoots a icebolt sideways
	void IceBolt ()
	{
		if (_status.requestMana (20)) {
			playerSprite.Play("default-spellcast");
			Vector3 startPosition = new Vector3 (transform.position.x, transform.position.y + 2.5f, transform.position.z);
			Instantiate (iceBolt, startPosition, iceBolt.transform.rotation);
		}
	}
	#endregion
	
	#region Thunder Skill
	//This skill calls down thunder
	void Thunder ()
	{
		if (_status.requestMana (30)) {
			playerSprite.Play("default-spellcast");
			Vector3 startPosition = new Vector3 (this.gameObject.transform.position.x, thunder.transform.position.y, this.gameObject.transform.position.z);
			Instantiate (thunder, startPosition, thunder.transform.rotation);
		}
	}
	#endregion
}
