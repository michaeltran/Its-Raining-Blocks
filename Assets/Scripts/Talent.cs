using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public class Talent {
	public string id {get; set;}
	public string name {get; set;}
	public int cost{get; set;}
	public List<string> requirement;
	public bool isUnlocked {get; set;}

	public Talent()
	{
	}

	public Talent(string id, string name, List<string> requirement, int cost)
	{
		this.id = id;
		this.name = name;
		this.isUnlocked = false;
		this.requirement = requirement;
		this.cost = cost;
	}
}