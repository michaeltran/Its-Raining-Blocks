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

[CustomEditor(typeof(MadFreeDraggable))]
public class MadFreeDraggableInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty dragArea;
    SerializedProperty dragStartPosition;
    
    SerializedProperty scaling;
    SerializedProperty scalingMin;
    SerializedProperty scalingMax;
    
    SerializedProperty moveEasing;
    SerializedProperty moveEasingType;
    SerializedProperty moveEasingDuration;
    
    SerializedProperty scaleEasing;
    SerializedProperty scaleEasingType;
    SerializedProperty scaleEasingDuration;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void OnEnable() {
        dragArea = serializedObject.FindProperty("dragArea");
        dragStartPosition = serializedObject.FindProperty("dragStartPosition");
        
        scaling = serializedObject.FindProperty("scaling");
        scalingMin = serializedObject.FindProperty("scalingMin");
        scalingMax = serializedObject.FindProperty("scalingMax");
        
        moveEasing = serializedObject.FindProperty("moveEasing");
        moveEasingType = serializedObject.FindProperty("moveEasingType");
        moveEasingDuration = serializedObject.FindProperty("moveEasingDuration");
        
        scaleEasing = serializedObject.FindProperty("scaleEasing");
        scaleEasingType = serializedObject.FindProperty("scaleEasingType");
        scaleEasingDuration = serializedObject.FindProperty("scaleEasingDuration");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();
    
        MadGUI.PropertyField(dragArea, "Drag Area");
        MadGUI.PropertyFieldVector2(dragStartPosition, "Drag Start Position");
        
        MadGUI.PropertyField(scaling, "Allow Scaling");
        MadGUI.ConditionallyEnabled(scaling.boolValue, () => {
            MadGUI.Indent(() => {
                MadGUI.PropertyField(scalingMin, "Scaling Min");
                MadGUI.PropertyField(scalingMax, "Scaling Max");
            });
        });
        
        MadGUI.PropertyField(moveEasing, "Move Easing");
        MadGUI.ConditionallyEnabled(moveEasing.boolValue, () => {
            MadGUI.Indent(() => {
                MadGUI.PropertyField(moveEasingType, "Type");
                MadGUI.PropertyField(moveEasingDuration, "Duration");
            });
        });
        
        MadGUI.PropertyField(scaleEasing, "Scale Easing");
        MadGUI.ConditionallyEnabled(scaleEasing.boolValue, () => {
            MadGUI.Indent(() => {
                MadGUI.PropertyField(scaleEasingType, "Type");
                MadGUI.PropertyField(scaleEasingDuration, "Duration");
            });
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