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

public class MadLevelInitTool : MadInitTool {

    // ===========================================================
    // Constants
    // ===========================================================
    
    const string IconPrefab = "Assets/Mad Level Manager/Examples/Prefabs/icon2Prefab.prefab";
    const string SlideLeftPrefab = "Assets/Mad Level Manager/Examples/Prefabs/slideLeft2Prefab.prefab";
    const string SlideRightPrefab = "Assets/Mad Level Manager/Examples/Prefabs/slideRight2Prefab.prefab";

    // ===========================================================
    // Fields
    // ===========================================================
    
    Layout layout;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    protected override void OnFormGUI() {
        layout = (Layout) EditorGUILayout.EnumPopup("Screen Layout", layout);
    }
    
    protected override void AfterCreate(MadRootNode root) {
        root.gameObject.AddComponent<MadLevelRoot>();

        MadLevelIcon icon;
        MadSprite slideLeft, slideRight;

        InitTemplates(root, out icon, out slideLeft, out slideRight);
                        
        switch (layout) {
            case Layout.Grid:
                CreateGrid(root, icon, slideLeft, slideRight);
                break;
            case Layout.Free:
                CreateFree (root, icon);
                break;
            default:
                Debug.LogError("Unknown layout: " + layout);
                break;
        }
        
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    void CreateGrid(MadRootNode root, MadLevelIcon icon, MadSprite slideLeft, MadSprite slideRight) {
        var panel = MadTransform.FindChild<MadPanel>(root.transform);
        var gridLayout = MadLevelGridTool.CreateUnderPanel(panel);
        
        gridLayout.iconTemplate = icon;
        gridLayout.leftSlideSprite = slideLeft;
        gridLayout.rightSlideSprite = slideRight;
        gridLayout.dirty = true;
    }
    
    void CreateFree(MadRootNode root, MadLevelIcon icon) {
        var panel = MadTransform.FindChild<MadPanel>(root.transform);
        var freeLayout = MadLevelFreeTool.CreateUnderPanel(panel);
        
        freeLayout.iconTemplate = icon;
        freeLayout.dirty = true;
    }
    
    void InitTemplates(MadRootNode root, out MadLevelIcon icon, out MadSprite slideLeftSprite,
            out MadSprite slideRightSprite) {
        var panel = MadTransform.FindChild<MadPanel>(root.transform);
        var templates = MadTransform.CreateChild(panel.transform, "Templates");
        
        GameObject iconPrefab = (GameObject) AssetDatabase.LoadAssetAtPath(IconPrefab, typeof(GameObject));
        GameObject slideLeftPrefab = (GameObject) AssetDatabase.LoadAssetAtPath(SlideLeftPrefab, typeof(GameObject));
        GameObject slideRightPrefab = (GameObject) AssetDatabase.LoadAssetAtPath(SlideRightPrefab, typeof(GameObject));
        
        if (MadGameObject.AnyNull(iconPrefab, slideLeftPrefab, slideRightPrefab)) {
            Debug.LogWarning("I cannot find all needed prefabs to create example templates. Have you moved Mad Level "
            + "Manager directory to other than default place?");
        }
        
        if (iconPrefab != null) {
            var obj = MadTransform.CreateChild(templates.transform, "icon", iconPrefab);
            icon = obj.GetComponent<MadLevelIcon>();
        } else {
            icon = null;
        }
        
        if (slideLeftPrefab != null) {
            var slide = MadTransform.CreateChild(templates.transform, "slide left", slideLeftPrefab);
            slideLeftSprite = slide.GetComponent<MadSprite>();
        } else {
            slideLeftSprite = null;
        }
        
        if (slideRightPrefab != null) {
            var slide = MadTransform.CreateChild(templates.transform, "slide right", slideRightPrefab);
            slideRightSprite = slide.GetComponent<MadSprite>();
        } else {
            slideRightSprite = null;
        }
        
        MadGameObject.SetActive(templates, false);
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum Layout {
        Grid,
        Free,
    }

}

#if !UNITY_3_5
} // namespace
#endif