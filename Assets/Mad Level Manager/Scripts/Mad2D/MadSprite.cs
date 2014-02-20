/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
[ExecuteInEditMode]
public class MadSprite : MadNode {

    // ===========================================================
    // Constants
    // ===========================================================
    
    // ===========================================================
    // Fields
    // ===========================================================
    
    public bool visible = true;
    public bool editorSelectable = true; // is selectable in the editor
    
    public PivotPoint pivotPoint = PivotPoint.Center;
    
    public Color tint = Color.white;
    
    public Texture2D texture;
    Texture2D lastTexture; // for reseting purposes
    
    public Vector2 textureOffset;
    public Vector2 textureRepeat = new Vector2(1, 1);
    
    public int guiDepth;
    public EventFlags eventFlags = EventFlags.All;
    
    protected float left, top, right, bottom;
    
    // live area of the texture is normalized bounds where pixels are visible
    // available only if texture is readable
    public float liveLeft = 0, liveBottom = 0, liveRight = 1, liveTop = 1;
    bool hasLiveBounds;
    bool triedToGetLiveBounds;
    
    public FillType fillType;
    public float fillValue = 1;
    
    public float radialFillOffset = 0;
    public float radialFillLength = 1;
    
    MadPanel parentPanel;
    bool actionsInitialized;
    
    string shaderName;
    SetupShader setupShaderFunction;
    int materialVariation;
    
    
    // ===========================================================
    // Properties
    // ===========================================================
    
    Vector2 _size;
    public Vector2 size {
        get {
            // If size for this sprite was not initially set, then do it now.
            // This causes the sprite to take initial size of texture when texture is set
            // for the first time.
            if (_size == Vector2.zero) {
                if (texture != null) {
                    ResizeToTexture();
                } else {
                    Debug.LogError("Requesting size of sprite without texture.", this);
                }
            }
            
            return _size;
        }
        
        set {
            _size = value;
        }
    }
    
    public Vector2 scalePixels {
        set {
            transform.localScale = new Vector3(value.x / size.x, value.y / size.y, 1);
        }
    }
    
    bool _hasFocus;
    
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="MadSprite"/> has focus. When other
    /// sprite is currently focused, it's focus will be lost.
    /// </summary>
    /// <value><c>true</c> if has focus; otherwise, <c>false</c>.</value>
    public bool hasFocus {
        set {
            if (!_hasFocus && value) {
                if (parentPanel.focusedSprite != null && parentPanel.focusedSprite != this) {
                    // triggering focus lost in that way, to not assign to focusedSprite a null value
                    parentPanel.focusedSprite._hasFocus = false;
                    parentPanel.focusedSprite.onFocusLost(parentPanel.focusedSprite);
                }
                
                parentPanel.focusedSprite = this;
                _hasFocus = true;
                onFocus(this);
            } else if (_hasFocus) {
                if (!value) {
                    parentPanel.focusedSprite = null;
                    onFocusLost(this);
                    _hasFocus = false;
                }
            }
        }
        
        get {
            return _hasFocus;
        }
    }
    
#region Actions
    Action _onMouseEnter = (sprite) => {};
    public Action onMouseEnter {
        get {
            if ((eventFlags & EventFlags.MouseHover) != 0) {
                return _onMouseEnter;
            } else {
                return NullAction;
            }
        }
        set { InitActions(); _onMouseEnter = value; }
    }
    
    Action _onMouseExit = (sprite) => {};
    public Action onMouseExit {
        get {
            if ((eventFlags & EventFlags.MouseHover) != 0) {
                return _onMouseExit;
            } else {
                return NullAction;
            }
        }
        set { InitActions(); _onMouseExit = value; }
    }
    
    Action _onMouseDown = (sprite) => {};
    public Action onMouseDown {
        get {
            if ((eventFlags & EventFlags.MouseClick) != 0) {
                return _onMouseDown;
            } else {
                return NullAction;
            }
        }
        set { InitActions(); _onMouseDown = value; }
    }
    
    Action _onMouseUp = (sprite) => {};
    public Action onMouseUp {
        get {
            var dragHandler = FindParent<MadDraggable>();
            if ((dragHandler == null || !dragHandler.dragging) && (eventFlags & EventFlags.MouseClick) != 0) {
                return _onMouseUp;
            } else {
                return NullAction;
            }
        }
        set { InitActions(); _onMouseUp = value; }
    }
    
