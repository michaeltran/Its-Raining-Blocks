Shader "Tools/Mad Level Manager/Unlit/Font" {
    Properties {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }

    SubShader {
        Tags {
            "Queue"="Overlay"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        LOD 100
 
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 
        Cull Off
        Lighting Off
        ColorMaterial AmbientAndDiffuse
        
        Pass {
            SetTexture [_MainTex] {
                combine texture * primary
            }
        }
    }
}
