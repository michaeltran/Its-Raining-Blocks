using UnityEngine;
using System.Collections;

public class Skills : MonoBehaviour {
	
	public GameObject fireBall;
	public GameObject thunder;
	
	private float originalTimeScale;
	private float originalFixedDeltaTime;
	private float slowMotionSpeed = 0.25f;
	private Status status;
	private GrayscaleEffect grayscaleEffect;
	private GameObject camera;
	
	
	// Use this for initialization
	void Start () {
		originalTimeScale = Time.timeScale;
		originalFixedDeltaTime = Time.fixedDeltaTime;
		
		status = (Status)this.gameObject.GetComponent<Status>();
		camera = GameObject.FindGameObjectWithTag ("MainCamera");
		grayscaleEffect = (GrayscaleEffect)camera.gameObject.GetComponent<GrayscaleEffect>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.Escape))
		{
			doThePause ();
		}
	}
	
	#region Pause
	void doThePause() {
		Pause.Instance.IsPaused = !Pause.Instance.IsPaused;
		if(Pause.Instance.IsPaused)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = originalTimeScale;
		}
	}
	#endregion
	
	
	#region TimeSlow Skill
	//This skill slows down time.
	void TimeSlowStart() {
		if(status.requestMana(50))
		{
			grayscaleEffect.effectAmount = 1;
			Time.timeScale = slowMotionSpeed;
			Time.fixedDeltaTime = Time.timeScale * 0.02f;
			Invoke ("TimeSlowEnd", 2f);
		}
	}
	void TimeSlowEnd() {
		grayscaleEffect.effectAmount = 0;
		Time.timeScale = originalTimeScale;
		Time.fixedDeltaTime = originalFixedDeltaTime;
	}
	#endregion
	
	#region FireBall Skill
	void FireBall() {
		if(status.requestMana(15))
		{
			//Play spell cast animation here.
			Vector3 startPosition = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y+2.5f, this.gameObject.transform.position.z);
			Instantiate(fireBall, startPosition, fireBall.transform.rotation);
		}
	}
	#endregion
	
	#region Thunder Skill
	void Thunder() {
		if(status.requestMana (30))
		{
			//Play spell cast animation here.
			Vector3 startPosition = new Vector3(this.gameObject.transform.position.x, thunder.transform.position.y, this.gameObject.transform.position.z);
			Instantiate (thunder, startPosition, thunder.transform.rotation);
		}
	}
	#endregion
}
