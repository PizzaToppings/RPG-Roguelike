Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _OutlineThickness ("Outline Thickness", Range(0, 10)) = 1.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"           = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType"      = "Transparent"
            "PreviewType"     = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4    _MainTex_TexelSize;
            fixed4    _Color;
            fixed4    _OutlineColor;
            float     _OutlineThickness;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.vertex   = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                OUT.color    = v.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, IN.texcoord);
                fixed4 col      = texColor * IN.color;

                // Sample 8 neighbours scaled by thickness to detect the sprite edge
                float2 step = _MainTex_TexelSize.xy * _OutlineThickness;

                float neighborAlpha =
                    tex2D(_MainTex, IN.texcoord + float2( step.x,  0)).a +
                    tex2D(_MainTex, IN.texcoord + float2(-step.x,  0)).a +
                    tex2D(_MainTex, IN.texcoord + float2( 0,  step.y)).a +
                    tex2D(_MainTex, IN.texcoord + float2( 0, -step.y)).a +
                    tex2D(_MainTex, IN.texcoord + float2( step.x,  step.y)).a +
                    tex2D(_MainTex, IN.texcoord + float2(-step.x,  step.y)).a +
                    tex2D(_MainTex, IN.texcoord + float2( step.x, -step.y)).a +
                    tex2D(_MainTex, IN.texcoord + float2(-step.x, -step.y)).a;

                // If current pixel is transparent but a neighbour is opaque ? outline pixel
                if (col.a < 0.01 && neighborAlpha > 0.0)
                {
                    col = _OutlineColor;
                }

                return col;
            }
            ENDCG
        }
    }
}
