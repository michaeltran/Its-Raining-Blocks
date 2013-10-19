using UnityEngine;
using System.Collections;
using System;

public class ActionBarButton : MonoBehaviour 
{
	public ActionBarRow BarRow;
	public ActionBarInfo Info;
	public UILabel StackLabel;
	public UILabel HotKeyLabel;
	public UILabel CooldownLabel;
	public UISprite CooldownSprite;
	public UIImageButton ImageButton;
	public UISprite Icon;
	
	public KeyCode HotKey;
	public bool isEmpty;
	public bool isLocked;
	public bool isCloneOnPickup;
	public bool DisplayOneStack;
	
	
	//NGUI OnPress Event
	void OnPress(bool isPressed)
	{
		// Left Clicking an item, thus activating it
		if( Input.GetMouseButtonUp(0) && (Info != null) && (ActionBarCursor.Instance.ButtonInfo == null) ) 
		{
			IsClicked();
		}
		//Dropping Item onto Button
		else if( Input.GetMouseButtonUp(0) && (Info == null) && (ActionBarCursor.Instance.ButtonInfo != null) ) 
		{
			//If Action bar allows multiple instances of same item
			if(BarRow.MultipleInstances == true) 
			{
				//checks to see if Group IDs of Row and Button match
				if(CheckGroupID(ActionBarCursor.Instance.ButtonInfo.GroupID, BarRow.GroupID) )
				{
					//Set Info of Held item to the Button and then clear Cursor
					SetInfo(ActionBarCursor.Instance.ButtonInfo);
					ActionBarCursor.Instance.Clear();
					NGUITools.PlaySound(ActionBarSettings.Instance.ButtonClickSound_Success);
				}
				else
				{
					NGUITools.PlaySound(ActionBarSettings.Instance.ButtonClickSound_Disable); 
				}
			}
			else//If only one instance per row
			{
				//Check group ID to make sure they item and row matches
				if(CheckGroupID(ActionBarCursor.Instance.ButtonInfo.GroupID, BarRow.GroupID) ) 
				{
					//Loop through each button in the row for multiple instances
					foreach(ActionBarButton button in BarRow.ButtonList) 
					{
						if(button.Info != null)
						{
							if( button.Info.InstanceNumber == ActionBarCursor.Instance.ButtonInfo.InstanceNumber)
							{
								button.RemoveInfo();
							}
						}
					}
					//Set Held item to button and clear held item
					SetInfo(ActionBarCursor.Instance.ButtonInfo);
					ActionBarCursor.Instance.Clear();
					NGUITools.PlaySound(ActionBarSettings.Instance.ButtonClickSound_Success); //Play Success sound
					
				}
				else
				{
					NGUITools.PlaySound(ActionBarSettings.Instance.ButtonClickSound_Disable);
				}
			}
			
			
		}
		//Swapping Held Item with Button's Item
		else if( Input.GetMouseButtonUp(0) && (Info != null) && (ActionBarCursor.Instance.ButtonInfo != null)  ) 
		{
			//Check to make sure the Group ID on the item matches the row's
			if(CheckGroupID(ActionBarCursor.Instance.ButtonInfo.GroupID,BarRow.GroupID) )
			{
				if(BarRow.isLocked == false)
				{
					if(BarRow.MultipleInstances == true)
					{
							ActionBarCursor.Instance.PermissionCheck = false;
							ActionBarInfo temp = ActionBarCursor.Instance.ButtonInfo;
							ActionBarCursor.Instance.Set(Info, BarRow.ButtonSize);
							SetInfo(temp);
							ActionBarCursor.Instance.PermissionCheck = true;
							NGUITools.PlaySound(ActionBarSettings.Instance.ButtonSound_Swap);
					}
					else//Only one instance per row
					{
						foreach(ActionBarButton button in BarRow.ButtonList)
						{
							if(button.Info != null)
							{
								//Loop through row and eliminate multiple instances of the item
								if( (button.Info.InstanceNumber == ActionBarCursor.Instance.ButtonInfo.InstanceNumber) )
								{
									button.RemoveInfo(); //Remove the multiple
								}
								
								
							}
						}
						ActionBarCursor.Instance.PermissionCheck = false;	//Temporarily Disable auto-destroy for cursor on click
						ActionBarInfo temp = ActionBarCursor.Instance.ButtonInfo;
						ActionBarCursor.Instance.Set(Info, BarRow.ButtonSize);
						SetInfo(temp);
						CooldownSprite.fillAmount = 0F;
						ActionBarCursor.Instance.PermissionCheck = true;
						NGUITools.PlaySound(ActionBarSettings.Instance.ButtonSound_Swap);
					}
				}
				
			}
			else
			{
				if(BarRow.isLocked == true)
				{
					ActionBarCursor.Instance.Clear(); 
				}
				
				//ActionBarCursor.Instance.Clear();   //Clear if placing item and it is mismatching ID
				NGUITools.PlaySound(ActionBarSettings.Instance.ButtonClickSound_Disable);
			}
		}
	
		else if( Input.GetMouseButtonUp(0) && (Info == null) && (ActionBarCursor.Instance.ButtonInfo == null) ) //Clicking Empty button
		{
			NGUITools.PlaySound(ActionBarSettings.Instance.ButtonClickSound_Disable);
		}
		
		
	}
	//NGUI OnDrag Event
	void OnDrag()
	{
		//Check to see if holding an item
		if(ActionBarCursor.Instance.ButtonInfo == null) 
		{
			 //Makes sure there is something in the button
			if(Info != null)
			{
				if(isLocked == false)
				{
					ActionBarCursor.Instance.Set(Info, BarRow.ButtonSize);
					if(isCloneOnPickup == false)
					{
						RemoveInfo();
					}
				}
				else
				{
					if(isCloneOnPickup)
					{
						ActionBarCursor.Instance.Set(Info, BarRow.ButtonSize);
					}
				}
			}
		}
	}
	//NGUI OnHover Event
	void OnHover (bool isOver)
	{
		ActionBarCursor.Instance.isHovered = isOver;
	}
	
	//NGUI OnTooltip Event
	void OnTooltip (bool show)
	{
		if(show)
		{
			if(ActionBarCursor.Instance.ButtonInfo == null)
			{
				string Tip;
				if(Info != null)
				{
					Tip = "Display Tooltip for this Button's Skill or Item here!" + "\n";
					Tip += "\n" + "Name: " + Info.Icon;
					if(Info.Stackable)
					{
						Tip += "\n" + "Number of Stacks: " + Info.Stack;
					}
					Tip += "\n" + "Cooldown: " + Info.CooldownAmount;
					if(Info.OnCooldown == true)
					{
						Tip += "\n" + "Cooldown Remaining: " + Mathf.Round( (Info.CooldownRemaining) ).ToString();
					}
				}
				else
				{
					Tip = "Empty Button";
				}
				UITooltip.ShowText(Tip);
				return;
			}
			
		}
		UITooltip.ShowText(null);
	}
	
	void Update()
	{
		if(Input.GetKeyUp(HotKey))
		{
			IsClicked();
		}

		if(Info != null)
		{
			if(Info.OnCooldown)
			{
				CooldownSprite.fillAmount = Info.CoolDownFill;
				if(ActionBarSettings.Instance.DisplayCooldownTimer == true)
				{
					CooldownLabel.text = Mathf.Round( (Info.CooldownRemaining) ).ToString();
				}
				else
				{
					CooldownLabel.text = "" ;
				}
			}
			else if(CooldownSprite.fillAmount > 0F)
			{
				if(Info.Disabled == false)
				{
					CooldownSprite.fillAmount = 0F;
				}
				CooldownLabel.text = "" ;
			}
			
			if(Info.Disabled == true)
			{
				if(Info.OnCooldown == false)
				{
					SetFilled(1F);
				}
				
				if(Info.DestroyOnZeroStacks == true)
				{
					
					if(ActionBarCursor.Instance.ButtonInfo != null)
					{
						if(ActionBarCursor.Instance.ButtonInfo.Disabled == true)
						{
							ActionBarCursor.Instance.Clear();
						}
					}
					RemoveInfo();
				}


			}

		}
	}
	
