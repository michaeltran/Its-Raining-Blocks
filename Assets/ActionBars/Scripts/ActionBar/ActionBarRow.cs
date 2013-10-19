using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;


public class ActionBarRow : MonoBehaviour 
{
	public GameObject ActionBarButtonPrefab;
	public enum RowArrangement{Horizontal, Vertical};
	public Vector2 ButtonSize;
	GameObject ActionBarButtonClone;
	public int Columns;
	public int ColumnPadding;
	public int Rows;
	public int RowPadding;
	public RowArrangement Arrangement;
	UIGrid ActionBarGrid;
	public bool isLocked = false;
	public int[]  GroupID = new int[1];
	public bool CloneOnPickup = false;
	public bool MultipleInstances = true;
	public bool DisplayOneStack = true;
	public KeyCode[] Hotkey;
	[HideInInspector]
	public int numGroups;
	[HideInInspector]
	public System.Collections.Generic.List<ActionBarButton> ButtonList = new System.Collections.Generic.List<ActionBarButton>();
	public List<ActionBarInitialization> Editor_ButtonInformation = new List<ActionBarInitialization>();

	public bool CompleteRowWithEmptyButtons;
	
	void Awake()
	{ 
		InitalizeRow();
	}

	public void InitalizeRow()
	{
		if(ActionBarButtonPrefab != null)
		{
			//Error checking in case user forgets to assign size values
			if( (ButtonSize.x == 0F) || (ButtonSize.y == 0F) )
			{
				Debug.LogWarning("Button Size is set to 0.  It will not be Visible");
			}
			//Avoids modifying the original Prefab
			ActionBarButtonClone = ActionBarButtonPrefab; 
			
			//Assign the Grid Component in order to arrange the objects.
			ActionBarGrid = (UIGrid)this.gameObject.AddComponent("UIGrid");
			ActionBarGrid.maxPerLine = Columns; 
			if(Arrangement == RowArrangement.Horizontal){ActionBarGrid.arrangement = UIGrid.Arrangement.Horizontal;}
			else{ActionBarGrid.arrangement = UIGrid.Arrangement.Vertical;}
			ActionBarGrid.cellWidth = ButtonSize.x + ColumnPadding;
			ActionBarGrid.cellHeight = ButtonSize.y + RowPadding;
			ActionBarGrid.sorted = false;
			ActionBarGrid.hideInactive = true;
			//Assigns each spell/item a unique number value
			foreach(ActionBarInfo Info in ActionBarItem.Instance.ItemList)
			{
				Info.InstanceNumber = ActionBarSettings.Instance.GetInstanceNumber();
			}
			//Spawn Spells/Items
			for(int i=0; i < (Rows * Columns); i++)
			{
				if(Editor_ButtonInformation[i] != null)
				{
					SpawnChild(Editor_ButtonInformation[i]);
				}
				else
				{
					Debug.Log ("Null");
				}
						
			}
			ActionBarGrid.Reposition();
		}
		else
		{
			Debug.LogWarning ("You need to Assign a Button Prefab to the ActionBarRow!");
		}
		
		
	}
	