    // quick touch (something like mouse click)
    Action _onTap = (sprite) => {};
    public Action onTap {
        get {
            if ((eventFlags & EventFlags.FingerTap) != 0) {
                return _onTap;
            } else {
                return NullAction;
            }
        }
        set { InitActions(); _onTap = value; }
    }
    
    Action _onFocus = (sprite) => {};
    public Action onFocus {
        get {
            if ((eventFlags & EventFlags.Focus) != 0) {
                return _onFocus;
            } else {
                return NullAction;
            }
        }
        set { InitActions(); _onFocus = value; }
    }
    
    Action _onFocusLost = (sprite) => {};
    public Action onFocusLost {
        get {
            if ((eventFlags & EventFlags.Focus) != 0) {
                return _onFocusLost;
            } else {
                return NullAction;
            }
        }
        set { InitActions(); _onFocusLost = value; }
    }
    
    void NullAction(MadSprite s) {}
#endregion
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    // returns bounds to draw gizmos
    public virtual Rect GetBounds() {
        UpdatePivotPoint();
        return new Rect(left * size.x, bottom * size.y, size.x, size.y);
    }
    
    Rect GetLiveBounds() {
        UpdatePivotPoint();
        return new Rect(
            (left + liveLeft) * size.x, (bottom + liveBottom) * size.y,
            (liveRight - liveLeft) * size.x, (liveTop - liveBottom) * size.y);
    }
    
    protected virtual void OnEnable() {
        // enable is called on script reload
        if (!MadNode.Instantiating) { // not possible to register sprite when instantiating
            RegisterSprite();
        }
        
        UpdateTexture();
    }
    
    public void TryFocus() {
        if ((eventFlags & EventFlags.Focus) != 0) {
            hasFocus = true;
        }
    }
    
    void OnDisable() {
        // disable is called on script reload
        UnregisterSprite();
    }
    
    protected virtual void Start() {
        // start is called on scene/script load
        RegisterSprite();
    }
    
    void OnDestroy() {
        // destroy can be called without disable
        UnregisterSprite();
    }
    
    void RegisterSprite() {
        parentPanel = FindParent<MadPanel>();
        
        if (parentPanel != null) {
            parentPanel.sprites.Add(this);
        }
    }
    
    void UnregisterSprite() {
        if (parentPanel != null) {
            parentPanel.sprites.Remove(this);
        }
    }
    
    protected void Update() {
        UpdateTexture();
    
        if (parentPanel == null) {
//            Debug.LogError("Sprite must be placed under a panel!", gameObject);
        }
        
        if (NeedLiveBounds()) {
            RecalculateLiveBounds();
        }
    }
    
    void UpdateTexture() {
        if (texture != lastTexture) {
            // texture changed or set as new, reset
            liveLeft = liveBottom = 0;
            liveRight = liveBottom = 0;
            hasLiveBounds = false;
            triedToGetLiveBounds = false;
            
            if (NeedLiveBounds()) {
                RecalculateLiveBounds();
            }
            
            lastTexture = texture;
        }
    }
    
