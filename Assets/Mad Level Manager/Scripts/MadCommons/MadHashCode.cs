/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MadLevelManager {

public class MadHashCode {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    int currentHash;
    int secondPrime = 13;
    
    // ===========================================================
    // Constructors
    // ===========================================================
    
    public MadHashCode() {
        currentHash = 37;
    }

    // ===========================================================
    // Methods
    // ===========================================================

    public void Add(System.Object obj) {
        currentHash += currentHash * secondPrime + (obj != null ? obj.GetHashCode() : 0);
    }
    
    public void AddEnumerable(IEnumerable enumerable) {
        if (enumerable == null) {
            Add(null);
            return;
        }
        
        foreach (var obj in enumerable) {
            Add(obj);
        }
    }
    
    public override int GetHashCode() {
        return currentHash;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace