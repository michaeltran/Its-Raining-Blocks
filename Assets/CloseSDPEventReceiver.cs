using UnityEngine;
using System.Collections;

public class CloseSDPEventReceiver : MonoBehaviour {
	void OnClick() {
		//TODO: Find a better way to call this, lol
		this.transform.parent.transform.parent.gameObject.SetActive(false);
	}
}
