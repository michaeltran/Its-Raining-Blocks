using UnityEngine;
using System.Collections;

public class ActionBarCooldownParticle: MonoBehaviour
{
    float startTime;
    public Color color;
    public float Duration = 0.5f;
    public float Size_Start = 0f;
    public float Size_End;
    public float Rotations = 2F;
	public UISprite CooldownSprite;
    void Start()
    {
        startTime = Time.time;
        GameObject.Destroy(gameObject.transform.parent.gameObject, Duration);
    }

    void Update()
    {
        float time = ((Time.time - startTime) / Duration);
        float size = Mathf.Lerp(Size_Start, Size_End, time);
		
		transform.localScale = new Vector3(size, size, 1);
		
        transform.rotation = Quaternion.Euler(0, 0, 360f * Rotations * time);
        
		CooldownSprite.color = new Color(color.r, color.b, color.g, 1 - (time/2));
    }
}
