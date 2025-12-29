Shader "Custom/IceWithZWrite"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0.5) // Màu băng (Alpha = 0.5 là trong suốt một nửa)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.9
        _Metallic ("Metallic", Range(0,1)) = 0.1
    }
    SubShader
    {
        // Queue 3000 là Transparent, nhưng ta render sớm hơn 1 chút để Projector bắt kịp
        Tags { "Queue"="Transparent-1" "RenderType"="Transparent" }
        LOD 200

        // --- ĐÂY LÀ DÒNG QUAN TRỌNG NHẤT ---
        // Bắt buộc ghi độ sâu dù đang trong suốt
        ZWrite On 
        // -----------------------------------

        // Chế độ hòa trộn để làm trong suốt
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a; // Sử dụng Alpha từ màu hoặc texture
        }
        ENDCG
    }
    FallBack "Standard"
}