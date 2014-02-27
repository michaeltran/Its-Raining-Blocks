using UnityEngine;
using System.Collections;

public class SkillTemplateController : MonoBehaviour {
	private TextAsset asset;
	private string folderPath = "SkillDescriptions/";
	private TalentTree talentTree;
	private LearnEventReciever learnEventReciever;
	private string fileName;
	
	void Start () {
		//talentTree = GameObject.Find("Controller").GetComponent<TalentTree>();
		//if (talentTree == null) Debug.Log ("dis ghey");
	}

	public void initializeStuff(int id) {
		getFileName (id);
		setLearnID(id);
		setDescription ();
		setTitle ();
	}

	string LoadFile(string path) {
		asset = (TextAsset)Resources.Load (folderPath + path, typeof(TextAsset));
		if(asset == null && path == "DEFAULT")
		{
			Debug.Log ("Shit son, even DEFAULT.txt doesnt exist. Someone f'd up pretty badly.");
			return "404";
		}
		else if(asset == null)
		{
			Debug.Log("FILE: " + path + ".txt was not found. Loading DEFAULT.txt instead.");
			return LoadFile ("DEFAULT");
		}
		else
		{
			return asset.text;
		}
	}

	void getFileName(int id) {
		talentTree = GameObject.Find("Controller").GetComponent<TalentTree>();
		fileName = talentTree.getTalentName(id);
	}

	void setDescription() {
		Transform childObject = transform.FindChild("Description");
		UILabel childUILabel = childObject.GetComponent<UILabel>();
		childUILabel.text = LoadFile (fileName);
	}

	void setTitle() {
		Transform childObject = transform.FindChild("NamePlate/Skill Name");
		UILabel childUILabel = childObject.GetComponent <UILabel>();
		childUILabel.text = fileName;
	}

	void setLearnID(int id) {
		learnEventReciever = transform.FindChild("LearnButton").GetComponent<LearnEventReciever>();
		learnEventReciever.currentId = id;
	}
}
