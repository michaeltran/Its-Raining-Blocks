/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;
using System.Xml;
using System;
using System.Linq;
using System.Text.RegularExpressions;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[CustomEditor(typeof(MadLevelConfiguration))]
public class MadLevelConfigurationInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    MadLevelConfiguration configuration;
    
    MadGUI.ScrollableList<LevelItem> list;
    List<LevelItem> items;
    
    static bool texturesLoaded;
    static Texture textureOther;
    static Texture textureLevel;
    static Texture textureExtra;
    static Texture textureError;
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnEnable() {
        configuration = target as MadLevelConfiguration;
    
        items = new List<LevelItem>();
    
        list = new MadGUI.ScrollableList<LevelItem>(items);
        list.label = "Level List";
        list.selectionEnabled = true;
        
        list.selectionCallback = (item) => ItemSelected(item);
    }
    
    void LoadTextures() {
        if (texturesLoaded) {
            return;
        }
        
        textureOther = Resources.Load("MadLevelManager/Textures/icon_other") as Texture;
        textureLevel = Resources.Load("MadLevelManager/Textures/icon_level") as Texture;
        textureExtra = Resources.Load("MadLevelManager/Textures/icon_extra") as Texture;
        textureError = Resources.Load("MadLevelManager/Textures/icon_error") as Texture;
        
        texturesLoaded = true;
    }
    
    void ItemSelected(LevelItem item) {
        Repaint();
        var focusedControl = GUI.GetNameOfFocusedControl();
        if (!string.IsNullOrEmpty(focusedControl)) {
            GUI.SetNextControlName("");
            GUI.FocusControl("");
        }
    }
    
    public override void OnInspectorGUI() {
        LoadTextures(); // loading textures with delay to prevent editor errors
        CheckAssetLocation();
        ActiveInfo();

        LoadItems();    
        list.Draw();
        
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.green;
        if (GUILayout.Button("Add")) {
            AddLevel();
        }
        GUI.color = Color.white;
        
        GUI.enabled = list.selectedItem != null;
        GUI.color = Color.red;
        if (GUILayout.Button("Remove")) {
            RemoveLevel();
        }
        GUI.color = Color.white;
        
        GUILayout.FlexibleSpace();
        
        string downLabel = "Move Down";
        if (GUILayout.Button(downLabel)) {
            MoveDown();
            configuration.SetDirty();
        }
        
        string upLabel = "Move Up";
        if (GUILayout.Button(upLabel)) {
            MoveUp();
            configuration.SetDirty();
        }
        
        GUI.enabled = true;
        
        EditorGUILayout.EndHorizontal();
        
        MadGUI.IndentBox("Level Properties", () => {
            var item = list.selectedItem;
            
            if (item == null) {
                item = new LevelItem(configuration);
                GUI.enabled = false;
            }
            
            MadUndo.RecordObject(configuration, "Edit '" + item.level.name + "'");
            EditorGUI.BeginChangeCheck();
            
            MadGUI.Validate(() => item.level.sceneObject != null, () => {
                item.level.sceneObject =
                    EditorGUILayout.ObjectField("Scene", item.level.sceneObject, typeof(UnityEngine.Object), false);
            });
            if (!CheckAssetIsScene(item.level.sceneObject)) {
                item.level.sceneObject = null;
            }
            
            MadGUI.Validate(() => !string.IsNullOrEmpty(item.level.name), () => {
                GUI.SetNextControlName("level name"); // needs names to unfocus
                item.level.name = EditorGUILayout.TextField("Level Name", item.level.name);
            });
            
            item.level.type = (MadLevel.Type) EditorGUILayout.EnumPopup("Type", item.level.type);
            
            GUI.SetNextControlName("arguments"); // needs names to unfocus
            item.level.arguments = EditorGUILayout.TextField("Arguments", item.level.arguments);
            
            if (EditorGUI.EndChangeCheck()) {
                configuration.SetDirty();
            }
            
            GUI.enabled = true;
            
        });
        
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Help")) {
            Help.BrowseURL("http://redmine.madpixelmachine.com/projects/mad-level-manager/wiki/Creating_Level_Configuration");
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        if (!configuration.IsValid()) {
            MadGUI.Error("Configuration is invalid. Please insect entries marked with \"!!!\" and make sure that " +
                "\"Scene\" and \"Level Name\" are set and level names are not duplicated.");
        }
        
        if (configuration.active && !configuration.CheckBuildSynchronized()) {
            if (MadGUI.ErrorFix(
                "Build configuration is not in synch with level configuration.",
                "Synchronize Now")) {
                configuration.SynchronizeBuild();
            }
        }
    }
    
    bool CheckAssetIsScene(UnityEngine.Object obj) {
        if (obj == null) {
            return false;
        }
    
        string path = AssetDatabase.GetAssetPath(obj);
        if (!path.EndsWith(".unity")) {
            EditorUtility.DisplayDialog(
                "Only scene allowed",
                "You can only drop scene files into this field.",
                "OK");
                
            return false;
        }
        
        return true;
    }
    
    void AddLevel() {
        MadUndo.RecordObject2(configuration, "Add Level");
        
        var levelItem = new LevelItem(configuration);
        
        LevelItem template = null;
        
        if (list.selectedItem != null) {
            template = list.selectedItem;
        } else if (items.Count > 0) {
            template = items.Last();
        }
        
        if (template != null) {
            levelItem.level.order = template.level.order + 1;
            
            levelItem.level.name = IncreaseNumber(template.level.name);
            levelItem.level.sceneObject = template.level.sceneObject;
            levelItem.level.type = template.level.type;
        } else {
            levelItem.level.order = 0;
            
            levelItem.level.name = "New Level";
            levelItem.level.type = MadLevel.Type.Level;
        } 
        
        items.Add(levelItem);
        configuration.levels.Add(levelItem.level);
        
        Reorder();
        
        configuration.SetDirty();
        
        list.selectedItem = levelItem;
        list.ScrollToItem(levelItem);
    }
    
    string IncreaseNumber(string name) {
        int num = 1;
        var match = Regex.Match(name, @".* \(([0-9]+)\)$");
        if (match.Success) {
            var numStr = match.Groups[1].Value;
            num = int.Parse(numStr) + 1;
            name = name.Substring(0, name.Length - (3 + numStr.Length));
        }
        
        return name + " (" + num + ")";
    }
    
    void RemoveLevel() {
        MadUndo.RecordObject2(configuration, "Remove Level");
        configuration.levels.Remove(list.selectedItem.level);
        items.Remove(list.selectedItem);
        Reorder();
        configuration.SetDirty();
        
        list.selectedItem = null;
    }
    
    void ActiveInfo() {
        bool assetLocationRight = IsAssetLocationRight();
        GUI.enabled = assetLocationRight;
    
        var active = MadLevelConfiguration.FindActive();
        if (active == configuration) {
            MadGUI.Info("This is the active configuration.");
        } else {
            string additional = "";
            if (!assetLocationRight) {
                additional = " (But first you must relocate this asset. Please look at the other error.)";
            }
        
            int choice = MadGUI.MessageWithButtonMulti("This configuration is not active. "
                                                  + "It's not currently used to manage your scenes." + additional, MessageType.Warning, "Select Active", "Activate This One");
            
            if (choice == 0) {
                var currentlyActive = MadLevelConfiguration.FindActive();
                if (currentlyActive != null) {
                    EditorGUIUtility.PingObject(currentlyActive);
                } else {
                    EditorUtility.DisplayDialog("Not Found", "No level configuration is active at the moment", "OK");
                }
            } else if (choice == 1) {
                configuration.active = true;
            }
        }
        GUI.enabled = true;
    }
    
    void CheckAssetLocation() {
        if (!IsAssetLocationRight()) {
            if (MadGUI.ErrorFix(
                "Configuration should be placed in Resources/LevelConfig directory", "Where it is now?")) {
                EditorGUIUtility.PingObject(configuration);   
            }
        }
    }
    
    bool IsAssetLocationRight() {
        var configurationPath = AssetDatabase.GetAssetPath(configuration);
        return configurationPath.EndsWith(string.Format("Resources/LevelConfig/{0}.asset", configuration.name));
    }
    
    void LoadItems() {
        if (items.Count != configuration.levels.Count) {
        
            items.Clear();
            foreach (var level in configuration.levels) {
                var item = new LevelItem(level);
                items.Add(item);
            }
            
        }
        
        Reorder();
    }
    
    void Reorder() {
        items.Sort((a, b) => {
            return a.level.order.CompareTo(b.level.order);
        });
        
        int i = 0;
        foreach (var item in items) {
            item.level.order = i;
            i += 10;
        }
    }
    
    void MoveUp() {
        MadUndo.RecordObject2(configuration, "Move '" + list.selectedItem.level.name + "' Up");
        MoveWith(-1);
    }
    
    void MoveDown() {
        MadUndo.RecordObject2(configuration, "Move '" + list.selectedItem.level.name + "' Down");
        MoveWith(1);
    }
    
    void MoveWith(int delta) {
        int index = items.IndexOf(list.selectedItem);
        int otherIndex = index + delta;
        
        if (otherIndex >= 0 && otherIndex < items.Count) {
            int order = list.selectedItem.level.order;
            list.selectedItem.level.order = items[otherIndex].level.order;
            items[otherIndex].level.order = order;
            
            Reorder();
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================
    
    [MenuItem("Assets/Create/Level Configuration")]
    public static void CreateAsset() {
        MadAssets.CreateAsset<MadLevelConfiguration>("New Configuration");
    }
    
    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    private class LevelItem : MadGUI.ScrollableListItem {
        public MadLevelConfiguration.Level level;
    
        public LevelItem(MadLevelConfiguration configuration) {
            this.level = configuration.CreateLevel();
        }
    
        public LevelItem(MadLevelConfiguration.Level level) {
            this.level = level;
        }
        
        public override void OnGUI() {
            var rect = EditorGUILayout.BeginHorizontal();
            
            GUILayout.Space(20);
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(level.name);
            
            Color origColor = GUI.color;
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.5f);
            EditorGUILayout.LabelField(level.sceneName);
            GUI.color = origColor;
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            
            Texture texture = textureOther;
            switch (level.type) {
                case MadLevel.Type.Other:
                    texture = textureOther;
                    break;
                case MadLevel.Type.Level:
                    texture = textureLevel;
                    break;
                case MadLevel.Type.Extra:
                    texture = textureExtra;
                    break;
                    
                default:
                    Debug.LogError("Unknown level type: " + level.type);
                    break;
            }
            
            if (!level.IsValid()) {
                texture = textureError;
            }
            
            GUI.DrawTexture(new Rect(rect.x, rect.y, 28, 34), texture);
            EditorGUILayout.EndHorizontal();
            
        }
    }

}

#if !UNITY_3_5
} // namespace
#endif