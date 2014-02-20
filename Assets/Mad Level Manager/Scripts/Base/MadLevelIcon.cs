/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

#pragma warning disable 0618

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
[ExecuteInEditMode]   
public class MadLevelIcon : MadSprite {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    // if true then levelSceneName and levelArguments are loaded from configuration
    public bool hasLevelConfiguration;
    
    public MadLevelConfiguration configuration;
    
    // level index in group
    public int levelIndex;
    
    public string levelSceneName;
    public string levelArguments;
    
    public MadLevelProperty completedProperty;
    public MadLevelProperty lockedProperty;
    public MadText levelNumber;
    
    // list of level icons to unlock on completion of this one
    public List<MadLevelIcon> unlockOnComplete;
    
    [HideInInspector]
    public int version = 0;
    
    // ===========================================================
    // Properties
    // ===========================================================
    
    public bool completed {
        set {
            if (completedProperty != null) {
                completedProperty.propertyEnabled = value;
            } else if (value) {
                // normally unlock on complete will be invoked by property change
                UnlockOnComplete();
            }
        }
    }
    
    public bool locked {
        get {
            if (lockedProperty != null) {
                return lockedProperty.propertyEnabled;
            } else {
                Debug.LogWarning("Locked property not set", this);
                return false;
            }
        }
        
        set {
            if (lockedProperty != null) {
                lockedProperty.propertyEnabled = value;
            }
            
            if (!value) {
                if (levelNumber != null) {
                    var property = levelNumber.GetComponent<MadLevelProperty>();
                    property.propertyEnabled = true;
                }
            }
        }
    }
    
    /// <summary>
    /// Level refrerence. You can get here all information about the referenced level.
    /// </summary>
    /// <value>The level.</value>
    public MadLevelConfiguration.Level level {
        get {
            return configuration.GetLevel(MadLevel.Type.Level, levelIndex);
        }
    }    
                    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    protected override void OnEnable() {
        base.OnEnable();
        Upgrade();
    }
    
    void Upgrade() {
        if (version == 0) {
            // in free layout of 1.3.x icon names were mistaken for level names
            // check if there's profile entry for level name of icon name
            // but without level name itself
            if (MadLevelProfile.IsLevelSet(name) && !MadLevelProfile.IsLevelSet(level.name)) {
                MadLevelProfile.RenameLevel(name, level.name);
            }
        }
        
        version = 1;
    }
    
    protected override void Start() {
        base.Start();
        
        // completed property object is optional
        // if it's not present, check the completed property manually
        if (completedProperty == null) {
            completed = MadLevelProfile.IsCompleted(level.name);
        }
        
        onMouseUp += (sprite) => Activate();
        onTap += (sprite) => Activate();
    }
    
    /// <summary>
    /// Activates this icon.
    /// </summary>
    public void Activate() {
        var layout = MadTransform.FindParent<MadLevelAbstractLayout>(transform);
        layout.Activate(this);
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    public MadLevelProperty.SpecialType TypeFor(MadLevelProperty property) {
        if (property == completedProperty) {
            return MadLevelProperty.SpecialType.Completed;
        }
        
        if (property == lockedProperty) {
            return MadLevelProperty.SpecialType.Locked;
        }
        
        if (property == levelNumber) {
            return MadLevelProperty.SpecialType.LevelNumber;
        }
        
        return MadLevelProperty.SpecialType.Regular;
    }

    public void UpdateProperty(string propertyName, bool state) {
        var properties = GetComponentsInChildren<MadLevelProperty>();
        bool found = false;
        
        foreach (var property in properties) {
            if (property.name == propertyName) {
                property.propertyEnabled = state;
                found = true;
            }
        }
        
        if (!found) {
            Debug.LogError("Cannot find property '" + propertyName + "'", gameObject);
        }
    }
    
    public void LoadLevel() {
        if (hasLevelConfiguration) {
            var level = configuration.GetLevel(MadLevel.Type.Level, levelIndex);
            MadLevel.LoadLevelByName(level.name);
        } else {
            if (!string.IsNullOrEmpty(levelSceneName)) {
                MadLevelProfile.recentLevelSelected = level.name;
            
                MadLevel.currentLevelName = level.name;
                MadLevel.arguments = "";
                Application.LoadLevel(levelSceneName);
            } else {
                Debug.LogError("Level scene name not set. I don't know what to load!");
                return;
            }
        }
        
    }
    
    void OnPropertyChange(MadLevelProperty property) {
        if (property.specialType == MadLevelProperty.SpecialType.Completed) {
            UnlockOnComplete();
        }
    }
    
    void UnlockOnComplete() {
        if (unlockOnComplete != null) {
            foreach (var icon in unlockOnComplete) {
                icon.locked = false;
            }
        }
    }
        
    // ===========================================================
    // Message methods
    // ===========================================================
    
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