    bool NeedLiveBounds() {
        return fillType != FillType.None && !hasLiveBounds && !triedToGetLiveBounds && texture != null;
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    public void SetMaterial(string shader, SetupShader setupShader) {
        shaderName = shader;
        setupShaderFunction = setupShader;
    }
    
    void InitActions() {
        if (!Application.isPlaying) {
            return;
        }
    
        if (actionsInitialized) {
            return;
        }
        
        BoxCollider box;
        box = gameObject.GetComponent<BoxCollider>();
        if (box == null) {
            box = gameObject.AddComponent<BoxCollider>();
        }
        var bounds = GetBounds();
        box.center = bounds.center;
        box.size = new Vector3(bounds.width, bounds.height, 0.01f);
        
        
        actionsInitialized = true;
    }
    
#if UNITY_EDITOR
    void OnDrawGizmos() {
        if (!editorSelectable) {
            // do not draw any gizmo if sprite is not selectable in the editor
            return;
        }
        
        if (texture == null) {
            // no texture, no size
            return;
        }
    
        // Draw the gizmo
        Gizmos.matrix = transform.localToWorldMatrix;
        
        Gizmos.color = (UnityEditor.Selection.activeGameObject == gameObject)
            ? Color.green : new Color(1, 1, 1, 0.2f);
        var bounds = GetBounds();
        Gizmos.DrawWireCube(bounds.center, new Vector3(bounds.width, bounds.height, 0));
        
        // Make the widget selectable
        
        Gizmos.color = Color.clear;
        Gizmos.DrawCube(bounds.center,
            new Vector3(bounds.width, bounds.height, 1f * (guiDepth + 1)));
            
        // Draw live gizmo
        if (texture != null) {
            if (hasLiveBounds) {
                Gizmos.color = Color.red;
                bounds = GetLiveBounds();
                Gizmos.DrawWireCube(bounds.center, new Vector3(bounds.width, bounds.height, 0));
            }
        }
    }
#endif

    public virtual bool CanDraw() {
        bool underThePanel = UnderThePanel();
        
        if (!underThePanel) {
            Debug.LogError("Sprite is not located under the Panel and it cannot be rendered.", this);
        }
        
        if (fillType != FillType.None && fillValue == 0) {
            return false;
        }
        
        return texture != null && underThePanel;
    }
    
    public virtual void DrawOn(ref MadList<Vector3> vertices, ref MadList<Color32> colors, ref MadList<Vector2> uv,
               ref MadList<int> triangles, out Material material) {
        
        UpdatePivotPoint();
    
        if ((fillType == FillType.RadialCW || fillType == FillType.RadialCCW) && (fillValue != 1 || radialFillLength != 1)) {
            DrawOnQuad(ref vertices, ref colors, ref uv, ref triangles);
        } else {
            DrawOnRegular(ref vertices, ref colors, ref uv, ref triangles);
        }
        
        if (!string.IsNullOrEmpty(shaderName) && setupShaderFunction != null) {
            if (materialVariation == 0) {
                material = parentPanel.materialStore.CreateUnique(texture, shaderName, out materialVariation);
            } else {
                material = parentPanel.materialStore.MaterialFor(texture, shaderName, materialVariation);
            }
            
            setupShaderFunction(material);
        } else {
            material = parentPanel.materialStore.MaterialFor(texture, MadLevelManager.MadShaders.UnlitTint);
        }
    }
    
    public void DrawOnRegular(ref MadList<Vector3> vertices, ref MadList<Color32> colors, ref MadList<Vector2> uv,
            ref MadList<int> triangles) {
        var matrix = PanelToSelfMatrix();
        var bounds = GetBounds();
        
        float vLeft = 0;
        float vTop = size.y;
        float vRight = size.x;
        float vBottom = 0;
        
        float uvLeft = textureOffset.x;
        float uvTop = textureOffset.y + textureRepeat.y;
        float uvRight = textureOffset.x + textureRepeat.x;
        float uvBottom = textureOffset.y;
        
        fillValue = Mathf.Clamp01(fillValue);
        
        if (fillValue != 1) {
            switch (fillType) {
                case FillType.LeftToRight:
                    vRight *= LiveCoordX(fillValue);
                    uvRight += LiveCoordX(fillValue) - 1;
                    break;
                case FillType.RightToLeft:
                    vLeft += LiveCoordX(1 - fillValue) * size.x;
                    uvLeft += LiveCoordX(1 - fillValue);
                    break;
                case FillType.BottomToTop:
                    vTop *= LiveCoordY(fillValue);
                    uvTop += LiveCoordY(fillValue) - 1;
                    break;
                case FillType.TopToBottom:
                    vBottom += LiveCoordY(1 - fillValue) * size.y;
                    uvBottom += LiveCoordY(1 - fillValue);
                    break;
                case FillType.ExpandHorizontal:
                    {
                        float fv = 0.5f + fillValue / 2;
                        vRight *= LiveCoordX(fv);
                        uvRight += LiveCoordX(fv) - 1;
                        vLeft += LiveCoordX(1 - fv) * size.x;
                        uvLeft += LiveCoordX(1 - fv);
                    }
                    break;
                case FillType.ExpandVertical:
                    {
                        float fv = 0.5f + fillValue / 2;
                        vTop *= LiveCoordY(fv);
                        uvTop += LiveCoordY(fv) - 1;
                        vBottom += LiveCoordY(1 - fv) * size.y;
                        uvBottom += LiveCoordY(1 - fv);
                    }
                    break;
                case FillType.None:
                    // change nothing
                    break;
            }
        }
        
        // left-bottom
        vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(new Vector3(vLeft, vBottom, 0), bounds)));
        // left-top
        vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(new Vector3(vLeft, vTop, 0), bounds)));
        // right - top
        vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(new Vector3(vRight, vTop, 0), bounds)));
        // right - bottom
        vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(new Vector3(vRight, vBottom, 0), bounds)));
        
        Color32 color32 = (Color32) tint;
        
        colors.Add(color32);
        colors.Add(color32);
        colors.Add(color32);
        colors.Add(color32);
        
        uv.Add(new Vector2(uvLeft, uvBottom));
        uv.Add(new Vector2(uvLeft, uvTop));
        uv.Add(new Vector2(uvRight, uvTop));
        uv.Add(new Vector2(uvRight, uvBottom));
        
        int offset = vertices.Count - 4;
        triangles.Add(0 + offset);
        triangles.Add(1 + offset);
        triangles.Add(2 + offset);
        
        triangles.Add(0 + offset);
        triangles.Add(2 + offset);
        triangles.Add(3 + offset);
    }
    
    public void DrawOnQuad(ref MadList<Vector3> vertices, ref MadList<Color32> colors, ref MadList<Vector2> uv,
            ref MadList<int> triangles) {
        
        bool invert = fillType == FillType.RadialCCW;
            
        var matrix = PanelToSelfMatrix();
        
        var bounds = GetBounds();
 
        var topLeftQuad = new Quad(invert);
        var topRightQuad = new Quad(invert);
        var bottomRightQuad = new Quad(invert);
        var bottomLeftQuad = new Quad(invert);
        
        topLeftQuad.anchor = Quad.Point.BottomRight;
        topRightQuad.anchor = Quad.Point.BottomLeft;
        bottomRightQuad.anchor = Quad.Point.TopLeft;
        bottomLeftQuad.anchor = Quad.Point.TopRight;
        
        var topLeftQuad2 = new Quad(topLeftQuad);
        var topRightQuad2 = new Quad(topRightQuad);
        var bottomRightQuad2 = new Quad(bottomRightQuad);
        var bottomLeftQuad2 = new Quad(bottomLeftQuad);
        
        // creating 8 quads instead of 8 because when using offset it may display one additional quad
        // and the simplest way is to create 8 quads and wrap around
        Quad[] ordered = new Quad[8];
        
        if (!invert) {
            ordered[0] = topRightQuad;
            ordered[1] = bottomRightQuad;
            ordered[2] = bottomLeftQuad;
            ordered[3] = topLeftQuad;
            ordered[4] = topRightQuad2;
            ordered[5] = bottomRightQuad2;
            ordered[6] = bottomLeftQuad2;
            ordered[7] = topLeftQuad2;
        } else {
            ordered[7] = topRightQuad2;
            ordered[6] = bottomRightQuad2;
            ordered[5] = bottomLeftQuad2;
            ordered[4] = topLeftQuad2;
            ordered[3] = topRightQuad;
            ordered[2] = bottomRightQuad;
            ordered[1] = bottomLeftQuad;
            ordered[0] = topLeftQuad;
        }
        
        float rOffset = radialFillOffset % 1;
        if (rOffset < 0) {
            rOffset += 1;
        }
        
        float fillValue = Mathf.Clamp01(this.fillValue) * radialFillLength;
        float fillStart = rOffset * 4;
        float fillEnd = (rOffset + fillValue) * 4;
        
        for (int i = Mathf.FloorToInt(fillStart); i < Mathf.CeilToInt(fillEnd); ++i) {
            var quad = ordered[i % 8];
        
            if (i < fillStart) {
                quad.offset = fillStart - i;
            } else {
                quad.offset = 0;
            }
            
            if (fillEnd > i + 1) {
                quad.progress = 1 - quad.offset;
            } else {
                quad.progress = fillEnd - i - quad.offset;
            }
        }
        
        float sx = size.x;
        float sy = size.y;
        float sx2 = sx / 2;
        float sy2 = sy / 2;
        
        // collect points anv uvs
        List<Vector2[]> genPoints = new List<Vector2[]>();
        List<Vector2[]> genUvs = new List<Vector2[]>();
        
        genPoints.Add(topRightQuad.Points(sx2, sy, sx, sy2));
        genUvs.Add(topRightQuad.Points(0.5f, 1, 1, 0.5f));
        genPoints.Add(topRightQuad2.Points(sx2, sy, sx, sy2));
        genUvs.Add(topRightQuad2.Points(0.5f, 1, 1, 0.5f));
        
        genPoints.Add(bottomRightQuad.Points(sx2, sy2, sx, 0));
        genUvs.Add(bottomRightQuad.Points(0.5f, 0.5f, 1, 0));
        genPoints.Add(bottomRightQuad2.Points(sx2, sy2, sx, 0));
        genUvs.Add(bottomRightQuad2.Points(0.5f, 0.5f, 1, 0));
        
        genPoints.Add(bottomLeftQuad.Points(0, sy2, sx2, 0));
        genUvs.Add(bottomLeftQuad.Points(0, 0.5f, 0.5f, 0));
        genPoints.Add(bottomLeftQuad2.Points(0, sy2, sx2, 0));
        genUvs.Add(bottomLeftQuad2.Points(0, 0.5f, 0.5f, 0));
        
        genPoints.Add(topLeftQuad.Points(0, sy, sx2, sy2));
        genUvs.Add(topLeftQuad.Points(0, 1, 0.5f, 0.5f));
        genPoints.Add(topLeftQuad2.Points(0, sy, sx2, sy2));
        genUvs.Add(topLeftQuad2.Points(0, 1, 0.5f, 0.5f));
        
        // add ass triangles
        Color32 color32 = (Color32) tint;
        
        for (int i = 0; i < 8; ++i) {
            var points = genPoints[i];
            var uvs = genUvs[i];
            
            if (points.Length == 0) {
                continue;
            }
            
            int offset = vertices.Count;
            
            for (int j = 0; j < points.Length; ++j) {
                vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(points[j], bounds)));
                uv.Add(uvs[j]);
                colors.Add(color32);
            }
            
            triangles.Add(0 + offset);
            triangles.Add(1 + offset);
            triangles.Add(2 + offset);
            
            // for quads
            if (points.Length > 3) {
                triangles.Add(0 + offset);
                triangles.Add(2 + offset);
                triangles.Add(3 + offset);
            }
        }
        
        
    }
    
    class Quad {
        public Point anchor;
        public float offset = 0;
        public float progress = 0;
        public bool invert = false;
        
        public Quad(bool invert) {
            this.invert = invert;
        }
        
        public Quad(Quad other) {
            anchor = other.anchor;
            offset = other.offset;
            progress = other.progress;
            invert = other.invert;
        }
        
        public Vector2[] Points(float left, float top, float right, float bottom) {
            if (progress == 0) {
                return new Vector2[0];
            } else if (progress == 1) {
                return new Vector2[] {
                    new Vector2(left, bottom),
                    new Vector2(left, top),
                    new Vector2(right, top),
                    new Vector2(right, bottom),
                };
            } else {
                float progressY = Y(progress + offset);
                float hy = left + (right - left) * progressY;
                float vy = bottom + (top - bottom) * progressY;
                
                float progressOffset = offset + progress;
                
                float offsetY = Y(offset);
                float ohy = left + (right - left) * offsetY;
                float ovy = bottom + (top - bottom) * offsetY;
            
                switch (anchor) {
                    case Point.BottomLeft:
                        if (!invert) {
                            if (progressOffset < 0.5f) {
                                return new Vector2[] {
                                    new Vector2(left, bottom),
                                    new Vector2(ohy, top),
                                    new Vector2(hy, top),
                                };
                            } else {
                                if (offset < 0.5f) {
                                    return new Vector2[] {
                                        new Vector2(left, bottom),
                                        new Vector2(ohy, top),
                                        new Vector2(right, top),
                                        new Vector2(right, vy),
                                    };
                                } else {
                                    return new Vector2[] {
                                        new Vector2(left, bottom),
                                        new Vector2(right, ovy),
                                        new Vector2(right, vy),
                                    };
                                }
                                
                            }
                        } else {
                            if (progressOffset < 0.5f) {
                                return new Vector2[] {
                                    new Vector2(left, bottom),
                                    new Vector2(right, ovy),
                                    new Vector2(right, vy),
                                };
                            } else {
                                if (offset < 0.5f) {
                                    return new Vector2[] {
                                        new Vector2(left, bottom),
                                        new Vector2(right, ovy),
                                        new Vector2(right, top),
                                        new Vector2(hy, top),
                                    };
                                } else {
                                    return new Vector2[] {
                                        new Vector2(left, bottom),
                                        new Vector2(ohy, top),
                                        new Vector2(hy, top),
                                    };
                                }
                            }
                        }
                    
                    case Point.TopLeft:
                        if (!invert) {
                            if (progressOffset < 0.5f) {
                                return new Vector2[] {
                                    new Vector2(left, top),
                                    new Vector2(right, top - ovy),
                                    new Vector2(right, top - vy),
                                };
                            } else {
                                if (offset < 0.5f) {
                                    return new Vector2[] {
                                        new Vector2(left, top),
                                        new Vector2(right, top - ovy),
                                        new Vector2(right, bottom),
                                        new Vector2(hy, bottom),
                                    };
                                } else {
                                    return new Vector2[] {
                                        new Vector2(left, top),
                                        new Vector2(ohy, bottom),
                                        new Vector2(hy, bottom),
                                    };
                                }
                                
                            }
                        } else {
                            if (progressOffset < 0.5f) {
                                return new Vector2[] {
                                    new Vector2(left, top),
                                    new Vector2(ohy, bottom),
                                    new Vector2(hy, bottom),
                                };
                            } else {
                                if (offset < 0.5f) {
                                    return new Vector2[] {
                                        new Vector2(left, top),
                                        new Vector2(ohy, bottom),
                                        new Vector2(right, bottom),
                                        new Vector2(right, top - vy),
                                    };
                                } else {
                                    return new Vector2[] {
                                        new Vector2(left, top),
                                        new Vector2(right, top - ovy),
                                        new Vector2(right, top - vy),
                                    };
                                }
                            }
                        }
                        
                    case Point.TopRight:
                        if (!invert) {
                            if (progressOffset < 0.5f) {
                                return new Vector2[] {
                                    new Vector2(right, top),
                                    new Vector2(right - ohy, bottom),
                                    new Vector2(right - hy, bottom),
                                };
                            } else {
                                if (offset < 0.5f) {
                                    return new Vector2[] {
                                        new Vector2(right, top),
                                        new Vector2(right - ohy, bottom),
                                        new Vector2(left, bottom),
                                        new Vector2(left, top - vy),
                                    };
                                } else {
                                    return new Vector2[] {
                                        new Vector2(right, top),
                                        new Vector2(left, top - ovy),
                                        new Vector2(left, top - vy),
                                    };
                                }
                            }
                        } else {
                            if (progressOffset < 0.5f) {
                                return new Vector2[] {
                                    new Vector2(right, top),
                                    new Vector2(left, top - ovy),
                                    new Vector2(left, top - vy),
                                };
                            } else {
                                if (offset < 0.5f) {
                                    return new Vector2[] {
                                        new Vector2(right, top),
                                        new Vector2(left, top - ovy),
                                        new Vector2(left, bottom),
                                        new Vector2(right - hy, bottom),
                                    };
                                } else {
                                    return new Vector2[] {
                                        new Vector2(right, top),
                                        new Vector2(right - ohy, bottom),
                                        new Vector2(right - hy, bottom),
                                    };
                                }
                            }
                        }
                        
                    case Point.BottomRight:
                        if (!invert) {
                            if (progressOffset < 0.5f) {
                                return new Vector2[] {
                                    new Vector2(right, bottom),
                                    new Vector2(left, ovy),
                                    new Vector2(left, vy),
                                };
                            } else {
                                if (offset < 0.5f) {
                                    return new Vector2[] {
                                        new Vector2(right, bottom),
                                        new Vector2(left, ovy),
                                        new Vector2(left, top),
                                        new Vector2(right - hy, top),
                                    };
                                } else {
                                    return new Vector2[] {
                                        new Vector2(right, bottom),
                                        new Vector2(right - ohy, top),
                                        new Vector2(right - hy, top),
                                    };
                                }
                            }
                        } else {
                            if (progressOffset < 0.5f) {
                                return new Vector2[] {
                                    new Vector2(right, bottom),
                                    new Vector2(right - ohy, top),
                                    new Vector2(right - hy, top),
                                };
                            } else {
                                if (offset < 0.5f) {
                                    return new Vector2[] {
                                        new Vector2(right, bottom),
                                        new Vector2(right - ohy, top),
                                        new Vector2(left, top),
                                        new Vector2(left, vy),
                                    };
                                } else {
                                    return new Vector2[] {
                                        new Vector2(right, bottom),
                                        new Vector2(left, ovy),
                                        new Vector2(left, vy),
                                    };
                                }
                            }
                        }
                }
            }
            
            Debug.LogError("Should not be here");
            return new Vector2[0];
        }
        
        float Y(float val) {
            float x = 1;
            
            float p = (val < 0.5f) ? val : 1 - val;
            float angle = p * 90 * Mathf.Deg2Rad;
            
            float y = Mathf.Tan(angle) * x;
            return y;
        }
        
        public enum Point {
            TopLeft,
            TopRight,
            BottomRight,
            BottomLeft,
        }
    }
    
    float LiveCoordX(float pos) {
        return liveLeft + (liveRight - liveLeft) * pos;
    }
    
    float LiveCoordY(float pos) {
        return liveBottom + (liveTop - liveBottom) * pos;
    }
    
    // returns matrix combination from panel (excluding) to self (including)
    protected Matrix4x4 PanelToSelfMatrix() {
        GameObject[] objects = HierarchyFromPanel();
        Matrix4x4 matrix = Matrix4x4.identity;
        
        foreach (var obj in objects) {
            var localMatrix = Matrix4x4.TRS(
                obj.transform.localPosition, obj.transform.localRotation, obj.transform.localScale);
            matrix *= localMatrix;
        }
        
        return matrix;
    }
    
    // returns all game objects that constructs hierarchy from panel (excluding) to self (including)
    // in this order
    GameObject[] HierarchyFromPanel() {
        List<GameObject> objects = new List<GameObject>();
        GameObject current = gameObject;
        
        while (current != parentPanel.gameObject) {
            objects.Add(current);
            current = current.transform.parent.gameObject;
        }
        
        objects.Reverse();
        return objects.ToArray();
    }
    
    bool UnderThePanel() {
        GameObject current = gameObject;
        
        while (current != parentPanel.gameObject) {
            var parent = current.transform.parent;
            if (parent == null) {
                return false;
            }
            
            current = current.transform.parent.gameObject;
        }
        
        return true;
    }
              
    protected void UpdatePivotPoint() {
        switch (pivotPoint) {
            case PivotPoint.BottomLeft:
                left = 0; bottom = 0;
                break;
            case PivotPoint.TopLeft:
                left = 0; bottom = -1;
                break;
            case PivotPoint.TopRight:
                left = -1; bottom = -1;
                break;
            case PivotPoint.BottomRight:
                left = -1; bottom = 0;
                break;
            case PivotPoint.Left:
                left = 0; bottom = -0.5f;
                break;
            case PivotPoint.Top:
                left = -0.5f; bottom = -1f;
                break;
            case PivotPoint.Right:
                left = -1f; bottom = -0.5f;
                break;
            case PivotPoint.Bottom:
                left = -0.5f; bottom = 0f;
                break;
            case PivotPoint.Center:
                left = -0.5f; bottom = -0.5f;
                break;
            default:
                Debug.LogError("Unkwnown pivot point: " + pivotPoint);
                break;
        }
        
        top = bottom + 1;
        right = left + 1;
    }
    
    protected Vector3 PivotPointTranslate(Vector3 p, Rect bounds) {
        return new Vector3(p.x + bounds.width * left, p.y + bounds.height * bottom, p.z);
    }
    
    // resizes sprite to match texture size
    public void ResizeToTexture() {
        size = new Vector2(texture.width, texture.height);
    }
    
    public void MinMaxDepthRecursively(out int min, out int max) {
        min = guiDepth;
        max = guiDepth;
        
        var sprites = MadTransform.FindChildren<MadSprite>(transform);
        foreach (var sprite in sprites) {
            min = Mathf.Min(min, sprite.guiDepth);
            max = Mathf.Max(max, sprite.guiDepth);
        }
    }
    
    public void RecalculateLiveBounds() {
        // recalculating live bounds may throw an exception when texture is not readable
        triedToGetLiveBounds = true;
        
#if UNITY_EDITOR
        var importer = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
        if (!importer.isReadable) {
#if MAD_DEBUG
            Debug.Log("Texture must be readable");
#endif
            return; // texture must be readable, but I won't display any error
        }
#endif        
    
        Color32[] pixels = texture.GetPixels32();
        int left = -1, bottom = -1, right = -1, top = -1;
        int w = texture.width;
        int h = texture.height;
        
        int index = 0;
        for (int y = 0; y < h; ++y) {
            for (int x = 0; x < w; ++x) {
                Color32 c = pixels[index++];
                if (IsOpaque(c)) {
                    if (left == -1 || x < left) {
                        left = x;
                    }
                    
                    if (bottom == -1) {
                        bottom = y;
                    }
                    
                    if (x > right) {
                        right = x;
                    }
                    
                    top = y;
                }
            }
        }
        
        liveLeft = (float) left / w;
        liveBottom = (float) bottom / h;
        liveRight = (float) right / w;
        liveTop = (float) top / h;
        
        hasLiveBounds = true;
    }
    
    bool IsOpaque(Color32 color) {
        return color.a != 0;
    }
    
