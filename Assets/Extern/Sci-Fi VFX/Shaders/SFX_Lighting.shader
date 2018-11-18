// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "QFX/SFX/Lighting"
{
	Properties
	{
		[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_DisplaceTexture("Displace Texture", 2D) = "white" {}
		_Scale("Scale", Range( 0.1 , 4)) = 0
		_Displace("Displace", Range( 0 , 1)) = 0
		_SpeedXY("SpeedXY", Vector) = (0,0,0,0)
	}

	Category 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane"  }

		
		SubShader
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off

			Pass {
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_OUTPUT_STEREO
				};
				
				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform sampler2D_float _CameraDepthTexture;
				uniform float _InvFade;
				uniform sampler2D _DisplaceTexture;
				uniform float4 _DisplaceTexture_ST;
				uniform float2 _SpeedXY;
				uniform float _Scale;
				uniform float _Displace;

				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					o.texcoord.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float2 uv_DisplaceTexture = i.texcoord * _DisplaceTexture_ST.xy + _DisplaceTexture_ST.zw;
					float2 uv1 = i.texcoord * float2( 1,1 ) + float2( 0,0 );
					float2 panner3 = ( uv1 + 1.0 * _Time.y * _SpeedXY);
					float4 temp_cast_1 = (1.0).xxxx;
					

					fixed4 col = ( _TintColor * tex2D( _MainTex, ( float3( uv_DisplaceTexture ,  0.0 ) + ( (( ( tex2D( _DisplaceTexture, (panner3*_Scale + 0.0) ) * 2.0 ) - temp_cast_1 )).rgb * _Displace ) ).xy ) );
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
7;525;1904;415;1627.709;116.6039;1.3;True;False
Node;AmplifyShaderEditor.Vector2Node;2;-2910.834,-340.0287;Float;False;Property;_SpeedXY;SpeedXY;4;0;Create;0,0;-1,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-2913.792,-464.9567;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;21;-2654.393,-219.7431;Float;False;Property;_Scale;Scale;2;0;Create;0;1;0.1;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;3;-2687.201,-356.8866;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;20;-2471.393,-356.7431;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;5;-2218.239,-385.7227;Float;True;Property;_DisplaceTexture;Displace Texture;1;0;Create;None;d784595d7b8bfef41ac0a5bd8fa0a662;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-1916.725,-196.2036;Float;False;Constant;_Float0;Float 0;4;0;Create;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1704.891,-116.9567;Float;False;Constant;_Float1;Float 1;4;0;Create;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-1727.751,-376.0347;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;8;-1560.113,-226.6836;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1530.959,133.3445;Float;False;Property;_Displace;Displace;3;0;Create;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;9;-1410.5,-2.116616;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-1242.297,-222.3935;Float;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1184.315,73.27937;Float;False;2;2;0;FLOAT3;0,0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;13;-941.4445,-74.67168;Float;False;_MainTex;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-1003.792,52.05232;Float;False;2;2;0;FLOAT2;0.0,0,0;False;1;FLOAT3;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;16;-699.9584,26.5843;Float;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;19;-634.2416,-154.7099;Float;False;_TintColor;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-294.6876,9.789632;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMasterNode;42;-118.1734,7.582417;Float;False;True;2;Float;ASEMaterialInspector;0;9;QFX/SFX/Lighting;134b5ded8c979ab45b6db51381fd0d69;QFX/SFX/Templates/Particles Alpha Blended HDR;2;SrcAlpha;OneMinusSrcAlpha;0;One;Zero;Off;True;True;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;3;0;1;0
WireConnection;3;2;2;0
WireConnection;20;0;3;0
WireConnection;20;1;21;0
WireConnection;5;1;20;0
WireConnection;6;0;5;0
WireConnection;6;1;4;0
WireConnection;8;0;6;0
WireConnection;8;1;7;0
WireConnection;9;0;8;0
WireConnection;11;0;9;0
WireConnection;11;1;10;0
WireConnection;14;0;12;0
WireConnection;14;1;11;0
WireConnection;16;0;13;0
WireConnection;16;1;14;0
WireConnection;41;0;19;0
WireConnection;41;1;16;0
WireConnection;42;0;41;0
ASEEND*/
//CHKSM=010C25933FBCBC7E8D4E1E79976EF56212F79918