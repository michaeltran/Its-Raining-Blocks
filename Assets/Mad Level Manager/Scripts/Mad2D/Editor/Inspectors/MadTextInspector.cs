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

[CustomEditor(typeof(MadText))]
public class MadTextInspector : MadSpriteInspector {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty font;
    SerializedProperty text;
    SerializedProperty scale;
    SerializedProperty letterSpacing;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    protected new void OnEnable() {
        base.OnEnable();
        
        font = serializedObject.FindProperty("font");
        text = serializedObject.FindProperty("text");
        scale = serializedObject.FindProperty("scale");
        letterSpacing = serializedObject.FindProperty("letterSpacing");
    }

    public override void OnInspectorGUI() {
        SectionSprite(DisplayFlag.WithoutSize | DisplayFlag.WithoutMaterial | DisplayFlag.WithoutFill);
        
        serializedObject.Update();
        MadGUI.PropertyField(font, "Font");
        EditorGUILayout.LabelField("Text");
        text.stringValue = EditorGUILayout.TextArea(text.stringValue);
        MadGUI.PropertyField(scale, "Scale");
        MadGUI.PropertyField(letterSpacing, "Letter Spacing");
        
        serializedObject.ApplyModifiedProperties();
    }
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    List<MadText> TextList() {
        var texts = ((MonoBehaviour) target).GetComponentsInChildren<MadText>();
        return new List<MadText>(texts);
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