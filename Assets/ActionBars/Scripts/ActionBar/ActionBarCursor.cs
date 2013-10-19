using UnityEngine;

public class ActionBarCursor : MonoBehaviour
{
	public static ActionBarCursor Instance;
	public Camera UICamera;
	public UISprite mSprite;
	public UIAtlas mAtlas;
	string mSpriteName;
	Transform mTrans;
	[HideInInspector]
	public bool isHovered;
	ActionBarInfo Info;
	[HideInInspector]
	public bool PermissionCheck;
	void Awake () { Instance = this; }
	void OnDestroy () { Instance = null; }

	void Start ()
	{
		Info = null;
		PermissionCheck = true;
		isHovered = false;
		mTrans = transform;
		mSprite = GetComponentInChildren<UISprite>();
		mSprite.depth = 100;
		if (UICamera == null) UICamera = NGUITools.FindCameraForLayer(gameObject.layer);
		UICamera.GetComponent<UICamera>().stickyPress = false;
		
	}
	

	void Update ()
	{
		if (mSprite.atlas != null)
		{
			Vector3 pos = Input.mousePosition;

			if (UICamera != null)
			{
				pos.x = Mathf.Clamp01(pos.x / Screen.width);
				pos.y = Mathf.Clamp01(pos.y / Screen.height);
				mTrans.position = UICamera.ViewportToWorldPoint(pos);

				if (UICamera.isOrthoGraphic)
				{
					mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(mTrans.localPosition, mTrans.localScale);
				}
				if(Input.GetMouseButtonUp(0))
				{
					
					if(Info != null)
					{
						if(isHovered == false)
						{
							if(PermissionCheck == true)
							{
								Clear();
								NGUITools.PlaySound(ActionBarSettings.Instance.ButtonSound_Destroyed);

							}
						}
					}
				}
			}
			else
			{
				pos.x -= Screen.width * 0.5f;
				pos.y -= Screen.height * 0.5f;
				mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(pos, mTrans.localScale);
			}
		}
	}
	//Clear Cursor item being held
	public void Clear ()
	{
		Set(null,new Vector2(64F,64F));
	}
	//Set item being held
	public void Set (ActionBarInfo ButtonInfo, Vector2 IconSize)
	{
		if (Instance != null)
		{
			if(ButtonInfo != null)
			{
				Instance.mSprite.transform.localScale = new Vector3(IconSize.x,IconSize.y,1F);
				Info = ButtonInfo;
				Instance.mAtlas = ActionBarSettings.Instance.Atlas[ButtonInfo.Atlas];
				Instance.mSprite.atlas = Instance.mAtlas;
				Instance.mSprite.spriteName = ButtonInfo.Icon;
				Instance.Update();
			}
			else
			{
				Info = null;
				Instance.mSprite.atlas = null;
				Instance.mSprite.spriteName = "";
				Instance.Update();
			}
		}
	}
	public ActionBarInfo ButtonInfo
	{
		get{ return Info;}
	}
}