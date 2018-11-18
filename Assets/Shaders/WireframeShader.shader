Shader "MyShaders/WireframeShader"
{
	Properties
	{
		_LineColor ("Line color", Color) = (0, 0, 0, 1)
		_Color("Color Tint", Color) = (1,1,1,1)
	    _LineWidth ("Line size", Range(0.0001, 0.15)) = 3.0
	    _IsWireframe ("Render wireframe", Range(0.00, 1.00)) = 1
	    _Bound("Upper/lower bound", Float) = 5.0
	}
	SubShader
	{
		LOD 100

		Pass
		{
			Tags {"LightMode"="ForwardBase"}

			CGPROGRAM
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#pragma vertex vert
			#pragma fragment frag
			

			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};

			fixed4 _LineColor;
			float _LineWidth;
			fixed4 _Color;
			fixed _IsWireframe;
			float _Bound;
			
			v2f vert (a2v v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = float4(v.uv, v.uv1);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.normal = v.normal;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed beyond_upper = step(_Bound, i.worldPos.y);
				fixed beyond_lower = step(i.worldPos.y, -_Bound);
				fixed beyond = beyond_upper + beyond_lower;
				fixed alpha = step(beyond, 0.5);
				clip(alpha - 0.5);

				fixed3 lightDir = UnityWorldSpaceLightDir(i.worldPos);
				fixed3 normalDir = normalize(i.normal);

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * _Color;
				fixed diff = saturate(dot(normalDir, lightDir));
				fixed3 diffuse = _LightColor0.rgb * _Color.rgb * diff;

				float d = min(i.uv.x, min(i.uv.y, i.uv.z));
				fixed3 wired_color = (1-step(_LineWidth, d)) * _LineColor.xyz + step(_LineWidth, d)*diffuse;

				fixed3 final_color = wired_color * _IsWireframe + diffuse * (1 - _IsWireframe);

				return fixed4(final_color + ambient, alpha);
			}
			ENDCG
		}
	} Fallback "Diffuse"
}
