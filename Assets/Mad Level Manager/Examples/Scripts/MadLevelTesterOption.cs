/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

// this is example code of how use Mad Level Manager code in your game
public class MadLevelTesterOption : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public int points; // number of points that this option gives
    
    bool completed = false;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void Start() {
        var sprite = GetComponent<MadSprite>();

        // connect to sprite onMouseDown && onTap event
        sprite.onMouseUp += sprite.onTap = (s) => {
        
            // star_1, star_2 and start_3 properties are names or game objects
            // that are nested under every icon in the level select screen
            
            // note that if you've gained 3 stars already there's no possibility to lost them
        
            // first star gained when points number is at least 100
            if (points >= 100) {
                EarnStar("star_1");
                MarkLevelCompleted();
            }
            
            // second star gained when points number is at least 150
            if (points >= 150) {
                EarnStar("star_2");
                MarkLevelCompleted();
            }
            
            // third star gained when points number is at least 200
            if (points >= 200) {
                EarnStar("star_3");
                MarkLevelCompleted();
            }
            
            // play animation
            var controller = GameObject.FindGameObjectWithTag("GameController");
            var script = controller.GetComponent<MadLevelTesterController>();
            script.PlayFinishAnimation(sprite, completed);
            
            StartCoroutine(WaitForAnimation());
        };
    }
    
    void EarnStar(string name) {
        // set level boolean sets level property of type boolean
        MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, name, true);
    }
    
    void MarkLevelCompleted() {
        // sets the level completed
        // by default next level will be unlocked
        MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
        
        // manual unlocking looks like this:
        // MadLevelProfile.SetLocked("level_name", false);
        
        // remembering that level is completed
        completed = true;
    }
    
    IEnumerator WaitForAnimation() {
        yield return new WaitForSeconds(2.2f); // animation lasts 2 second
        
        if (completed) {
            // if level is completed go to next level or to 'Level Select'
            // depends of it this the last level
            if (MadLevel.HasNext(MadLevel.Type.Level)) {
                MadLevel.LoadNext(MadLevel.Type.Level);
            } else {
                MadLevel.LoadLevelByName("Level Select");
            }
        } else {
            // if not completed go back to the menu
            MadLevel.LoadLevelByName("Level Select");
        }
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}