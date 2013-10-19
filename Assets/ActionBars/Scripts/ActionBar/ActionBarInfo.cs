using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class ActionBarInfo
{
	[SerializeField]
	public int Atlas;
	[SerializeField]
	public string Icon;
	[SerializeField]
	public int GroupID;
	[SerializeField]
	public bool SeperateInstance;
	[SerializeField]
	public bool DestroyOnZeroStacks;
	[SerializeField]
	public float Cooldown;
	[SerializeField]
	public GameObject Target;
	[SerializeField]
	public string Function;
	[SerializeField]
	public bool PlayCooldownAnimation = true;
	[SerializeField]
	public int InstanceNumber;
	[SerializeField]
	public UISprite.FillDirection FillDirection = UISprite.FillDirection.Radial360;
	[SerializeField]
	public bool DisplayOneStack;
	[SerializeField]
	public bool ActivateAbility = true;
	[SerializeField]
	public bool AutoCast = true;
	[SerializeField]
	public bool Stackable = false;
	[SerializeField]
	public int numStacks;
	[SerializeField]
	public bool isClicked = false;
	[SerializeField]
	bool mDisabled = false;
	
    [SerializeField]
    public Action<ActionBarInfo> Callback = null;
	[SerializeField]
    public HashSet<ActionBarButton> Buttons = new HashSet<ActionBarButton>();
	[SerializeField]
	float CooldownBegin;
	[SerializeField]
	public float CoolDownFill;
	[SerializeField]
	public float CooldownAmount;
	
	public void OnSelected()
	{
		if(Target != null)
		{
			Target.SendMessage(Function, Target);// SendMessageOptions.DontRequireReceiver);
		}
	}
	

	public ActionBarInfo SetActionBarInfo(int _Atlas, string _IconName, int _GroupID, bool _Stackable, float _Cooldown)
	{
		Atlas = _Atlas;
		Icon = _IconName;
		GroupID = _GroupID;
		Stackable = _Stackable;
		CooldownAmount = _Cooldown;
		Callback = (d) =>
        {
           d.Cooldown = CooldownAmount;
        };
		InstanceNumber = ActionBarSettings.Instance.GetInstanceNumber();
		return this;
	}
	
	
	//Starts cooldown for item/spell
	public System.Collections.IEnumerator StartCooldown()
	{
		Cooldown = CooldownAmount;//Callback for all instances of the item, starts cooldown for them all.
		float t = 0f;
		CoolDownFill = 1F;
		CooldownBegin = UnityEngine.Time.time;
		while (OnCooldown)
		{
			CoolDownFill = Mathf.Lerp(1F, 0f, t/Cooldown);
			t += Time.deltaTime; 
			yield return null;
		}
		if(Buttons.Count != 0)
		{
			if(isClicked)//Is called on only the button that is clicked. (Don't want multiple sounds at once)
			{
				foreach (ActionBarButton button in Buttons)
	        	{
					button.InstantiateCooldownEffect();
	         		button.SetFilled(0F);
	        	}
				if(PlayCooldownAnimation == true)
				{
					NGUITools.PlaySound(ActionBarSettings.Instance.CooldownFinishSound);
				}
			}
			isClicked = false;
		}
	}
	//Returns cooldown remaining
    public float CooldownRemaining
    {
        get { return CooldownAmount - (UnityEngine.Time.time - CooldownBegin); }
    }
	//Returns if the skill is on cooldow
    public bool OnCooldown
    {
        get { return (UnityEngine.Time.time - CooldownBegin) < Cooldown; }
    }
	//Manages the stacks on the item/spell
	public int Stack
    {
        get { return numStacks; }
        set
        {
			if(value != 0)
			{
				Stackable = true;
			}
	            value = UnityEngine.Mathf.Clamp(value, 0, int.MaxValue);
				
				
				
	            if (Stackable && numStacks != value)
	            {
	                numStacks = value;

	                foreach (ActionBarButton button in Buttons)
	                {
						 button.Stack = numStacks;
	                }
	
	                if (numStacks == 0 && !Disabled)
	                {
	                    Disabled = true;
	                }
	                
	                if(numStacks > 0 && Disabled)
	                {
	                    Disabled = false;
	                }
	            }
			

        }
    }
	//If item/skill is disabled
	public bool Disabled
    {
        get { return mDisabled; }
        set
        {
            mDisabled = value;
        }
    }
	
}
