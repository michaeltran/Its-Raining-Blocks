/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MadLevelManager {

public class MadMath  {

    // ===========================================================
    // Constants
    // ===========================================================
    
    public static readonly Vector3 InfinityVector3 =
        new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

    // ===========================================================
    // Fields
    // ===========================================================

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static Vector2 SmoothDampVector2(Vector2 current, Vector2 target,
            ref Vector2 currentVelocity, float time) {
        float velX = currentVelocity.x;
        float velY = currentVelocity.y;
        
        float x = Mathf.SmoothDamp(current.x, target.x, ref velX, time);
        float y = Mathf.SmoothDamp(current.y, target.y, ref velY, time);
        
        currentVelocity.x = velX;
        currentVelocity.y = velY;
        
        return new Vector2(x, y);
    }
    
    public static Vector2 ClosestPoint(Rect rect, Vector2 point) {
        if (rect.Contains(point)) {
            return point;
        } else {
            float x = point.x;
            float y = point.y;
            
            if (x < rect.xMin) {
                x = rect.xMin;
            } else if (x > rect.xMax) {
                x = rect.xMax;
            }
            
            if (y < rect.yMin) {
                y = rect.yMin;
            } else if (y > rect.yMax) {
                y = rect.yMax;
            }
            
            return new Vector2(x, y);
        }
    }
    
    public static Vector3 Round(Vector3 v) {
        return new Vector3(
            Mathf.Round(v.x),
            Mathf.Round(v.y),
            Mathf.Round(v.z)
        );
    }
    
    public static Rect Expand(Rect a, Rect b) {
        return new Rect(
            Mathf.Min(a.x, b.y),
            Mathf.Min(a.y, b.y),
            Mathf.Max(a.xMax, b.xMax) - Mathf.Min(a.xMin, b.xMin),
            Mathf.Max(a.yMax, b.yMax) - Mathf.Min(a.yMin, b.yMin)
            );
    }
    
    public static Rect Translate(Rect r, Vector2 delta) {
        return new Rect(r.x + delta.x, r.y + delta.y, r.width, r.height);
    }
    
    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace