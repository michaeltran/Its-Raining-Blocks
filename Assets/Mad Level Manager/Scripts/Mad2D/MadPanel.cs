/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(MadBigMeshRenderer))]
[RequireComponent(typeof(MadMaterialStore))]
public class MadPanel : MadNode {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public bool halfPixelOffset = true;
    
    public HashSet<MadSprite> sprites = new HashSet<MadSprite>();
    
    public MadMaterialStore materialStore {
        get {
            if (_materialStore == null) {
                _materialStore = gameObject.AddComponent<MadMaterialStore>();
            }
            
            return _materialStore;
        }
        
        private set {
            _materialStore = value;
        }
    }
    private MadMaterialStore _materialStore;
    
    [HideInInspector]
    MadSprite _focusedSprite;
    int _focusedSpriteModCount;
    public MadSprite focusedSprite {
        get { return _focusedSprite; }
        set {
            _focusedSprite = value;
            _focusedSpriteModCount++;
            if (onFocusChanged != null) {
                onFocusChanged(_focusedSprite);
            }
        }
    }
    
    Camera parentCamera;
    
    // mouse input helpers
    HashSet<MadSprite> hoverSprites = new HashSet<MadSprite>();
    List<MadSprite> mouseDownSprites = new List<MadSprite>();
    
    // ===========================================================
    // Events
    // ===========================================================
    
    public delegate void Event1<T>(T t);
    
    /// <summary>
    /// Occurs when on focus changed. Passes focused sprite or null if nothing is currently focused.
    /// </summary>
    public event Event1<MadSprite> onFocusChanged;

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnEnable() {
        materialStore = GetComponent<MadMaterialStore>();
        parentCamera = FindParent<Camera>();
    }

    void Awake() {
        sprites.Clear();
    }
    
    void Update() {
        // fix the offset
        if (halfPixelOffset) {
            var root = FindParent<MadRootNode>();
            float pixelSize = root.pixelSize;
            
            float x = 0, y = 0;
            if (Screen.height % 2 == 0) {
                y = pixelSize;
            }
            
            if (Screen.width % 2 == 0) {
                x = pixelSize;
            }
            
            transform.localPosition = new Vector3(x, y, 0);
        }
        
        UpdateInput();
    }
    
    void UpdateInput() {
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY
        UpdateTouchInput();
        if (Application.isEditor) {
            UpdateMouseInput();
        }
#else
        UpdateMouseInput();
#endif
    }
    
    void UpdateTouchInput() {
        var touches = Input.touches;
        foreach (var touch in touches) {
            if (touch.phase != TouchPhase.Ended) {
                continue;
            }
            
            var sprites = AllSpritesForScreenPoint(touch.position);
            foreach (var sprite in sprites) {
                var dragHandler = sprite.FindParent<MadDraggable>();
                if (dragHandler != null) {
                    if (dragHandler.dragging) {
                        continue;
                    }
                }
                
                sprite.onTap(sprite);
                sprite.TryFocus();
            }
        }
    }
    
    void UpdateMouseInput() {
        var sprites = new HashSet<MadSprite>(AllSpritesForScreenPoint(Input.mousePosition));
        
        // find newly hovered sprites
        foreach (var sprite in sprites) {
            if (hoverSprites.Add(sprite)) {
                sprite.onMouseEnter(sprite);
            }
        }
        
        // find sprites that are no longer hovered
        if (sprites.Count != hoverSprites.Count) {
            List<MadSprite> unhovered = new List<MadSprite>();
            foreach (var hoverSprite in hoverSprites) {
                if (!sprites.Contains(hoverSprite)) {
                    unhovered.Add(hoverSprite);
                    hoverSprite.onMouseExit(hoverSprite);
                }
            }
            
            foreach (var u in unhovered) {
                hoverSprites.Remove(u);
            }
        }
        
        if (Input.GetMouseButtonDown(0)) {
            foreach (var sprite in hoverSprites) {
                sprite.onMouseDown(sprite);
                mouseDownSprites.Add(sprite);
            }
        }
        
        if (Input.GetMouseButtonUp(0)) {
            int modCount = _focusedSpriteModCount;
            foreach (var sprite in mouseDownSprites) {
                sprite.onMouseUp(sprite);
                sprite.TryFocus();
            }
            
            mouseDownSprites.Clear();
            
            if (modCount == _focusedSpriteModCount && focusedSprite != null) {
                // focus lost to nothing
                focusedSprite.hasFocus = false;
            }
        }
    }
    
    IEnumerable<MadSprite> AllSpritesForScreenPoint(Vector2 point) {
        var ray = parentCamera.ScreenPointToRay(point);
        RaycastHit[] hits = Physics.RaycastAll(ray, 4);
        foreach (var hit in hits) {
            var collider = hit.collider;
            var sprite = collider.GetComponent<MadSprite>();
            if (sprite != null) {
                yield return sprite;
            }
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static MadPanel UniqueOrNull() {
        var panels = GameObject.FindObjectsOfType(typeof(MadPanel));
        if (panels.Length == 1) {
            return panels[0] as MadPanel;
        } else {
            return null;
        }
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif