using UnityEngine;
using System.Collections;


public class ActionBarItem : MonoBehaviour 
{
	static ActionBarItem mInstance = null;
	public System.Collections.Generic.List<ActionBarInfo> ItemList;
	public string[] ItemNames; 
	
	public static ActionBarItem Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType(typeof(ActionBarItem)) as ActionBarItem;
            }

            return mInstance;
        }
    }
	public void SetNames()
	{
		ItemNames = new string[ItemList.Count];
		for(int i = 0; i < ItemList.Count; i++)
		{ 
			ItemNames[i] = ItemList[i].Icon; 
		}
	}
}

