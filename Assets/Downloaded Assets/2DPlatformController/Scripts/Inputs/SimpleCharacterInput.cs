using UnityEngine;
using System.Collections;

/// <summary>
/// A simple character input. Arrows to move, left SHIFT to run, SPACE to jump.
/// </summary>
public class SimpleCharacterInput : RaycastCharacterInput
{

	public bool alwaysRun;

	void Update ()
	{
		
		if (Input.GetKey(KeyCode.R)) {
			Application.LoadLevel(0);
		}
		
		jumpButtonHeld = false;
		jumpButtonDown = false;
		x = 0;
		y = 0;
		
		if (Input.GetKey("right") ) {
			x = 0.5f;
		} else if (Input.GetKey("left") ) {
			x = -0.5f;
		}
	
		// Shift to run
		if (alwaysRun || Input.GetKey(KeyCode.LeftShift)) {
			x *= 2;
		}
		
		if (Input.GetKey("up") ) {
			y = 1;
		} else if (Input.GetKey("down") ) {
			y = -1;
		}
		
		if (Input.GetKey(KeyCode.Space) ) {
			jumpButtonHeld = true;
			if (Input.GetKeyDown(KeyCode.Space)) {
				jumpButtonDown = true;		
			} else {
				jumpButtonDown = false;		
			}
		} else {
			jumpButtonDown = false;
		}
	}
	
}

