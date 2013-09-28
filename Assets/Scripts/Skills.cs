using UnityEngine;
using System.Collections;

public class Skills : MonoBehaviour {
	
	private float originalTimeScale;
	private float originalFixedDeltaTime;
	private float slowMotionSpeed = 0.5f;
	
	// Use this for initialization
	void Start () {
		originalTimeScale = Time.timeScale;
		originalFixedDeltaTime = Time.fixedDeltaTime;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Alpha1))
		{
			TimeSlowStart ();
		}
	}
	
	#region TimeSlow
	//This skill slows down time.
	void TimeSlowStart() {
		Time.timeScale = slowMotionSpeed;
		Time.fixedDeltaTime = Time.timeScale * 0.02f;
		Invoke ("TimeSlowEnd", 2f);
	}
	void TimeSlowEnd() {
		Time.timeScale = originalTimeScale;
		Time.fixedDeltaTime = originalFixedDeltaTime;
	}
	#endregion
}
