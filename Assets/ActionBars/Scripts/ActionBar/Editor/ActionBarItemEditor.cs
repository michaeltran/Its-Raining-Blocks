using UnityEngine;
using UnityEditor;
using System.Collections;



[CustomEditor(typeof(ActionBarItem))]
public class ActionBarItemEditor : Editor 
{
	ActionBarItem Item;
	bool[] ButtonFolds;
    public override void OnInspectorGUI()  
    { 
		Item = target as ActionBarItem;
		
		if(Item.ItemList == null)
		{
			Item.ItemList = new System.Collections.Generic.List<ActionBarInfo>();
		}
		if(ButtonFolds == null)
		{
			ButtonFolds = new bool[Item.ItemList.Count];    
		}
		if(ButtonFolds.Length > Item.ItemList.Count ) 
		{
			ButtonFolds = new bool[Item.ItemList.Count];  
		}
		else if(ButtonFolds.Length < Item.ItemList.Count )
		{
			ButtonFolds = new bool[Item.ItemList.Count];
		}
		
		for( int i = 0; i < Item.ItemList.Count; i++ )
		{
			EditorGUILayout.BeginHorizontal();
			if(Item.ItemList[i].Icon != null)
			{
				ButtonFolds[i] = EditorGUILayout.Foldout(ButtonFolds[i], Item.ItemList[i].Icon );
			}
			else
			{
				ButtonFolds[i] = EditorGUILayout.Foldout(ButtonFolds[i], "Item/Skill: " + (i+1).ToString() );
			}
			if( GUILayout.Button("Remove", GUILayout.Width(60) ) )
			{
				Item.ItemList.RemoveAt(i);
			}
			EditorGUILayout.EndHorizontal();	 
			if(ButtonFolds[i] == true) 
			{
				Item.ItemList[i].Atlas = EditorGUILayout.IntField(new GUIContent("Atlas","Atlas Number that is set in settings"), Item.ItemList[i].Atlas);
				Item.ItemList[i].Icon = EditorGUILayout.TextField(new GUIContent("Icon", "Name of Sprite in Atlas"), Item.ItemList[i].Icon);
				Item.ItemList[i].GroupID = EditorGUILayout.IntField(new GUIContent("Group ID","ID that determines which Rows this can be placed on"), Item.ItemList[i].GroupID);
				Item.ItemList[i].CooldownAmount = EditorGUILayout.FloatField(new GUIContent("Cooldown","Cooldown Duration"), Item.ItemList[i].CooldownAmount);
				Item.ItemList[i].Stackable = EditorGUILayout.Toggle(new GUIContent("Stackable","If this can be stacked"), Item.ItemList[i].Stackable);
				if(Item.ItemList[i].Stackable == true)
				{
					Item.ItemList[i].Stack = EditorGUILayout.IntField(new GUIContent("Stack","Starting Stacks"), Item.ItemList[i].Stack);
					Item.ItemList[i].DisplayOneStack = EditorGUILayout.Toggle(new GUIContent("Display One Stack","If it should show Stack counter at 1"), Item.ItemList[i].DisplayOneStack);
					Item.ItemList[i].DestroyOnZeroStacks = EditorGUILayout.Toggle(new GUIContent("Destroy on Zero Stacks","If this is destroyed once it reaches Zero Stacks"), Item.ItemList[i].DestroyOnZeroStacks);
				}
				Item.ItemList[i].ActivateAbility = EditorGUILayout.Toggle(new GUIContent("Activate Ability","If this can be clicked"), Item.ItemList[i].ActivateAbility);
				Item.ItemList[i].PlayCooldownAnimation = EditorGUILayout.Toggle(new GUIContent("Cooldown Animation","If the End Cooldown Animaion should be shown"), Item.ItemList[i].PlayCooldownAnimation);
				Item.ItemList[i].FillDirection = (UISprite.FillDirection) EditorGUILayout.EnumPopup(new GUIContent("Cooldown Direction", "How the cooldown should Fill in the sprite"), Item.ItemList[i].FillDirection);
				Item.ItemList[i].Target = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Target", "Calls script on specified GameObject"), Item.ItemList[i].Target, typeof(GameObject), true);
				Item.ItemList[i].Function = EditorGUILayout.TextField(new GUIContent("Function", "Calls specified Function Name on Target GameObject"),Item.ItemList[i].Function);
			}
		}
		EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
		if( GUILayout.Button("Add Item/Skill", GUILayout.Width(130) ) )
		{
			Item.ItemList.Add(new ActionBarInfo()); 
		}
		GUILayout.FlexibleSpace(); 
        EditorGUILayout.EndHorizontal();	
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(Item);
		}
		Item.SetNames();
	}
}