using UnityEngine;
using System.Collections;
using MadLevelManager;

public class LoadingControllerMAD : MonoBehaviour {
	private MadSprite loadingLogoSprite;
	
	void Start() { 
		loadingLogoSprite = GetComponent<MadSprite>();
		enableLoadingLogo(false);
	}
	
	void Update() {
		if(Application.isLoadingLevel) {
			enableLoadingLogo(true);
		} else {
			enableLoadingLogo(false);
		}
	}
	
	void enableLoadingLogo(bool value) {
		if(loadingLogoSprite != null) {
			loadingLogoSprite.tint = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f), Random.Range(0.0f,1.0f));
			loadingLogoSprite.enabled = value;
		}
	}
}
