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

public class MadFreeDraggable : MadDraggable {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public Rect dragArea = new Rect(-200, -200, 400, 400);
    public Vector2 dragStartPosition = new Vector2(-200, 200);
    
    public bool scaling = false;
    public float scalingMax = 2;
    public float scalingMin = 0.25f;
    
    private Vector3 scaleSource;
    private Vector3 scaleTarget;
    private float scaleStartTime;
    
    public bool moveEasing = true;
    public bool scaleEasing = true;
    public MadiTween.EaseType scaleEasingType = MadiTween.EaseType.easeOutQuad;
    public float scaleEasingDuration = 0.5f;
    
    // current move anim
    private bool moveAnim;
    private Vector3 moveAnimStartPosition;
    private Vector3 moveAnimEndPosition;
    private float moveAnimStartTime;
    private float moveAnimDuration;
    private MadiTween.EaseType moveAnimEaseType;
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void OnValidate() {
        scalingMin = Mathf.Min(scalingMin, scalingMax);
        scalingMax = Mathf.Max(scalingMin, scalingMax);
    }
    
    void OnDrawGizmosSelected() {
        var center = dragArea.center;
        center = transform.TransformPoint(center);
        
        var topRight = new Vector3(dragArea.xMax, dragArea.yMax, 0.01f);
        topRight = transform.TransformPoint(topRight);
        var bottomLeft = new Vector3(dragArea.xMin, dragArea.yMin, 0.01f);
        bottomLeft = transform.TransformPoint(bottomLeft);
        
        var extends = new Vector2(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, extends);
    }
    
    protected override void Start() {
        base.Start();
        cameraPos = dragStartPosition;
        scaleSource = scaleTarget = transform.localScale;
    }
    
//    float lastDragTime;
    
    protected override void Update() {
        base.Update();
        
        cachedCamPos = cameraPos;
        
        UpdateMoving();
        
        if (!IsTouchingSingle()) {
            dragging = false;
            
            if (scaling) {
                float scaleModifier = ScaleModifier();
                if (scaleModifier != 0) {
                    scaleSource = transform.localScale;
                    scaleTarget += scaleTarget * scaleModifier;
                    scaleTarget = ClampLocalScale(scaleTarget);
                    scaleStartTime = Time.time;
                }
            }
            
            float timeDiff = Time.time - lastTouchTime;
            if (moveEasing && timeDiff < moveEasingDuration && !moveAnim) {
                MoveToLocal(estaminatedPos, moveEasingType, moveEasingDuration);
            } else {
                Clear();
            }
            
            cameraPos = cachedCamPos;
            ClampPosition();
            
        } else { // toching single (drag)
            StopMoving();
        
            Vector2 lastCamPos = cachedCamPos;
            var touchPos = TouchPosition();
            
            if (IsTouchingJustStarted()) {
                lastPosition = touchPos;
            } else {
                cachedCamPos -= touchPos - lastPosition;
                lastPosition = touchPos;
            }
            
            RegisterDelta(cachedCamPos - lastCamPos);
            
            if (dragDistance > deadDistance) {
                dragging = true;
                cameraPos = cachedCamPos;
                ClampPosition();
            }
        }
        
        if (scaling) {
            float timeDiff = Time.time - scaleStartTime;
            if (scaleEasing && timeDiff < scaleEasingDuration) {
                transform.localScale = Ease(scaleEasingType, scaleSource, scaleTarget, timeDiff / scaleEasingDuration);
            } else {
                transform.localScale = scaleTarget;
            }
        }
    }
    
    void UpdateMoving() {
        if (!moveAnim) {
            return;
        }
        
        if (moveAnimStartTime + moveAnimDuration > Time.time) {
            float percentage = (Time.time - moveAnimStartTime) / moveAnimDuration;
            cachedCamPos = Ease(moveAnimEaseType, moveAnimStartPosition, moveAnimEndPosition, percentage);
        } else {
            cachedCamPos = moveAnimEndPosition;
            moveAnim = false;
        }
        
        cachedCamPos = MadMath.ClosestPoint(dragArea, cachedCamPos);
 }
    
    public void MoveToLocal(Vector2 position) {
        MoveToLocal(position, default(MadiTween.EaseType), 0);
    }
    
    public void MoveToLocal(Vector2 position, MadiTween.EaseType easeType, float time) {
        if (time == 0) {
            cameraPos = MadMath.ClosestPoint(dragArea, position);    
            moveAnim = false;
        } else {
            moveAnimStartPosition = cachedCamPos;
            moveAnimEndPosition = position;
            moveAnimStartTime = Time.time;
            moveAnimDuration = time;
            moveAnimEaseType = easeType;
            moveAnim = true;
        }
    }
    
    void StopMoving() {
        moveAnim = false;
    }
    
    void ClampPosition() {
        var position = cameraPos;
        var rootNode = MadTransform.FindParent<MadRootNode>(transform);
        
        var areaBottomLeft = new Vector2(dragArea.xMin, dragArea.yMin);
        var areaTopRight = new Vector2(dragArea.xMax, dragArea.yMax);
        
        var screenBottomLeft = transform.InverseTransformPoint(rootNode.ScreenGlobal(0, 0));
        var screenTopRight = transform.InverseTransformPoint(rootNode.ScreenGlobal(1, 1));
        
        float deltaLeft = screenBottomLeft.x - areaBottomLeft.x;
        float deltaRight = screenTopRight.x - areaTopRight.x;
        float deltaTop = screenTopRight.y - areaTopRight.y;
        float deltaBottom = screenBottomLeft.y - areaBottomLeft.y;
        
        // apply scale because transform matrix does not contain it
        float scale = transform.localScale.x;
        deltaLeft *= scale;
        deltaRight *= scale;
        deltaTop *= scale;
        deltaBottom *= scale;
        
        if (dragArea.width < (screenTopRight.x - screenBottomLeft.x)) { // drag area smaller
            position.x = (areaTopRight.x + areaBottomLeft.x) / 2;
        } else if (deltaLeft < 0) {
            position.x -= deltaLeft;
        } else if (deltaRight > 0) {
            position.x -= deltaRight;
        }
        
        if (dragArea.height < (screenTopRight.y - screenBottomLeft.y)) {
            position.y = (areaBottomLeft.y + areaTopRight.y) / 2;
        } else if (deltaBottom < 0) {
            position.y -= deltaBottom;
        } else if (deltaTop > 0) {
            position.y -= deltaTop;
        }
        
        cameraPos = position;
    }
    
    Vector3 ClampLocalScale(Vector3 scale) {
        if (scale.x < scalingMin) {
            return new Vector3(scalingMin, scalingMin, scalingMin);
        } else if (scale.x > scalingMax) {
            return new Vector3(scalingMax, scalingMax, scalingMax);
        } else {
            return scale;
        }
    }
    
    float ScaleModifier() {
        #if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY
        if (!Application.isEditor) {
            if (multiTouches.Count == 2) {
                var firstPosition = multiTouches[0].position;
                var secondPosition = multiTouches[1].position;
                
                Vector2 moveVec = (secondPosition - firstPosition);
                float currentDistance = moveVec.magnitude;
                
                if (lastDoubleTouchDistance != 0) {
                    float delta = lastDoubleTouchDistance - currentDistance;
                    lastDoubleTouchDistance = currentDistance;
                    
                    // calculte scale speed by touch angle
                    var moveVecNorm = moveVec.normalized;
                    float speed =
                        Screen.width * Mathf.Abs(moveVecNorm.x) + Screen.height * Mathf.Abs(moveVecNorm.y);
                    
                    return -delta * 2 / speed;
                } else {
                    lastDoubleTouchDistance = currentDistance;
                }
                
            } else {
                lastDoubleTouchDistance = 0;
                return 0;
            }
            return 0;
        } else {
            return Input.GetAxis("Mouse ScrollWheel");
        }
        #else
        return Input.GetAxis("Mouse ScrollWheel");
        #endif
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif