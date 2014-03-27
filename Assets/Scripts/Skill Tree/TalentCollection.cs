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
}
