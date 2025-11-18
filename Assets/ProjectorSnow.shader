Shader "Projector/SnowDecal_Fix" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _ShadowTex ("Cookie", 2D) = "gray" {} // Kéo ảnh vết tuyết vào đây trong Material
    }
    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Pass {
            ZWrite Off
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha // Chế độ hòa trộn chuẩn cho trong suốt
            Offset -1, -1 // Giúp hình nằm đè lên trên tường mà không bị nhấp nháy

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
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Tính toán toạ độ chiếu
                float4 uv = UNITY_PROJ_COORD(i.uvShadow);
                
                // Lấy màu từ texture
                fixed4 tex = tex2Dproj (_ShadowTex, uv);

                // QUAN TRỌNG: Cắt bỏ những phần chiếu ngược (back projection) hoặc ngoài tầm
                // Nếu không có dòng này, hình có thể bị lặp lại hoặc nhòe
                if (uv.w < 0) discard; 

                // Áp dụng màu và độ trong suốt
                return tex * _Color;
            }
            ENDCG
        }
    }
}