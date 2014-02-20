/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[ExecuteInEditMode]
public class MadLevelProperty : MadNode {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    // is this property active? (enabled)
    [HideInInspector]
    public bool _propertyEnabled = true;
    
    public MadSprite[] showWhenEnabled;
    public MadSprite[] showWhenDisabled;
    
    public bool textFromProperty;
    public string textPropertyName;
    
    public SpecialType specialType {
        get {
            return icon.TypeFor(this);
        }
    }
    
    private MadSprite _sprite;
    private MadSprite sprite {
        get {
            if (_sprite == null) {
                _sprite = GetComponent<MadSprite>();
            }
            
            return _sprite;
        }
    }
    
    private MadLevelIcon _icon;
    private MadLevelIcon icon {
        get {
            if (_icon == null) {
                _icon = MadTransform.FindParent<MadLevelIcon>(transform);
            }
            
            return _icon;
        }
    }
    
    // ===========================================================
    // Properties
    // ===========================================================
    
    public bool propertyEnabled {
        get {
            return _propertyEnabled;
        }
        
        set {
            UpdateEnabled(value);
        }
    }
    
    public bool propertySet {
        get {
            if (Application.isPlaying) {
                return IsLevelBooleanSet();
            } else {
                return false;
            }
        }
    }
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnEnable() {
        if (Application.isPlaying) {
            if (propertySet) {
                propertyEnabled = GetLevelBoolean();
            } else {
                // save the property if unset
                SetLevelBoolean(propertyEnabled);
            }
        }
    }
    
    void Start() {
        if (textFromProperty && sprite is MadText) {
            var text = sprite as MadText;
            var level = icon.level;
            var str = MadLevelProfile.GetLevelAny(level.name, textPropertyName);
            text.text = str;
        }
    }

    void Update() {
        // cannot do the update every frame because of huge performance loss
//        if (propertySet) {
//            propertyEnabled = GetLevelBoolean();
//        }
    }
    
    void UpdateEnabled(bool enabled) {
        // do nothing is there's no change
        if (_propertyEnabled == enabled) {
            return;
        }
    
        MadSprite[] hideSprites, showSprites;
        if (enabled) {
            showSprites = showWhenEnabled;
            hideSprites = showWhenDisabled;
        } else {
            showSprites = showWhenDisabled;
            hideSprites = showWhenEnabled;
        }
        
        if (hideSprites != null) {
            foreach (var sprite in hideSprites) {
                // if this is property then change property value
                var property = sprite.GetComponent<MadLevelProperty>();
                if (property != null) {
                    property.propertyEnabled = false;
                } else {
                    sprite.visible = false;
                }
            }
        }
        
        if (showSprites != null) {
            foreach (var sprite in showSprites) {
                // if this is property then change property value
                var property = sprite.GetComponent<MadLevelProperty>();
                if (property != null) {
                    property.propertyEnabled = true;
                } else {
                    sprite.visible = true;
                }
            }
        }
        
        if (this.sprite != null) { // why the hell must be here 'this.'?
    
            if (!Application.isPlaying) {
                // in editor just change the visibility
                sprite.visible = enabled;
            } else {
                sprite.visible = enabled;
            }
        }
        
        _propertyEnabled = enabled;
        
        if (Application.isPlaying) {
            SetLevelBoolean(enabled);
            SendMessageUpwards("OnPropertyChange", this);
        }
    }
    
    bool GetLevelBoolean() {
        string levelName = icon.level.name;
        switch (specialType) {
            case SpecialType.Regular:
                return MadLevelProfile.GetLevelBoolean(levelName, name);
            case SpecialType.LevelNumber:
                return MadLevelProfile.GetLevelBoolean(levelName, name);
            case SpecialType.Locked:
                return MadLevelProfile.IsLocked(levelName);
            case SpecialType.Completed:
                return MadLevelProfile.IsCompleted(levelName);
            default:
                MadDebug.Assert(false, "Unknown special type: " + specialType);
                return false;
        }
    }
    
    void SetLevelBoolean(bool val) {
        string levelName = icon.level.name;
            switch (specialType) {
            case SpecialType.Regular:
                MadLevelProfile.SetLevelBoolean(levelName, name, val);
                break;
            case SpecialType.LevelNumber:
                MadLevelProfile.SetLevelBoolean(levelName, name, val);
                break;
            case SpecialType.Locked:
                MadLevelProfile.SetLocked(levelName, val);
                break;
            case SpecialType.Completed:
                MadLevelProfile.SetCompleted(levelName, val);
                break;
            default:
                MadDebug.Assert(false, "Unknown special type: " + specialType);
                break;
        }
    }
    
    bool IsLevelBooleanSet() {
        string levelName = icon.level.name;
            switch (specialType) {
            case SpecialType.Regular:
                return MadLevelProfile.IsLevelPropertySet(levelName, name);
            case SpecialType.LevelNumber:
                return MadLevelProfile.IsLevelPropertySet(levelName, name);
            case SpecialType.Locked:
                return MadLevelProfile.IsLockedSet(levelName);
            case SpecialType.Completed:
                return MadLevelProfile.IsCompletedSet(levelName);
                default:
                    MadDebug.Assert(false, "Unknown special type: " + specialType);
                return false;
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum Type {
        Bool,
        Integer,
        Float,
        String,
    }
    
    public enum SpecialType {
        Regular,
        Locked,
        Completed,
        LevelNumber,
    }
}

#if !UNITY_3_5
} // namespace
#endif