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

[CustomEditor(typeof(MadLevelProperty))]
public class MadLevelPropertyInspector : MadLevelManager.MadEditorBase {

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
        base.OnInspectorGUI();
        
        if (Foldout("Property", true)) {
            MadGUI.Indent(() => {
                MadGUI.BeginBox();
                
                var property = target as MadLevelProperty;
                
                EditorGUILayout.LabelField("Property name: " + target.name);
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = !property.propertyEnabled;
                if (GUILayout.Button("Enable")) {
                    property.propertyEnabled = true;
                    EditorUtility.SetDirty(property);
                }
                GUI.enabled = property.propertyEnabled;
                if (GUILayout.Button("Disable")) {
                    property.propertyEnabled = false;
                    EditorUtility.SetDirty(property);
                }
                GUI.enabled = true;
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
                
//                if (!Application.isPlaying) {
//                    if (MessageWithButton("To test animations please enter the Play mode.", "Enter Play Mode", MessageType.Info)) {
//                        EditorApplication.ExecuteMenuItem("Edit/Play");
//                    }
//                    EditorGUILayout.Space();
//                }
                
                MadGUI.EndBox();
            });
        }
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