﻿Shader "Custom/Lava" {
     
     Properties {
        _MainTex ("Main Texture", 2D) = "white" {}
        _TextureDistort("Texture Wobble", range(0,1)) = 0.1
        _NoiseTex("Extra Wave Noise", 2D) = "white" {}
        _Speed("Wave Speed", Range(0,1)) = 0.5
        _Amount("Wave Amount", Range(0,1)) = 0.6
        _Scale("Scale", Range(0,1)) = 0.5
        _Height("Wave Height", Range(0,1)) = 0.1
    }
    
    SubShader {
        Tags { "RenderType"="Opaque"  "Queue" = "Transparent" }
        LOD 100
        Blend OneMinusDstColor One
        Cull Off
       
        GrabPass{
            Name "BASE"
            Tags{ "LightMode" = "Always" }
        }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
                 

#include "UnityCG.cginc"
 
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f {
                float2 uv : TEXCOORD3;
                float4 vertex : SV_POSITION;
                float4 scrPos : TEXCOORD2;
                float4 worldPos : TEXCOORD4;
            };
            
            float _TextureDistort;
            sampler2D _MainTex, _NoiseTex;
            float4 _MainTex_ST;
            float _Speed, _Amount, _Height, _Scale;
 
            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                float4 tex = tex2Dlod(_NoiseTex, float4(v.uv.xy, 0, 0));//extra noise tex
                v.vertex.y += sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount * tex)) * _Height;//movement
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
               
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.scrPos = ComputeScreenPos(o.vertex);
                
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target {
                fixed distortx = tex2D(_NoiseTex, (i.worldPos.xz * _Scale)  + (_Time.x * 0.2)).r ;// distortion
                half4 col = tex2D(_MainTex, (i.worldPos.xz * _Scale) - (distortx * _TextureDistort));// texture times tint;       
                
                return col;
            }
            ENDCG
        }
    }
}