	public ActionBarInfo SpawnChild(ActionBarInitialization ButtonInformation)
	{
		//Assign Button Size
		ActionBarButtonClone.transform.GetChild(0).transform.localScale = new Vector3(ButtonSize.x,ButtonSize.y,1F); 
		//Add Button as a child to the Action Bar Row
		ActionBarButton button = NGUITools.AddChild(this.gameObject,ActionBarButtonClone).GetComponentInChildren<ActionBarButton>(); 
		//Assign MISC Information to Items/Spells
		if(ButtonInformation.Info.Stackable == true)
		{
			ButtonInformation.Info.Stack = ButtonInformation.Stacks;
		}
		if(ButtonInformation.Info.SeperateInstance == true)
		{
			button.SetInfo(Clone(ButtonInformation));
		}
		else
		{
			if(ButtonInformation.isEmpty == false)
			{
				if(ButtonInformation.InfoNumber < ActionBarItem.Instance.ItemList.Count)
				{
					button.SetInfo(ActionBarItem.Instance.ItemList[ButtonInformation.InfoNumber]); //Assign the Spell/Item Information
				}
				else
				{
					Debug.LogWarning(gameObject.name + " is attempting to Assign Spell that no longer exists!");
					button.SetInfo(null);
				}

			}
			else
			{
				button.SetInfo(null);
			} 
		}
		
		//button.DisplayOneStack = ButtonInformation.DisplayOneStack;
		button.HotKey = ButtonInformation.HotKey;  //Assign the Hotkey
		if(ActionBarSettings.Instance.HotKeyDictionary.ContainsKey(button.HotKey))
		{
			button.HotKeyLabel.text = ActionBarSettings.Instance.HotKeyDictionary[button.HotKey];
		}
		else//Check Keycode to see if it is None
		{
			button.HotKeyLabel.text = ""; //Assign Text to Blank since it has no Hotkey
		}
		button.isEmpty = ButtonInformation.isEmpty;
		button.isLocked = ButtonInformation.isLocked;
		button.isCloneOnPickup = ButtonInformation.isCloneOnPickup;
	

		
		button.BarRow = GetComponent<ActionBarRow>();	//Assign Parent Row
		ActionBarSettings.Instance.Buttons.Add(button); //Keep track of all buttons
		ButtonList.Add(button); //Add button to list
		ActionBarGrid.Reposition();
		return button.Info;
	}
	//Creates a clone of the item/spell.  This will be a seperate instance than the original.
	//It will share the same cooldown, yet will not share the same stacks.
	public static ActionBarInfo Clone(ActionBarInitialization ItemSlot)
	{
		ActionBarInfo tempObject = new ActionBarInfo();
		
		tempObject.SetActionBarInfo(ActionBarItem.Instance.ItemList[ItemSlot.InfoNumber].Atlas, 
									ActionBarItem.Instance.ItemList[ItemSlot.InfoNumber].Icon,
									ActionBarItem.Instance.ItemList[ItemSlot.InfoNumber].GroupID,
									false,
									ActionBarItem.Instance.ItemList[ItemSlot.InfoNumber].CooldownAmount);
		tempObject.FillDirection = ActionBarItem.Instance.ItemList[ItemSlot.InfoNumber].FillDirection;
		if(ActionBarItem.Instance.ItemList[ItemSlot.InfoNumber].Stackable == true)
		{
			tempObject.Stack = ItemSlot.Stacks;
		}							
		tempObject.ActivateAbility = ActionBarItem.Instance.ItemList[ItemSlot.InfoNumber].ActivateAbility;
		tempObject.PlayCooldownAnimation = ActionBarItem.Instance.ItemList[ItemSlot.InfoNumber].PlayCooldownAnimation;
		tempObject.DestroyOnZeroStacks = ActionBarItem.Instance.ItemList[ItemSlot.InfoNumber].DestroyOnZeroStacks;
		tempObject.Target = ActionBarItem.Instance.ItemList[ItemSlot.InfoNumber].Target;
		tempObject.Function = ActionBarItem.Instance.ItemList[ItemSlot.InfoNumber].Function;
		tempObject.SeperateInstance = ActionBarItem.Instance.ItemList[ItemSlot.InfoNumber].SeperateInstance;
	
		return tempObject;
	}
	
}

//Editor Class
[Serializable]
public class ActionBarInitialization
{
	[SerializeField]
	public ActionBarInfo Info;
	[SerializeField]
	public int InfoNumber;
	[SerializeField]
	public KeyCode HotKey;
	[SerializeField]
	public bool isEmpty = true;
	[SerializeField]
	public bool isLocked;
	[SerializeField]
	public bool isCloneOnPickup;
	[SerializeField]
	public int Stacks;
	//[SerializeField]
	//public bool DisplayOneStack;
		
}