/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace MadLevelManager {
 
// this class holds most of menu entries   
public class MadLevelMenu : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================
    
    private const string HomePage = 
        "http://redmine.madpixelmachine.com/projects/mad-level-manager/wiki";

    // ===========================================================
    // Fields
    // ===========================================================
 
    // ===========================================================
    // Static Methods
    // ===========================================================
    
    [MenuItem("Tools/Mad Level Manager/Initialize", false, 100)]
    static void InitTool() {
        EditorWindow.GetWindow<MadLevelInitTool>(false, "Init Tool", true);
    }
    
    [MenuItem ("Tools/Mad Level Manager/Create Font", false, 120)]
 static void CreateFont() {
        MadFontBuilder.CreateFont();
    }
    
    [MenuItem ("Tools/Mad Level Manager/Create UI/Sprite", false, 140)]
    [MenuItem ("GameObject/Create Other/Mad Level Manager/UI Sprite", false, 10000)]
    static void CreateSprite() {
        var sprite = MadTransform.CreateChild<MadSprite>(ActiveParentOrPanel(), "sprite");
        Selection.activeGameObject = sprite.gameObject;
    }
    
    [MenuItem ("Tools/Mad Level Manager/Create UI/Text", false, 141)]
    [MenuItem ("GameObject/Create Other/Mad Level Manager/UI Text", false, 10001)]
    static void CreateText() {
        var text = MadTransform.CreateChild<MadText>(ActiveParentOrPanel(), "text");
        Selection.activeGameObject = text.gameObject;
    }
    
    [MenuItem ("Tools/Mad Level Manager/Create UI/Anchor", false, 142)]
    [MenuItem ("GameObject/Create Other/Mad Level Manager/UI Anchor", false, 10002)]
    static void CreateAnchor() {
        var anchor = MadTransform.CreateChild<MadAnchor>(ActiveParentOrPanel(), "Anchor");
        Selection.activeGameObject = anchor.gameObject;
    }
    
    [MenuItem("Tools/Mad Level Manager/Create UI/Create Background", false, 149)]
    static void CreateBackground() {
        MadLevelBackgroundTool.ShowWindow();
    }
    
    static Transform ActiveParentOrPanel() {
        Transform parentTransform = null;
        
        var transforms = Selection.transforms;
        if (transforms.Length > 0) {
            var firstTransform = transforms[0];
            if (MadTransform.FindParent<MadPanel>(firstTransform) != null) {
                parentTransform = firstTransform;
            }
        }
        
        if (parentTransform == null) {
            var panel = MadPanel.UniqueOrNull();
            if (panel != null) {
                parentTransform = panel.transform;
            }
        }
        
        return parentTransform;
    }
    
    [MenuItem("Tools/Mad Level Manager/Create Level Icon", false, 150)]      
    static void CreateIcon() {
        var wizard = ScriptableWizard.DisplayWizard<MadLevelIconTool>("Create Icon", "Create");
        wizard.panel = MadPanel.UniqueOrNull();
    }
    
    [MenuItem("Tools/Mad Level Manager/Create Level Property/Empty", false, 155)]
    static void CreateEmptyProperty() {
        ScriptableWizard.DisplayWizard<MadLevelPropertyEmptyTool>("Create Empty Property", "Create");
    }
    
    [MenuItem("Tools/Mad Level Manager/Create Level Property/Sprite", false, 160)]
    static void CreateSpriteProperty() {
        ScriptableWizard.DisplayWizard<MadLevelPropertySpriteTool>("Create Sprite Property", "Create");
    }
    
    [MenuItem("Tools/Mad Level Manager/Create Level Property/Text", false, 165)]
    static void CreateTextProperty() {
        ScriptableWizard.DisplayWizard<MadLevelPropertyTextTool>("Create Text Property", "Create");
    }
    
    [MenuItem("Tools/Mad Level Manager/Create Grid Layout", false, 200)]
    static void CreateGridLayout() {
        var panel = MadPanel.UniqueOrNull();
        if (panel != null) {
            MadLevelGridTool.CreateUnderPanel(panel);
        } else {
            ScriptableWizard.DisplayWizard<MadLevelGridTool>("Create Grid Layout", "Create");
        }
    }
    
    [MenuItem("Tools/Mad Level Manager/Create Free Layout", false, 200)]
    static void CreateFreeLayout() {
        var panel = MadPanel.UniqueOrNull();
        if (panel != null) {
            MadLevelFreeTool.CreateUnderPanel(panel);
        } else {
            ScriptableWizard.DisplayWizard<MadLevelFreeTool>("Create Free Layout", "Create");
        }
    }
    
    [MenuItem("Tools/Mad Level Manager/Profile Tool", false, 900)]
    static void ProfileTool() {
        EditorWindow.GetWindow<MadLevelProfileTool>(false, "Profile Tool", true);
    }
    
    [MenuItem("Tools/Mad Level Manager/Select Active Configuration", false, 901)]
    static void SelectActiveConfiguration() {
        var active = MadLevelConfiguration.FindActive();
        if (active == null) {
            EditorUtility.DisplayDialog("Not Found", "No active configuration found.", "OK");
        } else {
//            EditorGUIUtility.PingObject(active);
            Selection.activeObject = active;
        }
    }
    
    [MenuItem("Tools/Mad Level Manager/Wiki, Manual", false, 1000)]
    static void OpenHomepage() {
        Application.OpenURL(HomePage);
    }
    
    //
    // validators
    //
    
    [MenuItem ("Tools/Mad Level Manager/Create Sprite", true)]
    [MenuItem ("Tools/Mad Level Manager/Create Level Icon", true)]
    [MenuItem ("Tools/Mad Level Manager/Create Property/Empty", true)]
    [MenuItem ("Tools/Mad Level Manager/Create Property/Sprite", true)]
    [MenuItem ("Tools/Mad Level Manager/Create Property/Text", true)]
    [MenuItem ("Tools/Mad Level Manager/Create Grid Layout", true)]
    [MenuItem ("Tools/Mad Level Manager/Create Free Layout", true)]
    static bool ValidateHasPanel() {
        return MadPanel.UniqueOrNull() != null;
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

}