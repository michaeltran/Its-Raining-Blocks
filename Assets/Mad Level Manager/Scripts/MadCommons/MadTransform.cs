/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MadLevelManager {

public class MadTransform {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static T CreateChild<T>(Transform parent, string name) where T : Component {
        GameObject go = null;
    
        go = new GameObject(name);
        go.transform.parent = parent;
        
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        
        T component = go.GetComponent<T>();
        if (component == null) {
            component = go.AddComponent<T>();
        }
        MadUndo.RegisterCreatedObjectUndo(go, "Created " + name);
        
        return component;
    }
    
    public static T GetOrCreateChild<T>(Transform parent, string name) where T : Component {
        T child = FindChild<T>(parent, (T t) => t.name == name, 0);
        if (child != null) return child;
        return CreateChild<T>(parent, name);
//        return child ?? CreateChild<T>(parent, name);
    }
    
    public static T CreateChild<T>(Transform parent, string name, T template) where T : Component {
        var gameObject = CreateChild(parent, name, template.gameObject);
        return gameObject.GetComponent<T>();
    }
    
    public static T GetOrCreateChild<T>(Transform parent, string name, T template) where T : Component {
        T child = FindChild<T>(parent, (T t) => t.name == name, 0);
        if (child != null) return child;
        return CreateChild<T>(parent, name, template);
//        return child ?? CreateChild<T>(parent, name);
    }
    
    public static GameObject CreateChild(Transform parent, string name) {
        GameObject go = new GameObject();
        go.transform.parent = parent;
        
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        
        go.name = name;
        
        MadUndo.RegisterCreatedObjectUndo(go, "Created " + name);
        
        return go;
    }
    
    public static GameObject CreateChild(Transform parent, string name, GameObject template) {
        GameObject go = null;
        go = GameObject.Instantiate(template) as GameObject;
        go.transform.parent = parent;
        go.name = name;
        
        MadUndo.RegisterCreatedObjectUndo(go, "Created " + name);
        
        return go;
    }
    
    public static T FindChild<T>(Transform parent) where T : Component {
        return FindChild(parent, (T t) => true);
    }
    
    public static T FindChild<T>(Transform parent, Predicate<T> predicate) where T : Component {
        return FindChild<T>(parent, predicate, int.MaxValue, 0);
    }
    
    public static T FindChild<T>(Transform parent, Predicate<T> predicate, int depth) where T : Component {
        return FindChild<T>(parent, predicate, depth, 0);
    }
    
    private static T FindChild<T>(Transform parent, Predicate<T> predicate, int depth, int currDepth)
        where T : Component {
        int count = parent.childCount;
        for (int i = 0; i < count; ++i) {
            var child = parent.GetChild(i);
            T component = child.GetComponent<T>();
            if (component != null && predicate(component)) {
                return component;
            }
            
            if (currDepth < depth) {
                var c = FindChild<T>(child, predicate, depth, currDepth + 1);
                if (c != null) {
                    return c;
                }
            }
        }
        
        return null;
    }
    
    public static T FindChildWithName<T>(Transform parent, string name) where T : Component {
        return FindChild<T>(parent, (t) => t.name == name);
    }
    
    public static List<T> FindChildren<T>(Transform parent) where T : Component {
        return FindChildren(parent, (T t) => true);
    }
    
    public static List<T> FindChildren<T>(Transform parent, Predicate<T> predicate) where T : Component {
        return FindChildren<T>(parent, predicate, int.MaxValue);
    }
    
    public static List<T> FindChildren<T>(Transform parent, Predicate<T> predicate, int depth) where T : Component {
        return FindChildren<T>(parent, predicate, depth, 0);
    }
    
    static List<T> FindChildren<T>(Transform parent, Predicate<T> predicate, int depth, int currDepth) where T : Component {
        List<T> output = new List<T>();
        
        int count = parent.childCount;
        for (int i = 0; i < count; ++i) {
            var child = parent.GetChild(i);
            T component = child.GetComponent<T>();
            if (component != null && predicate(component)) {
                output.Add(component);
            }
            
            if (currDepth < depth) {
                output.AddRange(FindChildren<T>(child, predicate, depth, currDepth + 1));
            }
        }
        
        return output;
    }
    
    public static T FindParent<T>(Transform t) where T : Component {
        return FindParent<T>(t, int.MaxValue);
    }
    
    public static T FindParent<T>(Transform t, int depth) where T : Component {
        return FindParent<T>(t, depth, (c) => true);
    }
    
    public static T FindParent<T>(Transform t, Predicate<T> predicate) where T : Component {
        return FindParent<T>(t, int.MaxValue, predicate);
    }
    
    public static T FindParent<T>(Transform t, int depth, Predicate<T> predicate) where T : Component {
        var c = t.parent;
        int currDepth = 0;
        while (c != null && currDepth <= depth) {
            var comp = c.GetComponent<T>();
            if (comp != null && predicate(comp)) {
                return comp;
            }
            
            c = c.parent;
            currDepth++;
        }
        
        return null;
    }
    
    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public delegate bool Predicate<T>(T t);

}

} // namespace