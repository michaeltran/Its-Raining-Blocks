using UnityEngine;
using System.Collections;

public class ManaBarBasic : MonoBehaviour
{
	public UILabel label;
	
	private UISlider _slider;
	private bool _displayText = true;
	
	void Awake ()
	{
		_slider = GetComponent<UISlider> ();
		if (_slider == null) {
			Debug.LogError ("Could not find the UISliderComponent.");
			return;
		}
		
		DisplayText = _displayText;
	}
	
	public void UpdateDisplay (float x)
	{
		_slider.sliderValue = x;
	}
	
	public void UpdateDisplay(float x, string str){
		UpdateDisplay (x);
		label.text = str;
	}
	
	public bool DisplayText{
		get {return _displayText;}
		set {
			_displayText = value;
			if (!_displayText)
			{
				label.text = "";
			}
		}
	}
}
