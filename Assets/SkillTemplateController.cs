using UnityEngine;
using System.Collections;

public class SkillTemplateController : MonoBehaviour {
	private string _folderPath = "SkillDescriptions/";
	private string _fileName;
	private string _talentName;
	private TalentTree _talentTree;
	private LearnEventReciever _learnEventReciever;

	public void initializeStuff(string id) {
		getFileName (id);
		setLearnID(id);
		setDescription ();
		setTitle ();
		setPicture();
		setCost (id);
	}

	void getFileName(string id) {
		_fileName = id;
		_talentTree = GameObject.Find("Controller").GetComponent<TalentTree>();
		_talentName = _talentTree.getTalentName(id);
	}

	void setLearnID(string id) {
		_learnEventReciever = transform.FindChild("LearnButton").GetComponent<LearnEventReciever>();
		_learnEventReciever.currentId = id;
	}

	void setDescription() {
		UILabel childUILabel = transform.FindChild ("Description").GetComponent<UILabel>();
		childUILabel.text = (LoadFile (_fileName) + "\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
		//Added the \n's because i dont know how to make text start at the top of a label, lol
	}

	void setTitle() {
		UILabel childUILabel = transform.FindChild ("NamePlate/Skill Name").GetComponent <UILabel>();
		childUILabel.text = _talentName;
	}

	void setPicture() {
		UISprite childUISprite = transform.FindChild("Picture/Sprite").GetComponent<UISprite>();
		childUISprite.spriteName = _fileName;
	}

	void setCost(string id) {
		UILabel childUILabel = transform.FindChild("PointCost").GetComponent<UILabel>();
		childUILabel.text = "[f7fd5d]Point Cost: " + _talentTree.getCost(id) + "[-]";
	}

	string LoadFile(string path) {
		TextAsset _textAsset = (TextAsset)Resources.Load (_folderPath + path, typeof(TextAsset));
		if(_textAsset == null && path == "DEFAULT")
		{
			Debug.Log ("Shit son, even DEFAULT.txt doesnt exist. Someone f'd up pretty badly.");
			return "404 Not Found";
		}
		else if(_textAsset == null)
		{
			Debug.Log("FILE: " + path + ".txt was not found. Loading DEFAULT.txt instead.");
			return LoadFile ("DEFAULT");
		}
		else
		{
			return _textAsset.text;
		}
	}
}