#region Animations
    public void AnimScale(Vector3 from, Vector3 to, float time, MadiTween.EaseType easing) {
        transform.localScale = from;
        AnimScaleTo(to, time, easing);
    }
    
    public void AnimScaleTo(Vector3 scale, float time, MadiTween.EaseType easing) {
        MadiTween.ScaleTo(gameObject, MadiTween.Hash(
            "scale", scale,
            "time", time,
            "easetype", easing
        ));
    }
    
    public void AnimMove(Vector3 from, Vector3 to, float time, MadiTween.EaseType easing) {
        transform.localPosition = from;
        AnimMoveTo(to, time, easing);
    }
    
    public void AnimMoveTo(Vector3 target, float time, MadiTween.EaseType easing) {
        MadiTween.MoveTo(gameObject, MadiTween.Hash(
            "position", target,
            "time", time,
            "easetype", easing,
            "islocal", true
        ));
    }
    
    public void AnimRotate(Vector3 from, Vector3 to, float time, MadiTween.EaseType easing) {
        transform.localScale = from;
        AnimScaleTo(to, time, easing);
    }
    
    public void AnimRotateTo(Vector3 rotation, float time, MadiTween.EaseType easing) {
        MadiTween.RotateTo(gameObject, MadiTween.Hash(
            "rotation", rotation,
            "time", time,
            "easetype", easing,
            "islocal", true
        ));
    }
    
    public void AnimColor(Color from, Color to, float time, MadiTween.EaseType easing) {
        tint = from;
        AnimColorTo(to, time, easing);
    }
    
    public void AnimColorTo(Color color, float time, MadiTween.EaseType easing) {
        MadiTween.ValueTo(gameObject,
            MadiTween.Hash(
                "from", tint,
                "to", color,
                "time", time,
                "onupdate", "OnTintChange",
                "easetype", easing
            ));
    }
    
    // color change receiver
    void OnTintChange(Color color) {
        tint = color;
    }
#endregion

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum PivotPoint {
        BottomLeft,
        TopLeft,
        TopRight,
        BottomRight,
        Left,
        Top,
        Right,
        Bottom,
        Center,
    }
    
    public enum FillType {
        None,
        LeftToRight,
        RightToLeft,
        BottomToTop,
        TopToBottom,
        ExpandHorizontal,
        ExpandVertical,
        RadialCW,
        RadialCCW,
    }
    
    public enum EventFlags {
        None = 0,
        /// <summary>
        /// Mouse enter and mouse left events.
        /// </summary>
        MouseHover = 1,
        /// <summary>
        /// Mouse click events.
        /// </summary>
        MouseClick = 2,
        /// <summary>
        /// Finger tap events.
        /// </summary>
        FingerTap = 4,
        /// <summary>
        /// Focus event.
        /// </summary>
        Focus = 8,
        
        All = 15
    }
    
    public delegate void Action(MadSprite sprite);
    public delegate void SetupShader(Material material);

}

#if !UNITY_3_5
} // namespace
#endif