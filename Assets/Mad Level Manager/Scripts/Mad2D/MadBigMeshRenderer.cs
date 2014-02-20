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

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]   
public class MadBigMeshRenderer : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    MadPanel panel;
    
    // helpers for decrasing GC activity
    MadList<Vector3> vertices = new MadList<Vector3>();
    MadList<Color32> colors = new MadList<Color32>();
    MadList<Vector2> uv = new MadList<Vector2>();
    MadList<MadList<int>> triangleList = new MadList<MadList<int>>();

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    void Start() {
        panel = GetComponent<MadPanel>();
        
        var meshFilter = transform.GetComponent<MeshFilter>();
        var mesh = meshFilter.sharedMesh;
        if (mesh == null) {
            mesh = new Mesh();
            mesh.name = "Generated Mesh";
            meshFilter.sharedMesh = mesh;
        }
#if !UNITY_3_5
        mesh.MarkDynamic();
#endif

#if UNITY_4_2
        if (Application.unityVersion.StartsWith("4.2.0")) {
            Debug.LogError("Unity 4.2 comes with terrible bug that breaks down Mad Level Manager rendering process. "
                + "Please upgrade/downgrade to different version. http://forum.unity3d.com/threads/192467-Unity-4-2-submesh-draw-order");
            }
#endif
    }

    void LateUpdate() {
        var meshFilter = transform.GetComponent<MeshFilter>();
        var mesh = meshFilter.sharedMesh;
        mesh.Clear();
        
        var sprites = VisibleSprites(panel.sprites);
        SortByGUIDepth(sprites);
        
        Material[] materials = new Material[sprites.Count];
        
        mesh.subMeshCount = sprites.Count;
        
        for (int i = 0; i < sprites.Count; ++i) {
            var sprite = sprites[i];
            Material material;
            MadList<int> triangles = new MadList<int>();
            
            sprite.DrawOn(ref vertices, ref colors, ref uv, ref triangles, out material);
            
            triangles.Trim();
            triangleList.Add(triangles);
            materials[i] = material;
        }
        
        vertices.Trim();
        colors.Trim();
        uv.Trim();
        triangleList.Trim();
        
        mesh.vertices = vertices.Array;
        mesh.colors32 = colors.Array;
        mesh.uv = uv.Array;
        mesh.RecalculateNormals();
        
        for (int i = 0; i < triangleList.Count; ++i) {
            MadList<int> triangles = triangleList[i];
            mesh.SetTriangles(triangles.Array, i);
        }
        
        renderer.sharedMaterials = materials;
        
        vertices.Clear();
        colors.Clear();
        uv.Clear();
        triangleList.Clear();
    }
    
    List<MadSprite> VisibleSprites(ICollection<MadSprite> sprites) {
        List<MadSprite> output = new List<MadSprite>();
        
        foreach (var sprite in sprites) {
            bool active = 
#if UNITY_3_5
        sprite.gameObject.active;
#else
        sprite.gameObject.activeInHierarchy;
#endif
            if (active && sprite.visible && sprite.tint.a != 0 && sprite.CanDraw()) {
                output.Add(sprite);
            }
        }
        
        return output;
    }
    
    void SortByGUIDepth(List<MadSprite> sprites) {
        sprites.Sort((x, y) => x.guiDepth.CompareTo(y.guiDepth));
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