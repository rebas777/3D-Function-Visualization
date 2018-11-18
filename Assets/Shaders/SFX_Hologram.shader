// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "QFX/SFX/Hologram"
{
	Properties
	{
		_MainColor("Main Color", Color) = (0,0,0,0)
		_AlbedoTexture("Albedo Texture", 2D) = "white" {}
		_MetallicTexture("Metallic Texture", 2D) = "white" {}
		_NormalTexture("Normal Texture", 2D) = "bump" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_EmissionColor("Emission Color", Color) = (0,0,0,0)
		_EmissionTexture("Emission Texture", 2D) = "white" {}
		[HDR]_RimColor("Rim Color", Color) = (0,0,0,0)
		_Rim("Rim", Range( 0 , 10)) = 1
		_GlowTiling("Glow Tiling", Range( 0 , 2)) = 2
		_GlowSpeed("Glow Speed", Vector) = (0,0,0,0)
		_Scan("Scan", Range( 0 , 1)) = 0.7926539
		_ScanTiling("Scan Tiling", Range( 0 , 100)) = 3.60764
		_ScanSpeed("Scan Speed", Vector) = (0,0,0,0)
		_FlickerTexture("Flicker Texture", 2D) = "white" {}
		_FlickerSpeed("Flicker Speed", Vector) = (0.1,0.1,0,0)
		_Glitch("Glitch", Range( 0.1 , 0.99)) = 0
		_GlitchOffset("Glitch Offset", Vector) = (0,0,0,0)
		_GlitchSpeed("Glitch Speed", Range( 0.1 , 1)) = 2
		_GlitchInterval("Glitch Interval", Range( 0.1 , 1)) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _NormalTexture;
		uniform float4 _NormalTexture_ST;
		uniform float4 _MainColor;
		uniform sampler2D _AlbedoTexture;
		uniform float4 _AlbedoTexture_ST;
		uniform sampler2D _EmissionTexture;
		uniform float4 _EmissionTexture_ST;
		uniform float4 _EmissionColor;
		uniform float _Rim;
		uniform float4 _RimColor;
		uniform sampler2D _MetallicTexture;
		uniform float4 _MetallicTexture_ST;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _ScanTiling;
		uniform float2 _ScanSpeed;
		uniform float _Scan;
		uniform float _GlowTiling;
		uniform float2 _GlowSpeed;
		uniform sampler2D _FlickerTexture;
		uniform float2 _FlickerSpeed;
		uniform float3 _GlitchOffset;
		uniform float _Glitch;
		uniform float _GlitchSpeed;
		uniform float _GlitchInterval;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float temp_output_100_0 = ( step( _Glitch , sin( ( ( 20.0 * _GlitchSpeed * _Time.y ) + ase_vertex3Pos.y ) ) ) * step( 0.8 , sin( ( _Time.y * _GlitchInterval * 20.0 ) ) ) );
			float4 appendResult92 = (float4(( _GlitchOffset.x * temp_output_100_0 ) , ( _GlitchOffset.y * temp_output_100_0 ) , ( _GlitchOffset.z * temp_output_100_0 ) , 0.0));
			float4 VertexOffset87 = appendResult92;
			v.vertex.xyz += VertexOffset87.xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalTexture = i.uv_texcoord * _NormalTexture_ST.xy + _NormalTexture_ST.zw;
			float3 Normal140 = UnpackNormal( tex2D( _NormalTexture, uv_NormalTexture ) );
			o.Normal = Normal140;
			float2 uv_AlbedoTexture = i.uv_texcoord * _AlbedoTexture_ST.xy + _AlbedoTexture_ST.zw;
			float4 tex2DNode23 = tex2D( _AlbedoTexture, uv_AlbedoTexture );
			float4 Main12 = ( _MainColor * tex2DNode23 );
			o.Albedo = Main12.rgb;
			float2 uv_EmissionTexture = i.uv_texcoord * _EmissionTexture_ST.xy + _EmissionTexture_ST.zw;
			float4 Emission134 = ( tex2D( _EmissionTexture, uv_EmissionTexture ) * _EmissionColor );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNDotV6 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode6 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNDotV6, (10.0 + (_Rim - 0.0) * (0.0 - 10.0) / (10.0 - 0.0)) ) );
			float4 Rim8 = ( fresnelNode6 * _RimColor * Main12 );
			o.Emission = ( Emission134 + Rim8 ).rgb;
			float2 uv_MetallicTexture = i.uv_texcoord * _MetallicTexture_ST.xy + _MetallicTexture_ST.zw;
			float4 tex2DNode142 = tex2D( _MetallicTexture, uv_MetallicTexture );
			float Metallic147 = ( tex2DNode142.r * _Metallic );
			o.Metallic = Metallic147;
			float Smoothness148 = ( tex2DNode142.a * _Smoothness );
			o.Smoothness = Smoothness148;
			float MainAlpha36 = tex2DNode23.a;
			float2 temp_cast_2 = (_Scan).xx;
			float2 Scan35 = step( frac( ( ( _ScanTiling * ase_worldPos.y ) + ( _Time.y * _ScanSpeed ) ) ) , temp_cast_2 );
			float2 Glow57 = frac( ( ( _GlowTiling * ase_worldPos.y ) + ( _Time.y * _GlowSpeed ) ) );
			float4 Flicker72 = tex2D( _FlickerTexture, ( _Time.y * _FlickerSpeed ) );
			float4 Opacity149 = ( MainAlpha36 * ( float4( Scan35, 0.0 , 0.0 ) + Rim8 + float4( Glow57, 0.0 , 0.0 ) ) * Flicker72 );
			o.Alpha = Opacity149.r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
