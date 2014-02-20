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

public class MadText : MadSprite {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public MadFont font;
    public string text = "";
    
    public float scale = 24;
    public float letterSpacing = 1;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    protected new void OnEnable() {
        base.OnEnable();
    }
    
    public override Rect GetBounds() {
        if (!CanDraw()) {
            return new Rect();
        }
        
        UpdatePivotPoint();
        
        float width = 0;
        float height = 0;
    
        foreach (char c in text) {
            var glyph = font.GlyphFor(c);
            if (glyph == null) {
                Debug.LogWarning("No glyph found for '" + c + "'");
                continue;
            }
            
            float xAdvance;
            var bounds = GlyphBounds(glyph, out xAdvance);
            width += xAdvance;
            height += bounds.height;
            
//            
//            float scaleMod = scale / font.data.infoSize;
//            
//            float w = (scale / glyph.height) * glyph.width * font.textureAspect;
//            width += w;
//            
//            if (c != ' ') {
//                width += letterSpacing;
//            }
        }
        
        var rawBounds = new Rect(0, 0, width, scale);
        
        var start = PivotPointTranslate(new Vector2(0, 0), rawBounds);
        var end = PivotPointTranslate(new Vector2(width, scale), rawBounds);
        
        return new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
    }
    
    private Rect GlyphBounds(MadFont.Glyph g, out float xAdvance) {
        float realScale = scale;
        float xOffset = 0, yOffset = 0;
    
        float baseScale = font.data.infoSize / (float) font.data.commonScaleH;
        realScale = g.height / baseScale * scale;
        xOffset = g.xOffset / baseScale * scale;
        yOffset = g.yOffset / baseScale * scale; // wrong?
        xAdvance = g.xAdvance / baseScale * scale * font.textureAspect;
        
        float w = (realScale / g.height) * g.width;
        
        return new Rect(xOffset, yOffset, w * font.textureAspect, realScale);
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    public override bool CanDraw() {
        return font != null && !string.IsNullOrEmpty(text);
    }
          
    public override void DrawOn(ref MadList<Vector3> vertices, ref MadList<Color32> colors, ref MadList<Vector2> uv,
                ref MadList<int> triangles, out Material material) {
            
        UpdatePivotPoint();
        
        var matrix = PanelToSelfMatrix();
        var bounds = GetBounds();
        
        material = font.material;
        float x = 0;
        
        foreach (char c in text) {
            int offset = vertices.Count;
        
            var glyph = font.GlyphFor(c);
            if (glyph == null) {
                Debug.LogWarning("Glyph not found: '" + c + "'");
                continue;
            }
            
//            float w = (scale / glyph.height) * glyph.width * font.textureAspect;
            float xAdvance;
            var gBounds = GlyphBounds(glyph, out xAdvance);
            
            if (c != ' ') { // render anything but space
                float left = x + gBounds.x;
                float bottom = gBounds.y;
                float right = left + gBounds.width;
                float top = bottom + gBounds.height;
            
                vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(new Vector3(left, bottom, 0), bounds)));
                vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(new Vector3(left, top, 0), bounds)));
                vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(new Vector3(right, top, 0), bounds)));
                vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(new Vector3(right, bottom, 0), bounds)));
                
                colors.Add(tint);
                colors.Add(tint);
                colors.Add(tint);
                colors.Add(tint);
                
                uv.Add(new Vector2(glyph.uMin, glyph.vMin));
                uv.Add(new Vector2(glyph.uMin, glyph.vMax));
                uv.Add(new Vector2(glyph.uMax, glyph.vMax));
                uv.Add(new Vector2(glyph.uMax, glyph.vMin));
                
                
                triangles.Add(0 + offset);
                triangles.Add(1 + offset);
                triangles.Add(2 + offset);
                
                triangles.Add(0 + offset);
                triangles.Add(2 + offset);
                triangles.Add(3 + offset);
                
//                x += gBounds.width + letterSpacing;
            } else {
//                x += gBounds.width; // no letter spacing for blank characters
            }
            
            x += xAdvance;
        }
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