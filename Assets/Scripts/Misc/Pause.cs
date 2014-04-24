using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour
{
	private static Pause _instance;
	public static Pause Instance
	{
		get
		{
			if (_instance == null) {
				_instance = new GameObject("Pause").AddComponent<Pause>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance != null) {
			GameObject.Destroy(gameObject);
		} else {	
			DontDestroyOnLoad(gameObject);
		}
	}

	public bool IsPaused;
}