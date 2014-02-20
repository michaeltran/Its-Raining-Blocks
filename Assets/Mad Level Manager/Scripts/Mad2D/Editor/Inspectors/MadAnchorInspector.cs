/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
[CustomEditor(typeof(MadAnchor))]
public class MadAnchorInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    public override void OnInspectorGUI() {
        DrawInspector(serializedObject);
    }

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static void DrawInspector(SerializedObject so) {
        so.Update();
        
        var mode = so.FindProperty("mode");
        var position = so.FindProperty("position");
        var anchorObject = so.FindProperty("anchorObject");
        var anchorCamera = so.FindProperty("anchorCamera");
        var moveIn3D = so.FindProperty("moveIn3D");
        var faceCamera = so.FindProperty("faceCamera");
        
        MadGUI.PropertyField(mode, "Mode");
        switch ((MadAnchor.Mode) mode.enumValueIndex) {
            case MadAnchor.Mode.ObjectAnchor:
                MadGUI.PropertyField(anchorObject, "Anchor Object");
                MadGUI.PropertyField(anchorCamera, "Anchor Camera");
                MadGUI.PropertyField(moveIn3D, "Move In 3D");
                MadGUI.PropertyField(faceCamera, "Face Camera", "Bar graphics will always face anchor camera.");
                break;
                
            case MadAnchor.Mode.ScreenAnchor:
                MadGUI.PropertyField(position, "Position");
                break;
                
            default:
                MadDebug.Assert(false, "Unknown mode: " + (MadAnchor.Mode) mode.enumValueIndex);
                break;
        }
        
        so.ApplyModifiedProperties();
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif