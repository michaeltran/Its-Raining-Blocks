using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class PlayerController : MonoBehaviour {
	void Awake() {
		//PlayerData.Instance.Save (Path.Combine(Application.dataPath, filePath));
		//PlayerData.Instance.Load (Path.Combine (Application.dataPath, filePath));
		Debug.Log(PlayerData.Instance.data.talentPoints);
	}
}