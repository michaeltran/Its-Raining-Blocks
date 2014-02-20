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

public class MadInitTool : EditorWindow {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    string rootObjectName = "Mad Level Root";
    int layer = 0;
    
    protected MadRootNode root;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    protected virtual void OnFormGUI() {}
    protected virtual void AfterCreate(MadRootNode root) {}

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnGUI() {
        MadGUI.Info("This tool initialized your scene for GUI drawing. Please choose root object name and layer "
            + "on which GUI will be painted.");
    
        rootObjectName = EditorGUILayout.TextField("Root Name", rootObjectName);
        layer = EditorGUILayout.LayerField("Layer", layer);
        
        OnFormGUI();
        
        if (GUILayout.Button("Create")) {
            var panel = MadPanel.UniqueOrNull();
            bool doInit = true;
            if (panel != null) {
                doInit = EditorUtility.DisplayDialog("Scene initialized", "Scene looks like it is already initialized. "
                    + "Are you sure that you want to continue?", "Yes", "No");
            }
        
            if (doInit) {
                Init(rootObjectName, layer);
            }
        }
    }
    
    MadRootNode Init(string rootObjectName, int layer) {
        var go = new GameObject();
        go.name = rootObjectName;
        var root = go.AddComponent<MadRootNode>();
        
        bool hasOtherCamera = GameObject.FindObjectOfType(typeof(Camera)) != null;
        
        var camera = MadTransform.CreateChild<MadNode>(go.transform, "Camera 2D");
        var cam = camera.gameObject.AddComponent<Camera>();
        cam.backgroundColor = Color.gray;
        cam.orthographic = true;
        cam.orthographicSize = 1;
        cam.nearClipPlane = -2;
        cam.farClipPlane = 2;
        cam.transform.localScale = new Vector3(1, 1, 0.01f);
        
        if (hasOtherCamera) {
            cam.clearFlags = CameraClearFlags.Depth;
        }
        
        var panel = camera.CreateChild<MadPanel>("Panel");
        
        // setup layers
        cam.cullingMask = 1 << layer;
        panel.gameObject.layer = layer;
        
        AfterCreate(root);
        
        return root;
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static void ShowWindow() {
        EditorWindow.GetWindow<MadInitTool>(false, "Init Tool", true);
    }
    
    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif