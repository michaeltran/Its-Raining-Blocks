using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

public class TalentTree : MonoBehaviour {
	public int talentPoints = 0;
	public int maxTalentPoints = 99;
	private bool hasPoints = false;
	private int talentPointsSpent = 0;
	private List<Talent> _talentList = new List<Talent> ();
	private TextAsset asset;

	private UILabel APLabel;

	void Start() {
		APLabel = GameObject.Find ("APLabel").GetComponent<UILabel>();

		getXML ();
	}

	void Update() {
		if (talentPoints >= 1) 
			hasPoints = true;
		else 
			hasPoints = false;
		setAPLabel ();
	}

	void setAPLabel () {
		APLabel.text = "Available Points: " + talentPoints;
	}

	public void getTalent(int id) {
		Talent talent = _talentList.Find (x => x.id == id);
		if(talent.isUnlocked != true)
		{
			if(checkPreRequisites(id) == true)
			{
				if(talentPoints >= talent.cost)
				{
					talentPoints -= talent.cost;
					talent.isUnlocked = true;
				}
			}
		}
	}

	public string getTalentName(int id) {
		Talent talent = _talentList.Find (x => x.id == id);
		if(talent == null)
			Debug.Log ("No talent found with the id: " + id);
		return talent.name;
	}

	// Returns true if all pre-req's found, false if not
	public bool checkPreRequisites(int id) {
		//Get the talent in question
		Talent talent = _talentList.Find (x => x.id == id);
		if(talent == null) {
			Debug.Log ("No talent found with the id: " + id);
			return false;
		}
		//Now get the talents required and check if they are unlocked
		foreach(int reqId in talent.requirement)
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
	
	public void addTalent(int id, string name, List<int> requirement, int cost) {
		Talent talent = new Talent(id, name, requirement, cost);
		_talentList.Add(talent);
	}

	void getXML() {
		asset = (TextAsset)Resources.Load ("sample", typeof(TextAsset));
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml (asset.text);
		XmlNodeList talentList = xmlDoc.GetElementsByTagName ("talent");
		
		foreach(XmlNode talentInfo in talentList)
		{
			int id = 0;
			string name = "";
			List<int> requirement = new List<int> ();
			int cost = 0;
			
			XmlNodeList talentContent = talentInfo.ChildNodes;
			foreach(XmlNode talentItems in talentContent)
			{
				
				if(talentItems.Name == "id")
				{
					id = int.Parse (talentItems.InnerText);
				}
				if(talentItems.Name == "name")
				{
					name = talentItems.InnerText;
				}
				if(talentItems.Name == "requirement")
				{
					requirement.Add (int.Parse (talentItems.InnerText));
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
	public int id {get; set;}
	public string name {get; set;}
	public bool isUnlocked {get; set;}
	public List<int> requirement;
	public int cost{get; set;}

	public Talent(int id, string name, List<int> requirement, int cost)
	{
		this.id = id;
		this.name = name;
		this.isUnlocked = false;
		this.requirement = requirement;
		this.cost = cost;
	}
}