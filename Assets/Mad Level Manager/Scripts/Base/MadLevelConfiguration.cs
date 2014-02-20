/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MadLevelManager;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[ExecuteInEditMode]
public class MadLevelConfiguration : ScriptableObject {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    // only one configuration can be active at time
    [SerializeField]
    private bool _active;
    
    public List<Level> levels;
    
    [NonSerialized]
    public Callback0 callbackChanged = () => {};
    
    // to prevent activation of everything that is found
    // sadly OnEnable() activation method can activate too much for some reason
    private static bool automaticallyActivatedSomething;
    
    // ===========================================================
    // Properties
    // ===========================================================
    
    public bool active {
        get {
            return _active;
        }
        
        set {
            _active = value;
            if (value) {
                DeactivateOthers();
            }
            SetDirty();
        }
    }
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    public override int GetHashCode() {
        int hash = 17;
    
        foreach (var level in levels) {
            hash = hash * 31 + level.GetHashCode();
        }
        
        return hash;
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnEnable() {
        Upgrade();
    }
    
    void Upgrade() {
        if (levels != null) {
            foreach(var level in levels) {
                level.parent = this;
                level.Upgrade();
            }
        }
    }
    
    public new void SetDirty() {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
        callbackChanged();
    }
    
    public Level CreateLevel() {
        return new Level(this);
    }
    
    public int LevelCount() {
        return levels.Count;
    }
    
    public int LevelCount(MadLevel.Type type) {
        var query = from level in levels where level.type == type select level;
        return query.Count();
    }
    
    public Level[] GetLevelsInOrder() {
        var query = from l in levels orderby l.order ascending select l;
        return query.ToArray();
    }
    
    public Level GetLevel(int index) {
        var query = from l in levels orderby l.order ascending select l;
        
        int skipped = 0;
        foreach (var level in query) {
            if (skipped == index) {
                return level;
            } else {
                skipped++;
            }
        }
        
        Debug.LogError(string.Format("Out of bounds: {0} (size: {1})", index, skipped));
        return null;
    }
    
    public Level GetLevel(MadLevel.Type type, int index) {
        var query = from l in levels where l.type == type orderby l.order ascending select l;
    
        int skipped = 0;
        foreach (var level in query) {
            if (skipped == index) {
                return level;
            } else {
                skipped++;
            }
        }
        
        Debug.LogError(string.Format("Out of bounds: {0} (size: {1})", index, skipped));
        return null;
    }
    
    public int FindLevelIndex(MadLevel.Type type, string levelName) {
        var query = from l in levels where l.type == type orderby l.order ascending select l;
        
        int index = 0;
        foreach (var level in query) {
            if (level.name == levelName) {
                return index;
            }
            
            index++;
        }
        
        return -1;
    }
    
    public Level FindLevelByName(string levelName) {
        var query = from l in levels where l.name == levelName select l;
        var first = query.FirstOrDefault();
        return first;
    }
    
    public Level FindNextLevel(string currentLevelName) {
        var currentLevel = FindLevelByName(currentLevelName);
        MadDebug.Assert(currentLevel != null, "Cannot find level " + currentLevelName);
        
        var nextLevelQuery =
            from l in levels
            where l.order > currentLevel.order
            orderby l.order ascending
            select l;
            
        var nextLevel = nextLevelQuery.FirstOrDefault();
        return nextLevel;
    }
    
    public Level FindNextLevel(string currentLevelName, MadLevel.Type type) {
        var currentLevel = FindLevelByName(currentLevelName);
        MadDebug.Assert(currentLevel != null, "Cannot find level " + currentLevelName);
        
        var nextLevelQuery =
            from l in levels
            where l.order > currentLevel.order && l.type == type
            orderby l.order ascending
            select l;
            
        var nextLevel = nextLevelQuery.FirstOrDefault();
        return nextLevel;
    }
    
    public Level FindPreviousLevel(string currentLevelName) {
        var currentLevel = FindLevelByName(currentLevelName);
        MadDebug.Assert(currentLevel != null, "Cannot find level " + currentLevelName);
        
        var nextLevelQuery =
            from l in levels
            where l.order < currentLevel.order
            orderby l.order descending
            select l;
            
        var nextLevel = nextLevelQuery.FirstOrDefault();
        return nextLevel;
    }
    
    public Level FindPreviousLevel(string currentLevelName, MadLevel.Type type) {
        var currentLevel = FindLevelByName(currentLevelName);
        MadDebug.Assert(currentLevel != null, "Cannot find level " + currentLevelName);
        
        var nextLevelQuery =
            from l in levels
            where l.order < currentLevel.order && l.type == type
            orderby l.order descending
            select l;
            
        var nextLevel = nextLevelQuery.FirstOrDefault();
        return nextLevel;
    }
    
    public Level FindFirstForScene(string levelName) { // TODO: look for index
        var ordered =
            from l in levels
            orderby l.order ascending
            select l;
        
        foreach (var level in ordered) {
            if (level.sceneName == levelName) {
                return level;
            }
        }
        
        return null;
    }
    
    IEnumerable<Level> Order(IEnumerable<Level> levels) {
        return from l in levels orderby l.order ascending select l;
    }
    
#if UNITY_EDITOR
    public bool CheckBuildSynchronized() {
        var scenes = EditorBuildSettings.scenes;
        
        if (levels.Count == 0) {
            // do not synchronize anything if it's nothing there
            return true;
        }
        
        if (scenes.Length == 0 && levels.Count > 0 || scenes.Length > 0 && levels.Count == 0) {
//            Debug.Log("Failed size test");
            return false;
        }
        
        if (scenes.Length == 0 && levels.Count == 0) {
            return true;
        }
        
        var firstLevel = GetLevel(0);
        
        // check if first scene is my first scene
        if (scenes[0].path != firstLevel.scenePath) {
//            Debug.Log("Different start scene");
            return false;
        }
        
        // find all configuration scenes that are not in build
        foreach (var level in levels) {
            if (!level.IsValid()) {
                continue;
            }
        
            var obj = Array.Find(scenes, (scene) => scene.path == level.scenePath);
            if (obj == null) {  // scene not found in build
//                Debug.Log("Scene not found in build: " + item.level.scene);
                return false;
            }
        }
        
        return true;
    }
    
    public void SynchronizeBuild() {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        foreach (var level in Order(levels)) {
            if (!level.IsValid()) {
                continue;
            }
        
            string path = level.scenePath;
            if (scenes.Find((obj) => obj.path == path) == null) {
                var scene = new EditorBuildSettingsScene(path, true);
                scenes.Add(scene);
            }
        }
        
        EditorBuildSettings.scenes = scenes.ToArray();
    }
#endif
    
    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static MadLevelConfiguration[] FindAll() {
        List<MadLevelConfiguration> output = new List<MadLevelConfiguration>();
        var configurations = Resources.LoadAll("LevelConfig", typeof(MadLevelConfiguration));
        
        foreach (var conf in configurations) {
            output.Add(conf as MadLevelConfiguration);
        }
        
        return output.ToArray();
    }
    
    public static MadLevelConfiguration GetActive() {
        var active = FindActive();
        MadDebug.Assert(active != null, "There's no active configuration. Please make at least one!");
        return active;
    }
    
    public static MadLevelConfiguration FindActive() {
        var all = FindAll();
        var active = from conf in all where conf.active == true select conf;
        
        var configuration = active.FirstOrDefault();
        
        if (active.Count() > 1) {
            Debug.LogError("There are more than one active configuration. "
                + "This shouldn't happen! Still I will use " + configuration.name
                + " and deactivate others", configuration);
            configuration.active = true;
        }
        
        return configuration;
    }
    
    void DeactivateOthers() {
        var all = FindAll();
        foreach (var conf in all) {
            if (conf != this) {
                conf.active = false;
            }
        }
    }
    
    public bool IsValid() {
        foreach (var level in levels) {
            if (!level.IsValid()) {
                return false;
            }
            
            if (level.sceneObject == null) {
                return false;
            }
        }
        
        return true;
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    [System.Serializable]
    public class Level {
    
        // fields
        [SerializeField] internal MadLevelConfiguration parent;
        public int order;
        
        public string name = "New Level";
        public MadLevel.Type type;
        public string arguments = "";
        
        [SerializeField] UnityEngine.Object _sceneObject;
        [SerializeField] string _scenePath;
        [SerializeField] string _sceneName;
        
        // deprecated fields
        [SerializeField] string scene = "";
        
        // properties
        public UnityEngine.Object sceneObject {
            get {
                return _sceneObject;
            }
            set {
                if (!Application.isEditor) {
                    Debug.LogError("This method has no effect when calling from play mode");
                }
            
#if UNITY_EDITOR
                _sceneObject = value;
                if (value != null) {
                    _scenePath = AssetDatabase.GetAssetPath(value);
                    string basename = scenePath.Substring(_scenePath.LastIndexOf('/') + 1);
                    _sceneName = basename.Substring(0, basename.IndexOf('.'));
                } else {
                    _sceneName = "";
                    _scenePath = "";
                }
#endif
            }
        }
        
        public string scenePath {
            get {
                return _scenePath;
            }
        }
        
        public string sceneName {
            get {
                return _sceneName;
            }
        }
        
        internal Level(MadLevelConfiguration parent) {
            this.parent = parent;
        }
        
        public void Upgrade() {
            // moves scene paths to into scenes
            if (!string.IsNullOrEmpty(scene)) {
#if UNITY_EDITOR
                    var obj = AssetDatabase.LoadAssetAtPath("Assets/" + scene, typeof(UnityEngine.Object));
                if (obj != null) {
                    sceneObject = obj;
                    scene = "";
                }
#endif
            }
        }
        
        public bool IsValid() {
            return sceneObject != null && !string.IsNullOrEmpty(name) && !HasDuplicatedName();
        }
        
        public bool HasDuplicatedName() {
            foreach (var otherLevel in parent.levels) {
                if (otherLevel == this) {
                    continue;
                }
                
                if (otherLevel.name == name) {
                    return true;
                }
            }
            
            return false;
        }
        
        public override int GetHashCode () {
            int hash = 17;
            hash = hash * 31 + order.GetHashCode();
            hash = hash * 31 + (sceneObject != null ? sceneObject.GetHashCode() : 0);
            hash = hash * 31 + name.GetHashCode();
            hash = hash * 31 + type.GetHashCode();
            hash = hash * 31 + arguments.GetHashCode();
            
            return hash;
        }
    }
    
    public delegate void Callback0();

}

#if !UNITY_3_5
} // namespace
#endif