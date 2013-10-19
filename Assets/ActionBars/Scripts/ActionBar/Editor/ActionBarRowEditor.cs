using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ActionBarRow))]
public class ActionBarRowEditor : Editor
{

	enum RowArrangement{Horizontal, Vertical};
	bool[] ButtonFolds = new bool[10];
	ActionBarRow BarRow;
    public override void OnInspectorGUI()  
    { 
		BarRow = target as ActionBarRow;
		BarRow.ActionBarButtonPrefab = (GameObject) EditorGUILayout.ObjectField("ActionBar Button", BarRow.ActionBarButtonPrefab, typeof(GameObject), false);
		BarRow.ButtonSize = EditorGUILayout.Vector2Field("Button Size", BarRow.ButtonSize);
		BarRow.Columns = EditorGUILayout.IntField("Columns", BarRow.Columns);
		BarRow.ColumnPadding = EditorGUILayout.IntField("Column Padding", BarRow.ColumnPadding);
		BarRow.Rows = EditorGUILayout.IntField("Rows", BarRow.Rows);
		BarRow.RowPadding = EditorGUILayout.IntField("Row Padding", BarRow.RowPadding);
		BarRow.Arrangement = (ActionBarRow.RowArrangement) EditorGUILayout.EnumPopup("Arrangement",BarRow.Arrangement);
		BarRow.MultipleInstances = EditorGUILayout.Toggle(new GUIContent("Multiple Instances", "If Multiple Instances of the same object is allowed per Row"), BarRow.MultipleInstances);
		
		BarRow.numGroups = EditorGUILayout.IntField("Number of Groups", BarRow.numGroups);
		if(BarRow.numGroups > BarRow.GroupID.Length )
		{
			BarRow.GroupID = new int[BarRow.numGroups];  
		}
		else if(BarRow.numGroups < BarRow.GroupID.Length )
		{
			BarRow.GroupID = new int[BarRow.numGroups];
		}
		for(int i =0; i < BarRow.GroupID.Length; i++)
		{
			BarRow.GroupID[i] = EditorGUILayout.IntField("ID",BarRow.GroupID[i]);
		}
		if(BarRow.Editor_ButtonInformation != null)
		{
			System.Array.Resize(ref ButtonFolds, BarRow.Editor_ButtonInformation.Count);
		}
		if(BarRow.Editor_ButtonInformation.Count > (BarRow.Columns * BarRow.Rows) )
		{
			for(int i = 0; i < BarRow.Editor_ButtonInformation.Count; i++)
			{ 
				if(i < (BarRow.Columns * BarRow.Rows) )
				{
					if(BarRow.Editor_ButtonInformation[i] == null) 
					{
						BarRow.Editor_ButtonInformation[i] = new ActionBarInitialization();
					}
				}
				else
				{
					BarRow.Editor_ButtonInformation.RemoveAt(i);
				}
				
			}
			ButtonFolds = new bool[BarRow.Editor_ButtonInformation.Count];
		}
		else if(BarRow.Editor_ButtonInformation.Count < (BarRow.Columns * BarRow.Rows) )
		{
			for(int i = 0; i < BarRow.Columns * BarRow.Rows; i++)
			{
				if(i < BarRow.Editor_ButtonInformation.Count) 
				{
					if(BarRow.Editor_ButtonInformation[i] == null) 
					{
						BarRow.Editor_ButtonInformation[i] = new ActionBarInitialization();
					}
				}
				else
				{
					BarRow.Editor_ButtonInformation.Add(new ActionBarInitialization());
				}
			}
			ButtonFolds = new bool[BarRow.Editor_ButtonInformation.Count];
		}
		
		EditorGUILayout.LabelField("Assign ButtonInformation", EditorStyles.boldLabel);
		//Error Checking******************
		for(int i = 0; i < BarRow.Editor_ButtonInformation.Count; i++)
		{
			
				if(BarRow.Editor_ButtonInformation[i].InfoNumber > ActionBarItem.Instance.ItemList.Count)
				{
					Debug.LogWarning(BarRow.name + " Button: " + (i+1) + "  Attempting to Access Item/Spell that no longer exists!");
				}
		}
		if(	ActionBarItem.Instance.ItemList.Count == 0)
		{
			Debug.LogWarning("No Item(s)  have been created.  Assign items in the ActionBar Controller");	
		}
		//Error Checking ******************
		for(int i = 0; i < BarRow.Editor_ButtonInformation.Count; i++)
		{
			ButtonFolds[i] = EditorGUILayout.Foldout(ButtonFolds[i], "Button: " + (i+1).ToString() );
			if(ButtonFolds[i] == true)
			{	
				
				
				BarRow.Editor_ButtonInformation[i].HotKey = (KeyCode) EditorGUILayout.EnumPopup(new GUIContent("Hotkey", "Hotkey for this Button"),BarRow.Editor_ButtonInformation[i].HotKey); 
				BarRow.Editor_ButtonInformation[i].isLocked = EditorGUILayout.Toggle(new GUIContent("Locked", "If you can pickup Items placed inside this button"), BarRow.Editor_ButtonInformation[i].isLocked);
					if(BarRow.Editor_ButtonInformation[i].isLocked == true)
					{
						BarRow.Editor_ButtonInformation[i].isCloneOnPickup = EditorGUILayout.Toggle(new GUIContent("Clone on Pickup", "Creates a new object on Pickup instead of replacing it"),BarRow.Editor_ButtonInformation[i].isCloneOnPickup);
					}
					else  
					{
						BarRow.Editor_ButtonInformation[i].isCloneOnPickup = false;
					}
				BarRow.Editor_ButtonInformation[i].isEmpty = EditorGUILayout.Toggle(new GUIContent("Empty Button", "If Button should start empty"), BarRow.Editor_ButtonInformation[i].isEmpty);
				if(BarRow.Editor_ButtonInformation[i].isEmpty == false)
				{
					EditorGUILayout.LabelField("Button Content", EditorStyles.boldLabel);
				
				
					BarRow.Editor_ButtonInformation[i].InfoNumber = EditorGUILayout.Popup(BarRow.Editor_ButtonInformation[i].InfoNumber, ActionBarItem.Instance.ItemNames, EditorStyles.popup);
					if(	ActionBarItem.Instance.ItemList.Count != 0)
					{	
						if(BarRow.Editor_ButtonInformation[i].InfoNumber < ActionBarItem.Instance.ItemList.Count)
						{
							if(ActionBarItem.Instance.ItemList[BarRow.Editor_ButtonInformation[i].InfoNumber ].Stackable == true)
							{
								BarRow.Editor_ButtonInformation[i].Info.SeperateInstance = EditorGUILayout.Toggle(new GUIContent("Seperate Instance", "Is this item it's own instance of stacks?"), BarRow.Editor_ButtonInformation[i].Info.SeperateInstance);
								if(BarRow.Editor_ButtonInformation[i].Info.SeperateInstance == true)
								{
									BarRow.Editor_ButtonInformation[i].Stacks = EditorGUILayout.IntField(new GUIContent("Stacks","Starting Stacks on Spell/Item.                           Set to Zero for Infinite"), BarRow.Editor_ButtonInformation[i].Stacks);
								}
								else
								{
									BarRow.Editor_ButtonInformation[i].Stacks = ActionBarItem.Instance.ItemList[BarRow.Editor_ButtonInformation[i].InfoNumber].Stack;
								}
							}
	
						}
					}
			
						
				}
				else //is empty
				{
					BarRow.Editor_ButtonInformation[i].InfoNumber = 0;
					BarRow.Editor_ButtonInformation[i].Info.SeperateInstance = false;
					BarRow.Editor_ButtonInformation[i].Stacks = 0;
				}
				
			}
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(BarRow);
		}
		
		
		

		
		
		
		
		

	}
	
}
