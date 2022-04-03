// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "KuchuBlit"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector]_MainTex("_MainTex", 2D) = "white" {}
		[ASEEnd][ASEBegin]_Distance("Distance", Float) = 0

		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
		
		Cull Back
		AlphaToMask Off
		
		HLSLINCLUDE
		#pragma target 2.0

		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS

		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 100801

			
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
				float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _Distance;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _MainTex;


						
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				#ifdef ASE_FOG
				o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif
				float2 texCoord17_g57 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float dst199_g11 = _Distance;
				float2 appendResult19_g57 = (float2(_ScreenParams.x , _ScreenParams.y));
				float c01556_g11 = 1.0;
				float4 k01285_g11 = ( tex2D( _MainTex, ( texCoord17_g57 + ( ( float2( -2,2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g57 ) ) ) ) * c01556_g11 );
				float2 texCoord17_g45 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g45 = (float2(_ScreenParams.x , _ScreenParams.y));
				float c04557_g11 = 4.0;
				float4 k02413_g11 = ( tex2D( _MainTex, ( texCoord17_g45 + ( ( float2( -1,2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g45 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g49 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g49 = (float2(_ScreenParams.x , _ScreenParams.y));
				float c07558_g11 = 7.0;
				float4 k03417_g11 = ( tex2D( _MainTex, ( texCoord17_g49 + ( ( float2( 0,2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g49 ) ) ) ) * c07558_g11 );
				float2 texCoord17_g60 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g60 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k04421_g11 = ( tex2D( _MainTex, ( texCoord17_g60 + ( ( float2( 1,2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g60 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g63 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g63 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k05425_g11 = ( tex2D( _MainTex, ( texCoord17_g63 + ( ( float2( 2,2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g63 ) ) ) ) * c01556_g11 );
				float2 texCoord17_g59 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g59 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k06429_g11 = ( tex2D( _MainTex, ( texCoord17_g59 + ( ( float2( -2,1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g59 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g40 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g40 = (float2(_ScreenParams.x , _ScreenParams.y));
				float c16559_g11 = 16.0;
				float4 k07510_g11 = ( tex2D( _MainTex, ( texCoord17_g40 + ( ( float2( -1,1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g40 ) ) ) ) * c16559_g11 );
				float2 texCoord17_g50 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g50 = (float2(_ScreenParams.x , _ScreenParams.y));
				float c26560_g11 = 26.0;
				float4 k08437_g11 = ( tex2D( _MainTex, ( texCoord17_g50 + ( ( float2( 0,1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g50 ) ) ) ) * c26560_g11 );
				float2 texCoord17_g58 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g58 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k09441_g11 = ( tex2D( _MainTex, ( texCoord17_g58 + ( ( float2( 1,1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g58 ) ) ) ) * c16559_g11 );
				float2 texCoord17_g64 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g64 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k10445_g11 = ( tex2D( _MainTex, ( texCoord17_g64 + ( ( float2( 2,1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g64 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g47 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g47 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k11449_g11 = ( tex2D( _MainTex, ( texCoord17_g47 + ( ( float2( -2,0 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g47 ) ) ) ) * c07558_g11 );
				float2 texCoord17_g42 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g42 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k12514_g11 = ( tex2D( _MainTex, ( texCoord17_g42 + ( ( float2( -1,0 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g42 ) ) ) ) * c26560_g11 );
				float2 texCoord17_g46 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g46 = (float2(_ScreenParams.x , _ScreenParams.y));
				float c41561_g11 = 41.0;
				float4 k13457_g11 = ( tex2D( _MainTex, ( texCoord17_g46 + ( ( float2( 0,0 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g46 ) ) ) ) * c41561_g11 );
				float2 texCoord17_g53 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g53 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k14461_g11 = ( tex2D( _MainTex, ( texCoord17_g53 + ( ( float2( 1,0 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g53 ) ) ) ) * c26560_g11 );
				float2 texCoord17_g56 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g56 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k15465_g11 = ( tex2D( _MainTex, ( texCoord17_g56 + ( ( float2( 2,0 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g56 ) ) ) ) * c07558_g11 );
				float2 texCoord17_g62 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g62 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k16469_g11 = ( tex2D( _MainTex, ( texCoord17_g62 + ( ( float2( -2,-1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g62 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g44 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g44 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k17518_g11 = ( tex2D( _MainTex, ( texCoord17_g44 + ( ( float2( -1,-1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g44 ) ) ) ) * c16559_g11 );
				float2 texCoord17_g51 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g51 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k18477_g11 = ( tex2D( _MainTex, ( texCoord17_g51 + ( ( float2( 0,-1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g51 ) ) ) ) * c26560_g11 );
				float2 texCoord17_g55 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g55 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k19481_g11 = ( tex2D( _MainTex, ( texCoord17_g55 + ( ( float2( 1,-1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g55 ) ) ) ) * c16559_g11 );
				float2 texCoord17_g52 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g52 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k20485_g11 = ( tex2D( _MainTex, ( texCoord17_g52 + ( ( float2( 2,-1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g52 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g41 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g41 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k21490_g11 = ( tex2D( _MainTex, ( texCoord17_g41 + ( ( float2( -2,-2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g41 ) ) ) ) * c01556_g11 );
				float2 texCoord17_g48 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g48 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k22522_g11 = ( tex2D( _MainTex, ( texCoord17_g48 + ( ( float2( -1,2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g48 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g43 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g43 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k23498_g11 = ( tex2D( _MainTex, ( texCoord17_g43 + ( ( float2( 0,-2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g43 ) ) ) ) * c07558_g11 );
				float2 texCoord17_g54 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g54 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k24502_g11 = ( tex2D( _MainTex, ( texCoord17_g54 + ( ( float2( 1,-2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g54 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g61 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g61 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k25506_g11 = ( tex2D( _MainTex, ( texCoord17_g61 + ( ( float2( 2,-2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g61 ) ) ) ) * c01556_g11 );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( ( ( k01285_g11 + k02413_g11 + k03417_g11 + k04421_g11 + k05425_g11 ) + ( k06429_g11 + k07510_g11 + k08437_g11 + k09441_g11 + k10445_g11 ) + ( k11449_g11 + k12514_g11 + k13457_g11 + k14461_g11 + k15465_g11 ) + ( k16469_g11 + k17518_g11 + k18477_g11 + k19481_g11 + k20485_g11 ) + ( k21490_g11 + k22522_g11 + k23498_g11 + k24502_g11 + k25506_g11 ) ) / 273.0 ).rgb;
				float Alpha = (( ( ( k01285_g11 + k02413_g11 + k03417_g11 + k04421_g11 + k05425_g11 ) + ( k06429_g11 + k07510_g11 + k08437_g11 + k09441_g11 + k10445_g11 ) + ( k11449_g11 + k12514_g11 + k13457_g11 + k14461_g11 + k15465_g11 ) + ( k16469_g11 + k17518_g11 + k18477_g11 + k19481_g11 + k20485_g11 ) + ( k21490_g11 + k22522_g11 + k23498_g11 + k24502_g11 + k25506_g11 ) ) / 273.0 )).r;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				return half4( Color, Alpha );
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM
			
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 100801

			
			#pragma vertex vert
			#pragma fragment frag
#if ASE_SRP_VERSION >= 110000
			#pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW
#endif
			#define SHADERPASS SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _Distance;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _MainTex;


			
			float3 _LightDirection;
#if ASE_SRP_VERSION >= 110000 
			float3 _LightPosition;
#endif
			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir( v.ase_normal );
#if ASE_SRP_VERSION >= 110000 
			#if _CASTING_PUNCTUAL_LIGHT_SHADOW
				float3 lightDirectionWS = normalize(_LightPosition - positionWS);
			#else
				float3 lightDirectionWS = _LightDirection;
			#endif
				float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
			#if UNITY_REVERSED_Z
				clipPos.z = min(clipPos.z, UNITY_NEAR_CLIP_VALUE);
			#else
				clipPos.z = max(clipPos.z, UNITY_NEAR_CLIP_VALUE);
			#endif
#else
				float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );
				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#endif
#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;

				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 texCoord17_g57 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float dst199_g11 = _Distance;
				float2 appendResult19_g57 = (float2(_ScreenParams.x , _ScreenParams.y));
				float c01556_g11 = 1.0;
				float4 k01285_g11 = ( tex2D( _MainTex, ( texCoord17_g57 + ( ( float2( -2,2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g57 ) ) ) ) * c01556_g11 );
				float2 texCoord17_g45 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g45 = (float2(_ScreenParams.x , _ScreenParams.y));
				float c04557_g11 = 4.0;
				float4 k02413_g11 = ( tex2D( _MainTex, ( texCoord17_g45 + ( ( float2( -1,2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g45 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g49 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g49 = (float2(_ScreenParams.x , _ScreenParams.y));
				float c07558_g11 = 7.0;
				float4 k03417_g11 = ( tex2D( _MainTex, ( texCoord17_g49 + ( ( float2( 0,2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g49 ) ) ) ) * c07558_g11 );
				float2 texCoord17_g60 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g60 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k04421_g11 = ( tex2D( _MainTex, ( texCoord17_g60 + ( ( float2( 1,2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g60 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g63 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g63 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k05425_g11 = ( tex2D( _MainTex, ( texCoord17_g63 + ( ( float2( 2,2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g63 ) ) ) ) * c01556_g11 );
				float2 texCoord17_g59 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g59 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k06429_g11 = ( tex2D( _MainTex, ( texCoord17_g59 + ( ( float2( -2,1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g59 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g40 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g40 = (float2(_ScreenParams.x , _ScreenParams.y));
				float c16559_g11 = 16.0;
				float4 k07510_g11 = ( tex2D( _MainTex, ( texCoord17_g40 + ( ( float2( -1,1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g40 ) ) ) ) * c16559_g11 );
				float2 texCoord17_g50 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g50 = (float2(_ScreenParams.x , _ScreenParams.y));
				float c26560_g11 = 26.0;
				float4 k08437_g11 = ( tex2D( _MainTex, ( texCoord17_g50 + ( ( float2( 0,1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g50 ) ) ) ) * c26560_g11 );
				float2 texCoord17_g58 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g58 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k09441_g11 = ( tex2D( _MainTex, ( texCoord17_g58 + ( ( float2( 1,1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g58 ) ) ) ) * c16559_g11 );
				float2 texCoord17_g64 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g64 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k10445_g11 = ( tex2D( _MainTex, ( texCoord17_g64 + ( ( float2( 2,1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g64 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g47 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g47 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k11449_g11 = ( tex2D( _MainTex, ( texCoord17_g47 + ( ( float2( -2,0 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g47 ) ) ) ) * c07558_g11 );
				float2 texCoord17_g42 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g42 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k12514_g11 = ( tex2D( _MainTex, ( texCoord17_g42 + ( ( float2( -1,0 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g42 ) ) ) ) * c26560_g11 );
				float2 texCoord17_g46 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g46 = (float2(_ScreenParams.x , _ScreenParams.y));
				float c41561_g11 = 41.0;
				float4 k13457_g11 = ( tex2D( _MainTex, ( texCoord17_g46 + ( ( float2( 0,0 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g46 ) ) ) ) * c41561_g11 );
				float2 texCoord17_g53 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g53 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k14461_g11 = ( tex2D( _MainTex, ( texCoord17_g53 + ( ( float2( 1,0 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g53 ) ) ) ) * c26560_g11 );
				float2 texCoord17_g56 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g56 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k15465_g11 = ( tex2D( _MainTex, ( texCoord17_g56 + ( ( float2( 2,0 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g56 ) ) ) ) * c07558_g11 );
				float2 texCoord17_g62 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g62 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k16469_g11 = ( tex2D( _MainTex, ( texCoord17_g62 + ( ( float2( -2,-1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g62 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g44 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g44 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k17518_g11 = ( tex2D( _MainTex, ( texCoord17_g44 + ( ( float2( -1,-1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g44 ) ) ) ) * c16559_g11 );
				float2 texCoord17_g51 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g51 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k18477_g11 = ( tex2D( _MainTex, ( texCoord17_g51 + ( ( float2( 0,-1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g51 ) ) ) ) * c26560_g11 );
				float2 texCoord17_g55 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g55 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k19481_g11 = ( tex2D( _MainTex, ( texCoord17_g55 + ( ( float2( 1,-1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g55 ) ) ) ) * c16559_g11 );
				float2 texCoord17_g52 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g52 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k20485_g11 = ( tex2D( _MainTex, ( texCoord17_g52 + ( ( float2( 2,-1 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g52 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g41 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g41 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k21490_g11 = ( tex2D( _MainTex, ( texCoord17_g41 + ( ( float2( -2,-2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g41 ) ) ) ) * c01556_g11 );
				float2 texCoord17_g48 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g48 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k22522_g11 = ( tex2D( _MainTex, ( texCoord17_g48 + ( ( float2( -1,2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g48 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g43 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g43 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k23498_g11 = ( tex2D( _MainTex, ( texCoord17_g43 + ( ( float2( 0,-2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g43 ) ) ) ) * c07558_g11 );
				float2 texCoord17_g54 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g54 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k24502_g11 = ( tex2D( _MainTex, ( texCoord17_g54 + ( ( float2( 1,-2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g54 ) ) ) ) * c04557_g11 );
				float2 texCoord17_g61 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult19_g61 = (float2(_ScreenParams.x , _ScreenParams.y));
				float4 k25506_g11 = ( tex2D( _MainTex, ( texCoord17_g61 + ( ( float2( 2,-2 ) * dst199_g11 ) * ( float2( 1,1 ) / appendResult19_g61 ) ) ) ) * c01556_g11 );
				
				float Alpha = (( ( ( k01285_g11 + k02413_g11 + k03417_g11 + k04421_g11 + k05425_g11 ) + ( k06429_g11 + k07510_g11 + k08437_g11 + k09441_g11 + k10445_g11 ) + ( k11449_g11 + k12514_g11 + k13457_g11 + k14461_g11 + k15465_g11 ) + ( k16469_g11 + k17518_g11 + k18477_g11 + k19481_g11 + k20485_g11 ) + ( k21490_g11 + k22522_g11 + k23498_g11 + k24502_g11 + k25506_g11 ) ) / 273.0 )).r;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}

			ENDHLSL
		}

	
	}
	
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18935
1952;47;1920;923;588.752;12.0208;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;47;-226.7521,480.9792;Inherit;False;Property;_Distance;Distance;1;0;Create;True;0;0;0;False;0;False;0;3.66;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-304.1278,364.2082;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;21;-310.1177,170.836;Inherit;True;Property;_MainTex;_MainTex;0;1;[HideInInspector];Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.FunctionNode;46;70.75571,354.4508;Inherit;False;GaussianBlur3X3;-1;;10;b73b4763fabe8b2448fc162a66815e8a;0;3;9;SAMPLER2D;0;False;1;FLOAT2;0,0;False;198;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;50;66.24799,503.9792;Inherit;False;GaussianBlur5x5;-1;;11;7afe02747ebff284f8461f31de6f2b80;0;2;9;SAMPLER2D;0;False;198;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwitchNode;51;379.248,392.9792;Inherit;False;1;2;8;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwizzleNode;49;605.248,456.9792;Inherit;False;FLOAT;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;797.1001,353.1;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;KuchuBlit;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;22;Surface;0;637809255917927340;  Blend;0;0;Two Sided;1;637809056680731684;Cast Shadows;1;0;  Use Shadow Threshold;0;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;DOTS Instancing;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,-1;0;  Type;0;0;  Tess;16,False,-1;0;  Min;10,False,-1;0;  Max;25,False,-1;0;  Edge Length;16,False,-1;0;  Max Displacement;25,False,-1;0;Vertex Position,InvertActionOnDeselection;1;0;0;5;False;True;True;False;False;False;;False;0
WireConnection;46;9;21;0
WireConnection;46;1;36;0
WireConnection;46;198;47;0
WireConnection;50;9;21;0
WireConnection;50;198;47;0
WireConnection;51;0;46;0
WireConnection;51;1;50;0
WireConnection;49;0;51;0
WireConnection;1;2;51;0
WireConnection;1;3;49;0
ASEEND*/
//CHKSM=6E9665D8C7BDEB66ADF629935CAEBA15D4D1E296