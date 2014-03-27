using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class PlayerData : Singleton<PlayerData> {
	public _PlayerData data = new _PlayerData();
	private string filePath = "DataFiles/test/test2.xml";
	
	protected PlayerData () {
		if(System.IO.File.Exists(Path.Combine(Application.dataPath, filePath))) {
			Load (Path.Combine (Application.dataPath, filePath));
		}
		else {
			Save();
		}
	}
	
	public void Save() {
		string path = Path.Combine (Application.dataPath, filePath);
		Save (path);
	}
	
	public void Save(string path) {
		var serializer = new XmlSerializer(typeof(_PlayerData));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, data);
		}
	}
	
	public void Load(string path) {
		var serializer = new XmlSerializer(typeof(_PlayerData));
		using(var stream = new FileStream(path, FileMode.Open))
		{
			data = serializer.Deserialize(stream) as _PlayerData;
		}
	}
}

[XmlRoot("PlayerData")]
public class _PlayerData {
	public int talentPoints = 10;
	public TalentCollection tc = null;
}