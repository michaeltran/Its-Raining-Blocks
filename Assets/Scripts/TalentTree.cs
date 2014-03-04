using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

public class TalentTree : MonoBehaviour {
	public int talentPoints = 0;
	public int maxTalentPoints = 99;

	private bool _hasPoints = false;
	private int _talentPointsSpent = 0;
	private List<Talent> _talentList = new List<Talent> ();
	private UILabel _APLabel;

	void Start() {
		_APLabel = GameObject.Find ("APLabel").GetComponent<UILabel>();
		loadDatabaseXML ();
	}

	void Update() {
		if (talentPoints >= 1) 
			_hasPoints = true;
		else 
			_hasPoints = false;
		setAPLabel ();
	}

	void setAPLabel () {
		_APLabel.text = "Available Points: " + talentPoints;
	}

	public bool isLearnt(string id) {
		Talent talent = getTalent(id);
		return talent.isUnlocked;
	}

	public int getCost(string id) {
		Talent talent = getTalent(id);
		return talent.cost;
	}

	public Talent getTalent(string id) {
		Talent talent = _talentList.Find (x => x.id == id);
		if(talent == null)
			Debug.Log("No talent found with the id: " + id);
		return talent;
	}

	public void learnTalent(string id) {
		Talent talent = getTalent(id);
		if(talent.isUnlocked != true && checkPreRequisites(id) == true && talentPoints >= talent.cost)
		{
			talentPoints -= talent.cost;
			talent.isUnlocked = true;
		}
	}

	public string getTalentName(string id) {
		Talent talent = getTalent(id);
		return talent.name;
	}

	// Returns true if all pre-req's found, false if not
	public bool checkPreRequisites(string id) {
		Talent talent = getTalent(id);
		//Now get the talents required and check if they are unlocked
		foreach(string reqId in talent.requirement)
		{
			Talent preReqTalent = _talentList.Find (x => x.id == reqId);
			if(preReqTalent == null)
			{
				Debug.Log ("No pre-requisite talent found with the id: " + id);
				return false;
			}
			if(preReqTalent.isUnlocked == false)
			{
				return false;
			}
		}
		return true;
	}
	
	public void addTalent(string id, string name, List<string> requirement, int cost) {
		Talent talent = new Talent(id, name, requirement, cost);
		_talentList.Add(talent);
	}

	void loadDatabaseXML() {
		TextAsset _textAsset = (TextAsset)Resources.Load ("TalentDatabase", typeof(TextAsset));
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml (_textAsset.text);
		XmlNodeList talentList = xmlDoc.GetElementsByTagName ("talent");
		
		foreach(XmlNode talentInfo in talentList)
		{
			string id = "";
			string name = "";
			List<string> requirement = new List<string> ();
			int cost = 0;
			
			XmlNodeList talentContent = talentInfo.ChildNodes;
			foreach(XmlNode talentItems in talentContent)
			{
				
				if(talentItems.Name == "id")
				{
					id = talentItems.InnerText;
				}
				if(talentItems.Name == "name")
				{
					name = talentItems.InnerText;
				}
				if(talentItems.Name == "requirement")
				{
					requirement.Add (talentItems.InnerText);
				}
				if(talentItems.Name == "cost")
				{
					cost = int.Parse (talentItems.InnerText);
				}
			}
			addTalent (id, name, requirement, cost);
		}
	}
}



public class Talent {
	public string id {get; set;}
	public string name {get; set;}
	public bool isUnlocked {get; set;}
	public List<string> requirement;
	public int cost{get; set;}

	public Talent(string id, string name, List<string> requirement, int cost)
	{
		this.id = id;
		this.name = name;
		this.isUnlocked = false;
		this.requirement = requirement;
		this.cost = cost;
	}
}