using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("TalentCollection")]
public class TalentCollection {
	[XmlArray("Talents")]
	[XmlArrayItem("Talent")]
	public List<Talent> _talentList = new List<Talent> ();

	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(TalentCollection));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}

	public static TalentCollection Load(string path)
	{
		var serializer = new XmlSerializer(typeof(TalentCollection));
		using(var stream = new FileStream(path, FileMode.Open))
		{
			return serializer.Deserialize(stream) as TalentCollection;
		}
	}
}
