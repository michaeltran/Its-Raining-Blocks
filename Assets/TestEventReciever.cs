using UnityEngine;
using System.Collections;

public class TestEventReciever : MonoBehaviour {
	public GameObject test;
	void OnClick() {
		test.SetActive(true);
	}
}
