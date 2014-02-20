/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

public class MadLevelKeyboardControl : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void Start() {
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ActivateNextLevel();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            ActivatePreviousLevel();
        }
    }
    
    void ActivateNextLevel() {
        var layout = MadLevelLayout.current;
        var activeIcon = layout.GetActiveIcon();
        if (activeIcon == null) {
            var firstIcon = layout.GetFirstIcon();
            Activate(firstIcon, layout);
        } else {
            var nextIcon = layout.GetNextIcon(activeIcon);
            if (nextIcon != null) {
                Activate(nextIcon, layout);
            }
        }
    }
    
    void ActivatePreviousLevel() {
        var layout = MadLevelLayout.current;
        var activeIcon = layout.GetActiveIcon();
        if (activeIcon != null) {
            var nextIcon = layout.GetPreviousIcon(activeIcon);
            if (nextIcon != null) {
                Activate(nextIcon, layout);
            }
        }
    }
    
    void Activate(MadLevelIcon icon, MadLevelAbstractLayout layout) {
        layout.Activate(icon);
        if (layout is MadLevelFreeLayout) {
            var free = layout as MadLevelFreeLayout;
            free.LookAtIcon(icon, MadiTween.EaseType.easeOutCubic, 1);
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}