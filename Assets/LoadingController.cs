using UnityEngine;
using System.Collections;

public class LoadingController : MonoBehaviour {
	public GameObject objectsToDisable;
	private UISprite loadingLogoSprite;
	
	void Start() { 
		loadingLogoSprite = GetComponent<UISprite>();
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
			loadingLogoSprite.color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f), Random.Range(0.0f,1.0f));
			loadingLogoSprite.enabled = value;
		}
	}
}
