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

[CustomEditor(typeof(MadLevelBackgroundLayer))]
public class MadLevelBackgroundLayerInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty texture;
    SerializedProperty tint;
    SerializedProperty scaleMode;
    SerializedProperty scale;
    SerializedProperty align;
    SerializedProperty position;
    SerializedProperty followSpeed;
    SerializedProperty scrollSpeed;
    
    MadLevelBackgroundLayer layer;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnEnable() {
        layer = target as MadLevelBackgroundLayer;
    
        texture = serializedObject.FindProperty("texture");
        tint = serializedObject.FindProperty("tint");
        scaleMode = serializedObject.FindProperty("scaleMode");
        scale = serializedObject.FindProperty("scale");
        align = serializedObject.FindProperty("align");
        position = serializedObject.FindProperty("position");
        followSpeed = serializedObject.FindProperty("followSpeed");
        scrollSpeed = serializedObject.FindProperty("scrollSpeed");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();
        
        GUI.color = Color.yellow;
        if (GUILayout.Button("<< Back To Layer Listing")) {
            Selection.activeGameObject = layer.parent.gameObject;
        }
        GUI.color = Color.white;
        GUILayout.Space(16);
        
        MadGUI.PropertyField(texture, "Texture");
        MadGUI.PropertyField(tint, "Tint");
        
        EditorGUILayout.Space();
        
        MadGUI.PropertyField(scaleMode, "Scale Mode");
        
        if (scaleMode.enumValueIndex == (int) MadLevelBackgroundLayer.ScaleMode.Manual) {
            MadGUI.PropertyField(align, "Align");
            EditorGUILayout.Space();
            MadGUI.PropertyFieldVector2Compact(position, "Position", 70);
            MadGUI.PropertyFieldVector2Compact(scale, "Scale", 70);
        } else {
            MadGUI.PropertyFieldVector2Compact(position, "Position", 70);
        }
        
        EditorGUILayout.Space();
        
        MadGUI.PropertyField(followSpeed, "Follow Speed");
        MadGUI.PropertyFieldVector2Compact(scrollSpeed, "Auto Scroll", 70);
        
        if (serializedObject.ApplyModifiedProperties()) {
            layer.SetDirty();
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