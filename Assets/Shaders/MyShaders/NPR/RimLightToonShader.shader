// NOTE：加了边缘光的卡通着色器
// 取消了普通的高光，加了模拟菲涅尔反射折射的边缘光
// NOTE：未考虑多光源，不可用于复杂场景渲染

Shader "MyShaders/RimLightToonShader"{
	Properties{
		_Color("Color Tint", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white"{}
		_Ramp("Ramp Texture", 2D) = "white"{}
		_Outline("Outline", Range(0,1)) = 0.1
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Specular("Specular Color", Color) = (1,1,1,1)
		_SpecularScale("Specular Scale", Range(0,0.1)) = 0.01
		_RimColor("Rim Light Color", Color) = (1,1,1,1)
		_ReflectionRate("Reflection Rate", Range(0,20)) = 0.3
		_ReflectionOn("Reflection on(0 or 1)", Int) = 0
		_RefractionRate("Refraction Rate", Range(0,50)) = 0.3
		_RefractionOn("Refraction on(0 or 1)", Int) = 0
	}

	SubShader{
		Tags{"RenderType" = "Opaque" "Queue" = "Geometry"}

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
			sampler2D _Ramp;
			fixed4 _Specular;
			fixed _SpecularScale;
			fixed _ReflectionRate;
			fixed _RefractionRate;
			fixed4 _RimColor;
			half _ReflectionOn;
			half _RefractionOn;
		
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

				// 考虑了衰减和阴影的半兰伯特值，用于 ramp texture 的采样
				fixed halfLambert = (0.5 * dot(worldNormal, worldLightDir) + 0.5) * atten;
				// 应该在 ramp 采样之前 *atten。如果还像传统光照那样在最后 *atten，会破坏采样的梯度化结果，
				// 黑乎乎的阴影部分会影响整个的卡通效果。

				fixed3 diffuse = _LightColor0.rgb * albedo * tex2D(_Ramp, float2(halfLambert, halfLambert)).rgb;

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

				// 计算边缘光
				float adotNV = abs(dot(worldNormal, worldViewDir));
				float Fr = pow(1-adotNV, _ReflectionRate);
				float Ft = pow(adotNV, _RefractionRate);

				//specular += (_RimColor.rgb * (Fr+Ft));
				fixed3 rim = saturate(_RimColor.rgb * (Fr*_ReflectionOn + Ft * _RefractionOn));

				return fixed4(ambient + diffuse + (specular+rim)*atten, 1.0);
			}


			ENDCG
		}

	}
	FallBack "Diffuse"
}