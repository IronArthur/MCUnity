Shader "MechCommanderUnity/PaletteSwapLookup"
{
	Properties
	{
		_MainTex ("Main Ind. Text.", 2D) = "white" {}
		_PaletteTex("Palette Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent"  "PreviewType"="Plane"}

		Cull Off 
		ZWrite Off 
		Lighting Off
		ZTest Off
		Blend SrcAlpha OneMinusSrcAlpha


		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float4 _MainTex_ST;
            float4 _PaletteTex_ST;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;//float2(v.uv.x ,1 - v.uv.y);
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _PaletteTex;

			fixed4 frag (v2f i) : SV_Target
			{
				float x = tex2D(_MainTex, i.uv).a;
				return tex2D(_PaletteTex, float2(x, 0));
			}

			ENDCG
		}
	}
}
