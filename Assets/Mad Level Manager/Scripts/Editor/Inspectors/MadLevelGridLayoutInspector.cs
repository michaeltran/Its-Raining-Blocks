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
 
[CustomEditor(typeof(MadLevelGridLayout))]
public class MadLevelGridLayoutInspector : MadLevelAbstractLayoutInspector {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty rightSlideSprite;
    SerializedProperty leftSlideSprite;
    
    SerializedProperty iconScale;
    SerializedProperty iconOffset;
    
    SerializedProperty leftSlideScale;
    SerializedProperty leftSlideOffset;
    
    SerializedProperty rightSlideScale;
    SerializedProperty rightSlideOffset;
    
    SerializedProperty gridWidth;
    SerializedProperty gridHeight;
    
    SerializedProperty pixelsWidth;
    SerializedProperty pixelsHeight;
    
    SerializedProperty pagesOffsetFromResolution;
    SerializedProperty pagesOffsetManual;
    
    SerializedProperty lookAtLastLevel;
    
    SerializedProperty hideManagerdObjects;

    MadLevelGridLayout script;        

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    bool generate {
        get {
            return script.setupMethod == MadLevelGridLayout.SetupMethod.Generate;
        }
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    protected override void OnEnable() {
        base.OnEnable();
        script = target as MadLevelGridLayout;
    
        rightSlideSprite = serializedObject.FindProperty("rightSlideSprite");
        leftSlideSprite = serializedObject.FindProperty("leftSlideSprite");
        
        iconScale = serializedObject.FindProperty("iconScale");
        iconOffset = serializedObject.FindProperty("iconOffset");
        
        leftSlideScale = serializedObject.FindProperty("leftSlideScale");
        leftSlideOffset = serializedObject.FindProperty("leftSlideOffset");
        
        rightSlideScale = serializedObject.FindProperty("rightSlideScale");
        rightSlideOffset = serializedObject.FindProperty("rightSlideOffset");
        
        gridWidth = serializedObject.FindProperty("gridWidth");
        gridHeight = serializedObject.FindProperty("gridHeight");
        
        pixelsWidth = serializedObject.FindProperty("pixelsWidth");
        pixelsHeight = serializedObject.FindProperty("pixelsHeight");
        
        pagesOffsetFromResolution = serializedObject.FindProperty("pagesOffsetFromResolution");
        pagesOffsetManual = serializedObject.FindProperty("pagesOffsetManual");
        
        lookAtLastLevel = serializedObject.FindProperty("lookAtLastLevel");
        
        configuration = serializedObject.FindProperty("configuration");
        hideManagerdObjects = serializedObject.FindProperty("hideManagedObjects");
    }

    MadLevelGridLayout.SetupMethod newSetupMethod;

    public override void OnInspectorGUI() {
        newSetupMethod = (MadLevelGridLayout.SetupMethod) EditorGUILayout.EnumPopup("Setup Method", script.setupMethod);
        if (newSetupMethod != script.setupMethod) {
            if (newSetupMethod == MadLevelGridLayout.SetupMethod.Generate && EditorUtility.DisplayDialog(
                "Are you sure?",
                "Are you sure that you want to switch to Generate setup method? If you've made any changes to grid "
                + "object, these changes will be lost!", "Set to Generate", "Cancel")) {
                    script.setupMethod = newSetupMethod;
                    script.deepClean = true;
                    EditorUtility.SetDirty(script);
                } else {
                    script.setupMethod = newSetupMethod;
                    EditorUtility.SetDirty(script);
                }
        }
    
        serializedObject.UpdateIfDirtyOrScript();
        
        if (script.setupMethod == MadLevelGridLayout.SetupMethod.Generate) {
            RebuildButton();
        }
        
        GUILayout.Label("Fundaments", "HeaderLabel");
        
        MadGUI.Indent(() => {
        
            MadGUI.PropertyField(configuration, "Configuration", MadGUI.ObjectIsSet);
            
            GUI.enabled = generate;
            
            MadGUI.PropertyField(iconTemplate, "Icon Template", MadGUI.ObjectIsSet);
            MadGUI.Indent(() => {
                MadGUI.PropertyFieldVector2(iconScale, "Scale");
                MadGUI.PropertyFieldVector2(iconOffset, "Offset");
            });
            
            GUI.enabled = true;
            
            MadGUI.PropertyField(leftSlideSprite, "Prev Page Sprite");
            MadGUI.Indent(() => {
                MadGUI.PropertyFieldVector2(leftSlideScale, "Scale");
                MadGUI.PropertyFieldVector2(leftSlideOffset, "Offset");
            });
                
            MadGUI.PropertyField(rightSlideSprite, "Next Page Sprite");
            MadGUI.Indent(() => {
                MadGUI.PropertyFieldVector2(rightSlideScale, "Scale");
                MadGUI.PropertyFieldVector2(rightSlideOffset, "Offset");
            });
        
        });
        
        GUILayout.Label("Dimensions", "HeaderLabel");
        
        MadGUI.Indent(() => {
            MadGUI.PropertyField(pixelsWidth, "Pixels Width");
            MadGUI.PropertyField(pixelsHeight, "Pixels Height");
            GUI.enabled = generate;
            MadGUI.PropertyField(gridHeight, "Grid Rows");
            MadGUI.PropertyField(gridWidth, "Grid Columns");
            GUI.enabled = true;
            MadGUI.PropertyField(pagesOffsetFromResolution, "Page Offset Auto");
            MadGUI.ConditionallyEnabled(!pagesOffsetFromResolution.boolValue, () => {
                MadGUI.Indent(() => {
                    MadGUI.PropertyField(pagesOffsetManual, "Pixels Offset");
                });
            });
        });
        
        GUILayout.Label("Mechanics", "HeaderLabel");
        
        MadGUI.Indent(() => {
            MadGUI.PropertyField(lookAtLastLevel, "Look At Last Level", "When scene is loaded, it will automatically "
                + "go to the page of previously played level (but only if previous scene is of type Level.");
            HandleMobileBack();
            TwoStepActivation();
        });
        
        GUILayout.Label("Debug", "HeaderLabel");
        
        MadGUI.Indent(() => {
            MadGUI.PropertyField(hideManagerdObjects, "Hide Managed",
                "Hides managed by Mad Level Manager objects from the Hierarchy. If you want to have a look at what the hierarchy "
                + "looks like exacly, you can unckeck this option. Be aware that all direct changes to generated "
                + "objects will be lost!");
        });

        serializedObject.ApplyModifiedProperties();
    }
    
    void RebuildButton() {
        GUILayout.Label("Rebuild", "HeaderLabel");
        MadGUI.Indent(() => {
            MadGUI.Info("In a case when you've changed something, and result is not available on the screen, "
                + "please hit this button.");
        
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            GUI.color = Color.green;
            if (GUILayout.Button("Rebuild Now")) {
                script.dirty = true;
                script.deepClean = true;
                EditorUtility.SetDirty(script);
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        });
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