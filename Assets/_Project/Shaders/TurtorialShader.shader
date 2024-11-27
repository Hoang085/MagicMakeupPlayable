Shader "Unlit/TutorialShader"
{
    Properties
    {
        _MainTexure ("Main Texture", 2D) = "white" {}
        _AnimateXY("Animate XY", Vector)=(0,0,0,0)
        _Color("Color", Color)=(1,1,1,1)
        _ScaleXTexture("_ScaleTexture", Range(0,1000)) = 3
        _BackgroundAlpha("Background Alpha", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
        LOD 100

        Pass
        {
            Cull Off // Cho phép hiển thị cả hai mặt
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off // Disable writing to depth buffer for transparency sorting
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTexure;
            float4 _MainTexure_ST;
            float4 _AnimateXY;
            float4 _Color;
            float _ScaleXTexture;
            float _BackgroundAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTexure);
                o.uv.xy += frac(_AnimateXY.xy * _MainTexure_ST.xy * _Time.yy);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uvs = i.uv;
                uvs.x *= (int)_ScaleXTexture;
                fixed4 textureColor = tex2D(_MainTexure, uvs);
                
                // If the alpha is zero, apply the background color
                if (textureColor.a == 0)
                {
                    textureColor = _Color;
                    textureColor.a *= _BackgroundAlpha; // Adjust the alpha for the background
                }

                return textureColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
