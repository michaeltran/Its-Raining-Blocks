using UnityEngine;
using System.Collections;

public class CheckAreaDamage : MonoBehaviour {
	
	private bool didDamage=false;
	// Use this for initialization
	
	public bool _DidDamage()
	{
		if (didDamage==false)	
		{
			didDamage=true;
			return false;	
		}
		return didDamage;
	}
}
