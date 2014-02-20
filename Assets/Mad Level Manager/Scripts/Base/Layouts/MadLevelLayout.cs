/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

/// <summary>
/// Helper class to find current level layout
/// </summary>
public class MadLevelLayout {

    // ===========================================================
    // Properties
    // ===========================================================
    
    /// <summary>
    /// Gets the current level layout that is present on the scene. Optimization note:
    /// Assign this property to variable if you want to refer to it multiple times. Calling in repeately will
    /// reduce the performance significantly.
    /// </summary>
    /// <value>Current level layout or <code>null<code> is no layout is present on the current scene.</value>
    public static MadLevelAbstractLayout current {
        get {
            var layouts = Component.FindObjectsOfType(typeof(MadLevelAbstractLayout));
            if (layouts.Length == 0) {
                Debug.LogError("There's no level layout on the current scene");
                return null;
            }
            
            if (layouts.Length > 1) {
                Debug.LogError("There's more than one level layout on the current scene.");
            }
            
            return layouts[0] as MadLevelAbstractLayout;
        }
    }

}

#if !UNITY_3_5
} // namespace
#endif