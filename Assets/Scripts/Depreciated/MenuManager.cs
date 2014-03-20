using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour 
{
	int groupWidth = 750;						// width of main GUI
	Rect buttonRect = new Rect(0,120,130,30);		// button size: x, y, width, height

	bool mainMenu = true;						// flag for main menu
	bool singlePlayer = false;					// flag for single player
	bool multiplayer = false;
	bool coopPlay = false;
	bool options = false;
	bool saving = false;
	string gameTitle = "It's Raining Blocks!";
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	void OnGUI()
	{
		GUI.depth = 1;
		//GUI.skin = menuSkin;
		if (mainMenu)
		{
			GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
			GUI.Box(new Rect(0,0, Screen.width, Screen.height), "");
		
			// Title
			GUI.Label(new Rect(Screen.width/2-50,0,650,100), gameTitle);   // game title text
		
			// Buttons
			Rect buttonRectTemp = new Rect(Screen.width/2-30, 250, buttonRect.width, buttonRect.height);
			if (GUI.Button(buttonRectTemp, "Single Player"))
			{
				mainMenu = false;
				singlePlayer =true;	
			}
		
			buttonRectTemp.y += buttonRect.height + 20;
			if (GUI.Button(buttonRectTemp, "Co-operative Play"))
			{
				mainMenu = false;
				coopPlay = true;
			}
		
			buttonRectTemp.y += buttonRect.height + 20;
			if (GUI.Button(buttonRectTemp, "Multiplayer"))
			{
				mainMenu = false;
				multiplayer = true;
			}
		
			buttonRectTemp.y += buttonRect.height + 20;
			if (GUI.Button(buttonRectTemp, "Options"))
			{
				mainMenu=false;
				options = true;
			}
			
			buttonRectTemp.y += buttonRect.height + 20;
			if (GUI.Button(buttonRectTemp, "Save"))
			{
				SaveGame(true);
			}
		
			buttonRectTemp.y += buttonRect.height + 20;
			if (GUI.Button(buttonRectTemp, "Load"))
			{
				LoadGame();
			}
			GUI.EndGroup();
		}
		if (singlePlayer)
		{
			GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
			GUI.Box(new Rect(0,0, Screen.width, Screen.height), "It's Raining Blocks!");
		
			Rect buttonRectTemp = new Rect(Screen.width/2-30, 250, buttonRect.width, buttonRect.height);
			if (GUI.Button(buttonRectTemp, "Easy"))
			{
				EasyStageSelect();
			}
		
			buttonRectTemp.y += buttonRect.height + 20;
			if (GUI.Button(buttonRectTemp, "Normal"))
			{
				NormalStageSelect();
			}
		
			buttonRectTemp.y += buttonRect.height + 20;
			if (GUI.Button(buttonRectTemp, "Hard"))
			{
				HardStageSelect();
			}	
			GUI.EndGroup();
		}
		if (multiplayer)
		{
			GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
			GUI.Box(new Rect(0,0, Screen.width, Screen.height), "It's Raining Blocks!");
		
			Rect buttonRectTemp = new Rect(Screen.width/2-30, 250, buttonRect.width, buttonRect.height);
			if (GUI.Button(buttonRectTemp, "Easy"))
			{
				
			}
		
			buttonRectTemp.y += buttonRect.height + 20;
			if (GUI.Button(buttonRectTemp, "Normal"))
			{
				
			}
		
			buttonRectTemp.y += buttonRect.height + 20;
			if (GUI.Button(buttonRectTemp, "Hard"))
			{
				
			}	
			GUI.EndGroup();
		}
		
		if (coopPlay)
		{
			
		}
		
		if (options)
		{
			
		}
	}
	void SaveGame(bool flag)
	{
			
	}
	void LoadGame()
	{
		
	}
	
	void EasyStageSelect()
	{
		
	}
	
	void NormalStageSelect()
	{
		
	}
	
	void HardStageSelect()
	{
		
	}
}