7;646;1904;387;1699.076;666.2806;1.6;True;False
Node;AmplifyShaderEditor.CommentaryNode;45;-2141.752,780.23;Float;False;1394.875;636.1515;Comment;11;35;34;33;43;31;32;30;26;29;28;24;Scan;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;74;-1075.196,-964.8503;Float;False;2354.055;926.0765;Comment;22;87;92;100;95;101;103;102;99;106;84;98;105;126;107;128;155;156;157;158;160;161;162;Glitch;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;26;-2090.812,1262.264;Float;False;Property;_ScanSpeed;Scan Speed;14;0;Create;0,0;3,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;15;-2115.967,-582.6549;Float;False;885.4144;508.7863;Comment;5;12;11;23;36;130;Main;1,1,1,1;0;0
Node;AmplifyShaderEditor.TimeNode;29;-2097.094,1102.784;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;48;-2152.703,1617.931;Float;False;1200.045;638.7216;Comment;9;57;63;54;55;52;49;51;53;64;Glow;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1;-2134.607,109.9991;Float;False;1334.6;502.9746;Comment;7;7;6;5;2;8;4;47;Rim;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;28;-2106.968,933.6244;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TimeNode;98;-1015.927,-646.6279;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-2100.911,849.0348;Float;False;Property;_ScanTiling;Scan Tiling;13;0;Create;3.60764;51;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;128;-1069.865,-739.6171;Float;False;Property;_GlitchSpeed;Glitch Speed;19;0;Create;2;0.156;0.1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-943.4568,-831.3733;Float;False;Constant;_Float1;Float 1;21;0;Create;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1807.627,1209.975;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PosVertexDataNode;84;-1004.904,-502.877;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;51;-2106.805,1702.965;Float;False;Property;_GlowTiling;Glow Tiling;10;0;Create;2;0.5;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-2113.697,240.7058;Float;False;Property;_Rim;Rim;9;0;Create;1;9.4;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1788.285,954.0509;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-753.006,-757.021;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;130;-1840.415,-505.9869;Float;False;Property;_MainColor;Main Color;0;0;Create;0,0,0,0;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;52;-2102.987,1956.714;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;53;-2109.534,1788.551;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;49;-2065.633,2105.956;Float;False;Property;_GlowSpeed;Glow Speed;11;0;Create;0,0;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;23;-1895.559,-285.5937;Float;True;Property;_AlbedoTexture;Albedo Texture;1;0;Create;None;ed6b753151e5e5643a33a75a40b5bdc6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;105;-980.8676,-339.9866;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;107;-1038.564,-192.2296;Float;False;Property;_GlitchInterval;Glitch Interval;20;0;Create;2;1;0.1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-914.2263,-115.8531;Float;False;Constant;_Float0;Float 0;21;0;Create;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-1786.865,1745.778;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1596.066,-383.7047;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;99;-607.2783,-577.6461;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-1545.831,1024.263;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-722.4578,-210.463;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-1810.751,2047.7;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;47;-1795.848,247.0753;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;10.0;False;3;FLOAT;10.0;False;4;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;67;-829.0516,1619.136;Float;False;1145.839;404.78;Comment;5;72;70;68;69;71;Flicker;1,1,1,1;0;0
Node;AmplifyShaderEditor.TimeNode;70;-798.8917,1693.555;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;4;-1478.319,514.926;Float;False;12;0;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;69;-778.4207,1859.287;Float;False;Property;_FlickerSpeed;Flicker Speed;16;0;Create;0.1,0.1;0.5,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;5;-1507.889,331.438;Float;False;Property;_RimColor;Rim Color;8;1;[HDR];Create;0,0,0,0;0.3529412,0.6456387,1.5,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;6;-1542.917,175.7199;Float;False;World;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;64;-1581.202,1887.766;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;103;-544.5494,-285.2672;Float;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;102;-442.7641,-578.0757;Float;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1438.044,-388.2923;Float;False;Main;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FractNode;33;-1367.731,1024.481;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-366.9243,-739.6544;Float;False;Property;_Glitch;Glitch;17;0;Create;0;0.99;0.1;0.99;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-1536.979,1291.401;Float;False;Property;_Scan;Scan;12;0;Create;0.7926539;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;34;-1199.446,1022.354;Float;True;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1177.273,281.091;Float;False;3;3;0;FLOAT;0.0;False;1;COLOR;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FractNode;63;-1390.663,1887.032;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StepOpNode;101;-292.7599,-307.047;Float;True;2;0;FLOAT;0.8;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-523.2333,1784.243;Float;True;2;2;0;FLOAT;0,0;False;1;FLOAT2;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StepOpNode;95;-192.6005,-600.9301;Float;True;2;0;FLOAT;0.99;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;1022.896,643.9156;Float;False;8;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;61;1021.492,728.6387;Float;False;57;0;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;131;-602.6545,136.4026;Float;True;Property;_EmissionTexture;Emission Texture;7;0;Create;None;4e163e9238b36af4c8e5bd22e1dad895;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;35;-963.8255,1015.454;Float;False;Scan;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;90.75124,-466.1025;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;155;127.5105,-658.1254;Float;False;Property;_GlitchOffset;Glitch Offset;18;0;Create;0,0,0;0.2,0,-0.1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-1008.045,275.7874;Float;False;Rim;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;68;-265.6546,1756.335;Float;True;Property;_FlickerTexture;Flicker Texture;15;0;Create;None;e80c3c84ea861404d8a427db8b7abf04;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;38;1020.135,563.1547;Float;False;35;0;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;132;-518.8383,337.3003;Float;False;Property;_EmissionColor;Emission Color;6;0;Create;0,0,0,0;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;57;-1172.846,1881.415;Float;False;Glow;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;143;810.9833,277.8345;Float;False;Property;_Metallic;Metallic;4;0;Create;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-1443.186,-192.0193;Float;False;MainAlpha;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;72;97.07669,1757.098;Float;False;Flicker;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;1032.84,478.5265;Float;False;36;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;811.4882,352.135;Float;False;Property;_Smoothness;Smoothness;5;0;Create;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;142;784.5161,76.86777;Float;True;Property;_MetallicTexture;Metallic Texture;2;0;Create;None;bc440fe34a95b51478185a495ae8653a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;-262.366,266.4832;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;59;1258.155,621.3296;Float;False;3;3;0;FLOAT2;0,0;False;1;COLOR;0,0;False;2;FLOAT2;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;1035.201,850.3755;Float;False;72;0;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;545.8097,-378.0251;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;544.7098,-489.0251;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;156;545.7097,-590.0252;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;1148.464,283.6125;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;1442.684,599.9172;Float;False;3;3;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;1147.409,131.0074;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;92;797.3001,-509.4266;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;203.3283,942.8617;Float;False;134;0;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;139;196.2389,117.2058;Float;True;Property;_NormalTexture;Normal Texture;3;0;Create;None;d1c23eaf3bfb52944a2684af48083f58;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;-84.36646,260.7414;Float;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;201.1062,1030.071;Float;False;8;0;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;136;436.8448,966.514;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;147;1356.715,128.1329;Float;False;Metallic;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;480.5459,1499.011;Float;False;87;0;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;150;577.6583,1279.102;Float;False;149;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;17;423.9306,636.1476;Float;False;12;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;87;979.7853,-515.6304;Float;False;VertexOffset;-1;True;1;0;FLOAT4;0.0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;152;380.1793,1205.334;Float;False;148;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;140;505.5978,115.9733;Float;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;148;1344.963,279.6042;Float;False;Smoothness;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;141;284.5915,768.292;Float;False;140;0;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;151;480.1793,1118.334;Float;False;147;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;149;1624.658,597.1019;Float;False;Opacity;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;129;794.6003,1077.427;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;QFX/SFX/Hologram;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;29;2
WireConnection;32;1;26;0
WireConnection;30;0;24;0
WireConnection;30;1;28;2
WireConnection;126;0;162;0
WireConnection;126;1;128;0
WireConnection;126;2;98;2
WireConnection;55;0;51;0
WireConnection;55;1;53;2
WireConnection;11;0;130;0
WireConnection;11;1;23;0
WireConnection;99;0;126;0
WireConnection;99;1;84;2
WireConnection;31;0;30;0
WireConnection;31;1;32;0
WireConnection;106;0;105;2
WireConnection;106;1;107;0
WireConnection;106;2;161;0
WireConnection;54;0;52;2
WireConnection;54;1;49;0
WireConnection;47;0;2;0
WireConnection;6;3;47;0
WireConnection;64;0;55;0
WireConnection;64;1;54;0
WireConnection;103;0;106;0
WireConnection;102;0;99;0
WireConnection;12;0;11;0
WireConnection;33;0;31;0
WireConnection;34;0;33;0
WireConnection;34;1;43;0
WireConnection;7;0;6;0
WireConnection;7;1;5;0
WireConnection;7;2;4;0
WireConnection;63;0;64;0
WireConnection;101;1;103;0
WireConnection;71;0;70;2
WireConnection;71;1;69;0
WireConnection;95;0;160;0
WireConnection;95;1;102;0
WireConnection;35;0;34;0
WireConnection;100;0;95;0
WireConnection;100;1;101;0
WireConnection;8;0;7;0
WireConnection;68;1;71;0
WireConnection;57;0;63;0
WireConnection;36;0;23;4
WireConnection;72;0;68;0
WireConnection;133;0;131;0
WireConnection;133;1;132;0
WireConnection;59;0;38;0
WireConnection;59;1;60;0
WireConnection;59;2;61;0
WireConnection;158;0;155;3
WireConnection;158;1;100;0
WireConnection;157;0;155;2
WireConnection;157;1;100;0
WireConnection;156;0;155;1
WireConnection;156;1;100;0
WireConnection;145;0;142;4
WireConnection;145;1;144;0
WireConnection;62;0;37;0
WireConnection;62;1;59;0
WireConnection;62;2;73;0
WireConnection;146;0;142;1
WireConnection;146;1;143;0
WireConnection;92;0;156;0
WireConnection;92;1;157;0
WireConnection;92;2;158;0
WireConnection;134;0;133;0
WireConnection;136;0;137;0
WireConnection;136;1;18;0
WireConnection;147;0;146;0
WireConnection;87;0;92;0
WireConnection;140;0;139;0
WireConnection;148;0;145;0
WireConnection;149;0;62;0
WireConnection;129;0;17;0
WireConnection;129;1;141;0
WireConnection;129;2;136;0
WireConnection;129;3;151;0
WireConnection;129;4;152;0
WireConnection;129;9;150;0
WireConnection;129;11;88;0
ASEEND*/
//CHKSM=D7618E0D1BD2C7DCE5000291D18AE31526BD6A95