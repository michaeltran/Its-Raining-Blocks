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

[CustomEditor(typeof(MadLevelFreeLayout))]
public class MadLevelFreeLayoutInspector : MadLevelAbstractLayoutInspector {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    // won't display offset because it's used to placing icons and it's mostly useless for users
//    SerializedProperty offset;
    
    SerializedProperty backgroundTexture;
    
    SerializedProperty lookAtLastLevel;
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    protected override void OnEnable() {
        base.OnEnable();
        backgroundTexture = serializedObject.FindProperty("backgroundTexture");
        
        lookAtLastLevel = serializedObject.FindProperty("lookAtLastLevel");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();
        
        GUILayout.Label("Fundaments", "HeaderLabel");
        MadGUI.Indent(() => {
            MadGUI.PropertyField(configuration, "Configuration", MadGUI.ObjectIsSet);
            MadGUI.PropertyField(iconTemplate, "Icon Template", MadGUI.ObjectIsSet);
            MadGUI.PropertyField(backgroundTexture, "Background Texture");
        });
        
        GUILayout.Label("Mechanics", "HeaderLabel");
        
        MadGUI.Indent(() => {
            MadGUI.PropertyField(lookAtLastLevel, "Look At Last Level", "When scene is loaded, it will automatically "
                                 + "go to the previously played level (but only if previous scene is of type Level.");
            HandleMobileBack();
            TwoStepActivation();
        });
        
        serializedObject.ApplyModifiedProperties();
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif