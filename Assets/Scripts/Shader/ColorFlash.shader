Shader "Custom/ColorFlash" {
     Properties
     {
         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
         _Color ("Tint", Color) = (1,1,1,1)
         _FlashColor ("Flash Color", Color) = (1,1,1,1)
         _FlashAmount ("Flash Amount",Range(0.0,1.0)) = 0.0
         [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

         // outline
         _Outline ("Outline", Range(0.0,1.0)) = 0.0
         _OutlineColor("Outline Color", Color) = (1,1,1,1)

     }
 
     SubShader
     {
         Tags
         { 
             "Queue"="Transparent" 
             "IgnoreProjector"="True" 
             "RenderType"="Transparent" 
             "PreviewType"="Plane"
             "CanUseSpriteAtlas"="True"
         }
 
         Cull Off
         Lighting Off
         ZWrite Off
         Fog { Mode Off }
         Blend One OneMinusSrcAlpha
 
         Pass
         {
         CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile DUMMY PIXELSNAP_ON
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
                 half2 texcoord  : TEXCOORD0;
             };
             
             fixed4 _Color;
             fixed4 _FlashColor;
             float _FlashAmount;
             float _Outline;
             fixed4 _OutlineColor;
 
             v2f vert(appdata_t IN)
             {
                 v2f OUT;
                 OUT.vertex = UnityObjectToClipPos(IN.vertex);
                 OUT.texcoord = IN.texcoord;
                 OUT.color = IN.color * _Color;
                 #ifdef PIXELSNAP_ON
                 OUT.vertex = UnityPixelSnap (OUT.vertex);
                 #endif
 
                 return OUT;
             }
 
             sampler2D _MainTex;
             float4 _MainTex_TexelSize;
 
             fixed4 frag(v2f IN) : COLOR
             {
                 fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                 c.rgb = lerp(c.rgb,_FlashColor.rgb,_FlashAmount);

                 if(_Outline > 0 && c.a == 0) {
                    fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, _MainTex_TexelSize.y));
                    fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0, _MainTex_TexelSize.y));
                    fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(_MainTex_TexelSize.x, 0));
                    fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(_MainTex_TexelSize.x, 0));

                    // If one of the neighbouring pixels is invisible, we render an outline
                    // if(pixelUp.a * pixelDown.a * pixelRight.a * pixelLeft.a == 0) {
                    //     c.rgba = fixed4(1,1,1,1) * _OutlineColor;
                    // }
                    
                    // If one of the neighbouring pixels is visible, we render an outline
                    if(pixelUp.a + pixelDown.a + pixelRight.a + pixelLeft.a >= 1) {
                        c.rgba = fixed4(1,1,1,1) * _OutlineColor;
                    }
                 }

                 c.rgb *= c.a;
             
                 return c;
             }
         ENDCG
         }
     }
 }