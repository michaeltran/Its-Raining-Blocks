/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
[RequireComponent(typeof(MadSprite))]   
public class MadLevelBackgroundLayer : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================
    
    public const float ScrollSpeedMultiplier = 0.01f;

    // ===========================================================
    // Fields
    // ===========================================================
    
    public Texture2D texture;
    public Color tint = Color.white;
    
    public Vector2 scale = Vector2.one;
    public ScaleMode scaleMode;
    public Align align = Align.Middle;
    
    public Vector2 position = Vector2.zero;
    
    public float followSpeed = 1;
    public Vector2 scrollSpeed; // texture lengths per second
    
    Vector2 scrollAccel;
    
    MadRootNode _root;
    MadRootNode root {
        get {
            if (_root == null) {
                _root = MadTransform.FindParent<MadRootNode>(transform);
            }
            
            return _root;
        }
    }
    
    MadLevelBackground _parent;
    public MadLevelBackground parent {
        get {
            if (_parent == null) {
                _parent = MadTransform.FindParent<MadLevelBackground>(transform);
            }
            
            return _parent;
        }
    }
    
    
    MadSprite _sprite;
    MadSprite sprite {
        get {
            if (_sprite == null) {
                _sprite = GetComponent<MadSprite>();
            }
            
            return _sprite;
        }
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    void Start() {
        SetDirty();
    }
    
    public void SetDirty() {
        sprite.texture = texture;
        sprite.tint = tint;
        
        int index = parent.IndexOf(this);
        name = string.Format("{0:D2} layer ({1})", index, texture != null ? texture.name : "empty");
    }
    
    public void Cleanup() {
        if (sprite != null) {
            if (Application.isPlaying) {
                Destroy(gameObject);
            } else {
                MadUndo.DestroyObjectImmediate(gameObject);
            }
        }
    }
    
    public void Update() {
    
        if (sprite == null || sprite.texture == null) {
            return;
        }
        
        float scaleX = scale.x;
        float scaleY = scale.y;
        
        float spriteWidth = root.screenWidth;
        float spriteHeight = root.screenHeight;
        
        switch (scaleMode) {
            case ScaleMode.Fill:
                scaleX = (root.screenHeight / (float) sprite.texture.height)
                    * (sprite.texture.width / (float) root.screenWidth);
                break;
            case ScaleMode.Manual:
                spriteHeight = sprite.texture.height * scaleY;
                scaleX *= sprite.texture.width / (float) root.screenWidth;
                break;
        }
        
    
        // scale to fill whole screen, but keep the scale set by user
        sprite.scalePixels = new Vector2(spriteWidth, spriteHeight);
        
        if (scaleMode == ScaleMode.Manual) {
            switch (align) {
                case Align.None:
                    sprite.transform.localPosition =
                        new Vector3(0, Screen.height * position.y, 0);
                    break;
            
                case Align.Middle:
                    sprite.transform.localPosition =
                        new Vector3(0, position.y, 0);
                    break;
                
                case Align.Bottom:
                    sprite.transform.localPosition =
                        new Vector3(
                            0,
                            root.screenHeight * (- 0.5f) + position.y + (0.5f * scaleY * sprite.texture.height),
                            0);
                    break;
                    
                case Align.Top:
                    sprite.transform.localPosition =
                        new Vector3(
                            0,
                            root.screenHeight * (0.5f) + position.y + (-0.5f * scaleY * sprite.texture.height),
                            0);
                    break;
            }
        } else {
            sprite.transform.localPosition = new Vector3(0, root.screenHeight * position.y, 0);
        }
        
        // set proper repeat
        float rx = 1, ry = 1;
        
        sprite.textureRepeat = new Vector2(rx * (1 / scaleX), ry);
        
        // follow draggable
        var locPos = parent.UserPosition;
        float dx = -locPos.x * followSpeed;
        float dy = -locPos.y * followSpeed;
        
        dx /= Screen.width / rx;
        dy /= Screen.height / ry;
        
        sprite.textureOffset = new Vector2(dx * (1 / scaleX) + position.x, dy);
        
        if (scrollSpeed != Vector2.zero) {
            scrollAccel +=
                new Vector2(
                    scrollSpeed.x * ScrollSpeedMultiplier / scale.x,
                    scrollSpeed.y * ScrollSpeedMultiplier / scale.y)
                * Time.deltaTime;
                
            scrollAccel = new Vector2(scrollAccel.x % 1, scrollAccel.y % 1);
            sprite.textureOffset += scrollAccel;
        }
        
        
    }
       
    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum ScaleMode {
        Manual,
        Fill,
    }
    
    public enum Align {
        None,
        Top,
        Middle,
        Bottom,
    }

}

#if !UNITY_3_5
} // namespace
#endif