	//Called when the Item/Spell is clicked
	void IsClicked()
	{
		if(Info != null)
		{
			if( (Info.OnCooldown == false) && (Info.Disabled == false) )
				{
					//Tells which Info was clicked
					Info.isClicked = true; 
					//Call the primary select function (Activates the spell/item, User defines what happens there)
					Info.OnSelected(); 
					NGUITools.PlaySound(ActionBarSettings.Instance.ButtonClickSound_Success);
					if(Info.Stackable == true)
					{
						//Reduce Stack of item by 1
						Info.Stack--;
						
						foreach( ActionBarButton mButtons in ActionBarSettings.Instance.Buttons)
						{
							if(mButtons != null)
							{
								if(mButtons.Info != null)
								{
									if(mButtons.Info.Icon.Equals(Info.Icon))
									{
										if(Info.ActivateAbility == true) //If skill is an activatable skill (not passive)
										{
											StartCoroutine(mButtons.Info.StartCooldown());
										}
									}
								}
							}
						}
					}
					if(Info.ActivateAbility == true)//If skill is an activatable skill (not passive)
					{
						StartCoroutine(Info.StartCooldown()); //Starts cooldown for all Buttons in the set
					}
				}
			else
			{
				NGUITools.PlaySound(ActionBarSettings.Instance.ButtonClickSound_Disable);
			}	
		}
		
		else
		{
			NGUITools.PlaySound(ActionBarSettings.Instance.ButtonClickSound_Disable);
		}
	}
	
	//Assign new Item/Spell to the Button
	public ActionBarInfo SetInfo(ActionBarInfo ButtonInformation) 
	{
		if (ButtonInformation == null)
        {
			RemoveInfo();
            return null;
        }
        ActionBarInfo temp = Info;
        Info = ButtonInformation;
		Info.Buttons.Add(this);
		CooldownSprite.fillDirection = Info.FillDirection;
        SetTexture(ActionBarSettings.Instance.Atlas[Info.Atlas], Info.Icon);
        if(ButtonInformation.Stackable)
		{
			Stack = ButtonInformation.Stack;
		}
		else
		{
			Stack = 0;
		}
        if (temp != null)
        {
            temp.Buttons.Remove(this);
        }
        return temp;
	}
	//Set the Texture of the Button
	public void SetTexture(UIAtlas Atlas,string SpriteName)  
	{
		Icon.atlas = Atlas;
		Icon.spriteName = SpriteName;
	}
	//Sets the fill amount on the cooldown Sprite
	public void SetFilled(float FillAmount)
	{
		CooldownSprite.fillAmount = FillAmount;
	}
	//Sets Stack text
	public int Stack
    {
        get { return Info.Stack;}
        set
        {
            if (value <= 1 && !DisplayOneStack)
            {
                StackLabel.text = "";
            }
            else
            {
                StackLabel.text = value.ToString();
            }
			
			if(Info.Stack > 0){SetFilled (0F);}
        }
    }
	//Remove Item from button
	public void RemoveInfo() 
	{
		StackLabel.text = "";
		Icon.atlas = null;
		Icon.spriteName = "";
		Info.CoolDownFill = CooldownSprite.fillAmount;
		CooldownLabel.text = "";
		CooldownSprite.fillAmount = 0F;
		Info.Buttons.Remove(this);
        Info = null;
	} 
	//Checks ID of Item vs the Row
	bool CheckGroupID(int _GroupID, int[] Group) 
	{
		for(int i = 0; i< Group.Length; i++)
		{
			if(_GroupID == Group[i])
			{
				return true;
			}
		}
		return false;
	}
	//Create cooldown effect
	public void InstantiateCooldownEffect() 
	{
		if(Info.PlayCooldownAnimation == true)
		{
			ActionBarSettings.Instance.CooldownEffect.transform.GetChild(0).transform.localScale = new Vector3(BarRow.ButtonSize.x, BarRow.ButtonSize.y, 1F);
			NGUITools.AddChild(this.gameObject, ActionBarSettings.Instance.CooldownEffect);
		}
	}


	
}
