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
 
[ExecuteInEditMode]   
public class MadAnchor : MadNode {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public Mode mode = Mode.ScreenAnchor;
    
    // anchoring to screen position
    public Position position;
    
    // anchoring to object position
    public GameObject anchorObject;
    public Camera anchorCamera;
    public bool moveIn3D;
    public bool faceCamera;
    
    MadRootNode _root;
    MadRootNode root {
        get {
            if (_root == null) {
                _root = MadTransform.FindParent<MadRootNode>(transform);
            }
            
            return _root;
        }
    }

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    public void Update() {
        switch (mode) {
            case Mode.ScreenAnchor:
                UpdateScreenAnchor();
                break;
            case Mode.ObjectAnchor:
                UpdateObjectAnchor();
                break;
            default:
                MadDebug.Assert(false, "Unknown mode: " + mode);
                break;
        }
    }
    
    void UpdateScreenAnchor() {
        var input = FromPosition(position);
        transform.position = input;
    }
    
    Vector3 FromPosition(Position position) {
        float x = 0, y = 0;
    
        switch (position) {
            case Position.Left:
                x = 0;
                y = 0.5f;
                break;
            case Position.Top:
                y = 1;
                x = 0.5f;
                break;
            case Position.Right:
                x = 1;
                y = 0.5f;
                break;
            case Position.Bottom:
                y = 0;
                x = 0.5f;
                break;
            case Position.TopLeft:
                x = 0;
                y = 1;
                break;
            case Position.TopRight:
                x = 1;
                y = 1;
                break;
            case Position.BottomRight:
                x = 1;
                y = 0;
                break;
            case Position.BottomLeft:
                x = 0;
                y = 0;
                break;
            case Position.Center:
                x = 0.5f;
                y = 0.5f;
                break;
            default:
                MadDebug.Assert(false, "Unknown option: " + position);
                break;
        }
        
        var pos = root.ScreenGlobal(x, y);
        return pos;
    }
    
    void UpdateObjectAnchor() {
        if (anchorObject == null) {
            return;
        }
        
        Camera camera = anchorCamera;
        if (camera == null) {
            if (Application.isPlaying) {
                MadDebug.LogOnce("Anchor camera not set. Using main camera.", this);
            }
            camera = Camera.main;
        }
        
        float z = transform.position.z;
        
        Camera guiCamera = MadTransform.FindParent<Camera>(transform);
        
        var pos = camera.WorldToScreenPoint(anchorObject.transform.position);
        pos = guiCamera.ScreenToWorldPoint(pos);
        
        if (!moveIn3D) {
            pos.z = z;
        }
        
        transform.position = pos;
        
        if (faceCamera) {
            transform.LookAt(anchorCamera.transform);
        }
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum Mode {
        ScreenAnchor,
        ObjectAnchor,
    }
    
    public enum Position {
        Left,
        Top,
        Right,
        Bottom,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,
        Center,
    }

}

#if !UNITY_3_5
} // namespace
#endif