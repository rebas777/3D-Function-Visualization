// NOTE: Tone Based Shader 基于色调的卡通着色器

Shader "MyShaders/Tone Based Shader"{
	Properties{
		_Color ("Diffuse Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Outline ("Outline", Range(0,1)) = 0.1
		_OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
		_Specular ("Specular", Color) = (1, 1, 1, 1)
		_SpecularScale("Specular Scale", Range(0,0.1)) = 0.01
		_Blue ("Blue", Range(0, 1)) = 0.5
		_Alpha ("Alpha", Range(0, 1)) = 0.5
		_Yellow ("Yellow", Range(0, 1)) = 0.5
		_Beta ("Beta", Range(0, 1)) = 0.5
	}

	SubShader{
		Tags{"RenderType" = "Opaque"}

		// 调用泛性卡通着色器中的勾边 Pass
		UsePass "MyShaders/ GeneralToonShader /OUTLINE"

		Pass{
			Tags{"LightMode" = "ForwardBase"}

			Cull Back 

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile_fwdbase
		
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Specular;
			fixed _SpecularScale;
			fixed _Blue;
			fixed _Yellow;
			fixed _Alpha;
			fixed _Beta;
		
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
			}; 
		
			struct v2f {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				SHADOW_COORDS(3)
			};

			v2f vert(a2v v){
				v2f o;
				
				o.pos = UnityObjectToClipPos( v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				o.worldNormal  = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				TRANSFER_SHADOW(o);
				
				return o;
			}

			float4 frag(v2f i) : SV_Target{
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 worldHalfDir = normalize(worldLightDir + worldViewDir);

				fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				// 计算考虑光照衰减和阴影的半兰伯特值
				fixed diff = dot(worldNormal, worldLightDir);
				diff = (diff*0.5 + 0.5)*atten;

				fixed3 k_blue = fixed3(0, 0, _Blue);
				fixed3 k_yellow = fixed3(_Yellow, _Yellow, 0);
				fixed3 k_cool = k_blue + _Alpha * albedo;
				fixed3 k_warm = k_yellow + _Beta * albedo;

				fixed3 diffuse = _LightColor0.rgb * (diff * k_warm + (1-diff)*k_cool);

				// 与传统高光计算不同，传统为：
				// fixed spec = pow(saturate(dot(normal, halfDir)), _Gloss);
				fixed spec = dot(worldNormal, worldHalfDir);
				// fwidth 计算窗口空间的近似倒数值的和
				// 总之这里 w 是一个很小的值
				fixed w = fwidth(spec) * 2.0;

				/*
				* smoothstep 函数：
				* 	当第三个参数小于 -w 时返回0；大于 w 时返回1； 否则在 01之间差值
				*   这里起到高光边缘抗锯齿的作用
				*	
				* lerp 函数：
				* 	以第三个参数作为权重，在第一个参数和第二个参数之间差值
				*	lerp(a,b,w) --- return a + w*(b-a);
				*
				* step 函数：
				* 	step(thre, val) --- return (val > thre) ? 1 : 0;
				*
				* 最后一项的作用：在 _SpecularScale 很小的时候完全消除高光。
				*/
				fixed3 specular = _Specular.rgb *
					lerp(0, 1, smoothstep(-w, w, spec + _SpecularScale - 1)) *
					step(0.0001, _SpecularScale);

				return fixed4(ambient + diffuse + specular*atten, 1.0);
			}


			ENDCG
		}

	}
	FallBack "Diffuse"

}