using UnityEngine;
using System.Collections;

public class BGMController : MonoBehaviour {
	void Start () {
		DontDestroyOnLoad(this.gameObject);
	}
}
