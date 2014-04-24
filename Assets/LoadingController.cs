using UnityEngine;
using System.Collections;

public class LoadingController : MonoBehaviour {
	private UISprite loadingLogoSprite;
	
	void Start() {
		loadingLogoSprite = GetComponent<UISprite>();
		//enableLoadingLogo(false);
	}
	
	void Update() {
		if(Application.isLoadingLevel) {
			enableLoadingLogo(true);
		} else {
			enableLoadingLogo(true);
		}
	}
	
	void enableLoadingLogo(bool value) {
		if(loadingLogoSprite != null) {
			loadingLogoSprite.color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f), Random.Range(0.0f,1.0f));
			loadingLogoSprite.enabled = value;
		}
	}
}
