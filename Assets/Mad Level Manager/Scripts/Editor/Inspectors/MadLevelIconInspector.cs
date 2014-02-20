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

[CustomEditor(typeof(MadLevelIcon))]
public class MadLevelIconInspector : MadSpriteInspector {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty levelSceneName;
    SerializedProperty unlockOnComplete;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    protected new void OnEnable() {
        base.OnEnable();
        
        levelSceneName = serializedObject.FindProperty("levelSceneName");
        unlockOnComplete = serializedObject.FindProperty("unlockOnComplete");
    }

    public override void OnInspectorGUI() {
        var levelIcon = target as MadLevelIcon;
        
        MadGUI.BeginBox("Properties");
        MadGUI.Indent(() => {
            var properties = PropertyList();
            foreach (MadLevelProperty property in properties) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(property.name, GUILayout.Width(170));
                
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
                
                GUILayout.EndHorizontal();
            }
        });
        MadGUI.EndBox();
        
        MadGUI.BeginBox("Level Icon");
        MadGUI.Indent(() => {
            if (levelIcon.hasLevelConfiguration) {
                int levelCount = levelIcon.configuration.LevelCount(MadLevel.Type.Level);
                if (levelCount > levelIcon.levelIndex) {
                    var level = levelIcon.level;
                    MadGUI.Disabled(() => {
                        EditorGUILayout.TextField("Level Name", level.name);
                        EditorGUILayout.TextField("Level Arguments", level.arguments);
                    });
                }
                if (MadGUI.InfoFix("These values are set and managed by level configuration.",
                    "Configuration")) {
                    Selection.objects = new Object[] { levelIcon.configuration };
                }
            } else {
                serializedObject.Update();
                MadGUI.PropertyField(levelSceneName, "Level Scene Name");
                serializedObject.ApplyModifiedProperties();
            }
        
            //
            // Completed property select popup
            //
            MadGUI.PropertyFieldObjectsPopup<MadLevelProperty>(
                target,
                "\"Completed\" Property",
                ref levelIcon.completedProperty,
                PropertyList(),
                false
            );
            
            MadGUI.PropertyFieldObjectsPopup<MadLevelProperty>(
                target,
                "\"Locked\" Property",
                ref levelIcon.lockedProperty,
                PropertyList(),
                false
            );
        
            MadGUI.PropertyFieldObjectsPopup<MadText>(
                target,
                "Level Number Text",
                ref levelIcon.levelNumber,
                TextList(),
                false
            );
            
            serializedObject.Update();
            if (MadGUI.Foldout("Unlock On Complete", false)) {
                var arrayList = new MadGUI.ArrayList<MadLevelIcon>(
                    unlockOnComplete, (p) => { MadGUI.PropertyField(p, ""); });
                arrayList.Draw();
            }
            serializedObject.ApplyModifiedProperties();
        });
        MadGUI.EndBox();
        
        MadGUI.BeginBox("Sprite");
        MadGUI.Indent(() => {
            SectionSprite();
        });
        MadGUI.EndBox();
        
    }
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    List<MadLevelProperty> PropertyList() {
        var properties = ((MonoBehaviour) target).GetComponentsInChildren<MadLevelProperty>();
        return new List<MadLevelProperty>(properties);
    }
    
    List<MadText> TextList() {
        var texts = ((MonoBehaviour) target).GetComponentsInChildren<MadText>();
        return new List<MadText>(texts);
    }
    
    MadLevelProperty GetProperty(string name) {
        var properties = ((MonoBehaviour) target).GetComponentsInChildren<MadLevelProperty>();
        foreach (var property in properties) {
            if (property.name == name) {
                return property;
            }
        }
        
        Debug.LogError("Property " + name + " not found?!");
        return null;
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