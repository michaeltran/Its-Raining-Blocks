/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
[CustomEditor(typeof(MadSprite))]   
public class MadSpriteInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    private SerializedProperty visible;
    private SerializedProperty pivotPoint;
    private SerializedProperty tint;
    private SerializedProperty texture;
    private SerializedProperty textureOffset;
    private SerializedProperty textureRepeat;
    private SerializedProperty guiDepth;
    private SerializedProperty fillType;
    private SerializedProperty fillValue;
    private SerializedProperty radialFillOffset;
    private SerializedProperty radialFillLength;
    
    private MadSprite sprite;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    protected void OnEnable() {
        sprite = target as MadSprite;
    
        visible = serializedObject.FindProperty("visible");
        pivotPoint = serializedObject.FindProperty("pivotPoint");
        tint = serializedObject.FindProperty("tint");
        texture = serializedObject.FindProperty("texture");
        textureOffset = serializedObject.FindProperty("textureOffset");
        textureRepeat = serializedObject.FindProperty("textureRepeat");
        guiDepth = serializedObject.FindProperty("guiDepth");
        fillType = serializedObject.FindProperty("fillType");
        fillValue = serializedObject.FindProperty("fillValue");
        radialFillOffset = serializedObject.FindProperty("radialFillOffset");
        radialFillLength = serializedObject.FindProperty("radialFillLength");
    }
    
    public override void OnInspectorGUI() {
        SectionSprite();
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    protected void SectionSprite() {
        SectionSprite(DisplayFlag.None);
    }
    
    protected void SectionSprite(DisplayFlag flags) {
        serializedObject.Update();
        MadGUI.PropertyField(visible, "Visible");
    
        if ((flags & DisplayFlag.WithoutMaterial) == 0) {
            MadGUI.PropertyField(texture, "Texture", MadGUI.ObjectIsSet);
            MadGUI.Indent(() => {
                MadGUI.PropertyFieldVector2(textureRepeat, "Repeat");
                MadGUI.PropertyFieldVector2(textureOffset, "Offset");
            });
        }
        
        MadGUI.PropertyField(tint, "Tint");
        
        if ((flags & DisplayFlag.WithoutSize) == 0) {
            if (GUILayout.Button(new GUIContent("Resize To Texture",
                "Resizes this sprite to match texture size"))) {
                var sprite = target as MadSprite;
                MadUndo.RecordObject2(sprite, "Resize To Texture");
                sprite.ResizeToTexture();
                EditorUtility.SetDirty(sprite);
            }
        }
        
        MadGUI.PropertyField(pivotPoint, "Pivot Point");
        MadGUI.PropertyField(guiDepth, "GUI Depth");
        
        if ((flags & DisplayFlag.WithoutFill) == 0) {
            MadGUI.PropertyField(fillType, "Fill Type");
            EditorGUILayout.Slider(fillValue, 0, 1, "Fill Value");
            
            if (sprite.fillType == MadSprite.FillType.RadialCCW || sprite.fillType == MadSprite.FillType.RadialCW) {
                MadGUI.PropertyFieldSlider(radialFillOffset, -1, 1, "Offset");
                MadGUI.PropertyFieldSlider(radialFillLength, 0, 1, "Length");
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    [Flags]
    protected enum DisplayFlag {
        None = 0,
        WithoutSize = 1,
        WithoutMaterial = 2,
        WithoutFill = 4,
    }

}

#if !UNITY_3_5
} // namespace
#endif