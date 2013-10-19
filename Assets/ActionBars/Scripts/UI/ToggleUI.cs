using UnityEngine;
using System.Collections;

public class ToggleUI : MonoBehaviour 
{
	public GameObject ToggleObject;
	public bool Hide;
	float DefaultZ;
	
	void Awake()
	{
		DefaultZ = ToggleObject.transform.localPosition.z;
		if(Hide == true)
		{
			ToggleObject.transform.localPosition = new Vector3(ToggleObject.transform.localPosition.x,ToggleObject.transform.localPosition.y, -2000F);
		}
		
	}

	void OnClick()
	{
		if(ActionBarCursor.Instance.ButtonInfo != null)
		{
			ActionBarCursor.Instance.Clear();
		}
			if(Hide == false) // Set Object Inactive
			{
				ToggleObject.transform.localPosition = new Vector3(ToggleObject.transform.localPosition.x,ToggleObject.transform.localPosition.y, -2000F);
				Hide = true;
			}
			else// Set Object Active
			{
				ToggleObject.transform.localPosition = new Vector3(ToggleObject.transform.localPosition.x,ToggleObject.transform.localPosition.y, DefaultZ);
				Hide = false;
			}
	}
}
