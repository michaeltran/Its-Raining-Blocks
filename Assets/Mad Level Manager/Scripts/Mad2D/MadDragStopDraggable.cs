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

public class MadDragStopDraggable : MadDraggable {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public DragStopCallback dragStopCallback;
    List<Vector2> dragStops = new List<Vector2>();
    
    int dragStopCurrentIndex = 0;
    int forcedDragStopIndex = -1;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    protected override void Update() {
        base.Update();
        
        if (dragStops.Count == 0) {
            return;
        }
        
        if (!IsTouchingSingle()) {
            dragging = false;
            Clear();
            ReturnToDragStop();
            cameraPos = cachedCamPos;
        } else {
            forcedDragStopIndex = -1;
            int closest = ClosestDragStopIndex();
            Vector2 lastCamPos = cachedCamPos;
            var touchPos = TouchPosition();
            
            if (IsTouchingJustStarted()) {
                lastPosition = touchPos;
            } else {
                cachedCamPos -= touchPos - lastPosition;
                lastPosition = touchPos;
            }
            
            int closestNeighbor = ClosestNeighbor(closest);
            
            if (closestNeighbor != -1) {
                // dot product to AB vector to stay on the line
                Vector2 ab = dragStops[closestNeighbor] - dragStops[closest];
                Vector2 ac = cachedCamPos - dragStops[closest];
                
                float projection = Vector2.Dot(ac, ab.normalized);
                Vector2 position = ab.normalized * projection;
                cachedCamPos = dragStops[closest] + position;
            } else {
                // out of drag area, return NOW!
                cachedCamPos = dragStops[closest];
            }
            
            RegisterDelta(cachedCamPos - lastCamPos);
            
            if (dragDistance > deadDistance) {
                dragging = true;
                cameraPos = cachedCamPos;
            }
        }
    }

    public int AddDragStop(float x, float y) {
        dragStops.Add(new Vector2(x, y));
        return dragStops.Count - 1;
    }
    
    public void MoveTo(int dragStop) {
        forcedDragStopIndex = dragStop;
    }
    
    void ReturnToDragStop() {
        int index = ClosestDragStopIndex();
        if (index != dragStopCurrentIndex) {
            dragStopCurrentIndex = index;
            dragStopCallback(index);
        }
        
        Vector3 closest = dragStops[index];
        float timeDiff = Time.time - lastTouchTime;
        if (timeDiff < moveEasingDuration) {
            cachedCamPos = Ease(moveEasingType, lastTouchCameraPos, (Vector2) closest, timeDiff / moveEasingDuration);
        } else {
            cachedCamPos = closest;
        }
    }
    
    Vector3 ClosestDragStop() {
        var index = ClosestDragStopIndex();
        if (index != -1) {
            return dragStops[index];
        } else {
            Debug.LogError("No drag stops defined");
            return Vector3.zero;
        }
    }
    
    int ClosestDragStopIndex() {
        if (forcedDragStopIndex != -1) {
            return forcedDragStopIndex;
        }
        
        Vector3 currentPosition = estaminatedPos;
        float closestDistance = float.PositiveInfinity;
        int index = -1;
        
        for (int i = 0; i < dragStops.Count; ++i) {
            var dragStop = dragStops[i];
            float distance = Vector2.Distance(currentPosition, dragStop);
            if (distance < closestDistance) {
                closestDistance = distance;
                index = i;
            }
        }
        
        return index;
    }
    
    int ClosestNeighbor(int index) {
        int result = -1;
        float closestDistance = float.PositiveInfinity;
        var currentPoint = dragStops[index];
        
        if (index - 1 >= 0) {
            var point = dragStops[index - 1];
            var distance = Vector2.Distance(currentPoint, point);
            var cameraDistance = Vector2.Distance(cachedCamPos, point);
            
            if (cameraDistance <= distance) {
                closestDistance = distance; // it always will be closer than infinity
                result = index - 1;
            }
            
        }
        
        if (index + 1 < dragStops.Count) {
            var point = dragStops[index + 1];
            var distance = Vector2.Distance(currentPoint, point);
            var cameraDistance = Vector2.Distance(cachedCamPos, point);
            
            if (distance < closestDistance && cameraDistance <= distance) {
                result = index + 1;
            }
        }
        
        return result;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public delegate void DragStopCallback(int index);

}

#if !UNITY_3_5
} // namespace
#endif