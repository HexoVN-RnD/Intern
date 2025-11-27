Shader "Projector/SnowDecal_FinalFix" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _ShadowTex ("Cookie", 2D) = "gray" {}
        _BorderCutoff ("Border Cutoff", Range(0, 0.2)) = 0.05 // Khoảng cắt bỏ viền
    }
    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Pass {
            ZWrite Off
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            Offset -1, -1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct v2f {
                float4 uvShadow : TEXCOORD0;
                float4 pos : SV_POSITION;
            };
            
            float4x4 unity_Projector;
            
            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.uvShadow = mul (unity_Projector, vertex);
                return o;
            }
            
            sampler2D _ShadowTex;
            fixed4 _Color;
            float _BorderCutoff;
            
            fixed4 frag (v2f i) : SV_Target
            {
                float4 uv = UNITY_PROJ_COORD(i.uvShadow);
                
                // --- LOGIC CẮT VIỀN (BORDER CUTOFF) ---
                // Nếu UV nằm sát mép (trong khoảng 0 -> 0.05 hoặc 0.95 -> 1), ta vứt bỏ luôn.
                // Điều này triệt tiêu hoàn toàn các vệt sọc ở rìa.
                if (uv.x < _BorderCutoff || uv.x > (1.0 - _BorderCutoff) || 
                    uv.y < _BorderCutoff || uv.y > (1.0 - _BorderCutoff)) 
                {
                    discard; // Hoặc return fixed4(0,0,0,0);
                }

                fixed4 tex = tex2Dproj (_ShadowTex, uv);
                
                // Loại bỏ phần chiếu ngược
                if (uv.w < 0) discard;

                return tex * _Color;
            }
            ENDCG
        }
    }
}