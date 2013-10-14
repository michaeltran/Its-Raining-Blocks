using UnityEngine;
using System.Collections;

public class Skills : MonoBehaviour {
	
	private float originalTimeScale;
	private float originalFixedDeltaTime;
	private float slowMotionSpeed = 0.25f;
	private Status status;
	
	// Use this for initialization
	void Start () {
		originalTimeScale = Time.timeScale;
		originalFixedDeltaTime = Time.fixedDeltaTime;
		
		status = (Status)this.gameObject.GetComponent ("Status");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			TimeSlowStart ();
		}
	}
	
	#region TimeSlow Skill
	//This skill slows down time.
	void TimeSlowStart() {
		if(status.requestMana(50))
		{
			Time.timeScale = slowMotionSpeed;
			Time.fixedDeltaTime = Time.timeScale * 0.02f;
			Invoke ("TimeSlowEnd", 2f);
		}
	}
	void TimeSlowEnd() {
		Time.timeScale = originalTimeScale;
		Time.fixedDeltaTime = originalFixedDeltaTime;
	}
	#endregion
	
	#region FireBall Skill
	#endregion
}
