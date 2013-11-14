using UnityEngine;


using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Runtime.Serialization;
using System.Reflection;


[Serializable()]
public class SaveData : ISerializable
{
	public int easyLevelReached = 1;
	public int normalLevelReached = 1;
	public int hardLevelReached = 1;
	public int nightmareLevelreached = 1;
	public int difficultySelected = 1;
	public int music = 1;
	public int sound = 1;
	public int volume = 1;
	public bool bloom = true;
	public int controller = 1;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public SaveData()
	{
		
	}
	public SaveData(SerializationInfo info, StreamingContext ctxt)
	{
		easyLevelReached = (int)info.GetValue("easyLevelReached", typeof(int));
		normalLevelReached = (int) info.GetValue("normalLevelReached", typeof(int));
		hardLevelReached =(int) info.GetValue("hardLevelReached", typeof(int));
		nightmareLevelreached = (int) info.GetValue("nightmareLevelReached", typeof(int));
		difficultySelected = (int) info.GetValue("difficultySelected", typeof(int));
		music = (int) info.GetValue("music", typeof(int));
		sound = (int) info.GetValue("sound", typeof(int));
		volume = (int) info.GetValue("volume", typeof(int));
		bloom = (bool) info.GetValue("bloom", typeof(bool));
		controller = (int) info.GetValue("controller", typeof(int));
	}
	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("easyLevelReached", easyLevelReached);
		info.AddValue("normalLevelReached", normalLevelReached);
		info.AddValue("hardLevelReached", hardLevelReached);
	}
	
	public void Save()
	{
		SaveData data = new SaveData();
		Stream stream = File.Open ("MySavedGame.game", FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder();
		Debug.Log ("Writing Information");
		bformatter.Serialize(stream, data);
		stream.Close();
	}
	public void Load()
	{
		SaveData data = new SaveData();
		Stream stream = File.Open("MySavedGame.game",FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder();
		Debug.Log("Reading Data");
		data = (SaveData)bformatter.Deserialize(stream);
		stream.Close();
	}
}

public sealed class VersionDeserializationBinder : SerializationBinder
{
	public override Type BindToType (string assemblyName, string typeName)
	{
		 if (!string.IsNullOrEmpty (assemblyName) && !string.IsNullOrEmpty (typeName))
		{
			Type typeToDeserialize = null;
			assemblyName = Assembly.GetExecutingAssembly().FullName;
			typeToDeserialize = Type.GetType (String.Format ("{0},{1}", typeName, assemblyName));
			return typeToDeserialize;
		}
		return null;
	}
}
