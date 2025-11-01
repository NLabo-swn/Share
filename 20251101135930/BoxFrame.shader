Shader "Custom/BoxFrame"
{
    Properties
    {
        _FillColor ("Fill Color", Color) = (1,1,1,1)
        _EdgeColor ("Edge Color", Color) = (0,0,0,1)
        _Thickness ("Edge Thickness (world units)", Float) = 0.02
        _HalfExtents ("Half Extents (world)", Vector) = (0.5,0.5,0.5,0)
        _WorldCenter ("World Center", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _FillColor;
            fixed4 _EdgeColor;
            float _Thickness;
            float3 _HalfExtents;
            float3 _WorldCenter;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float4 worldPos4 = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos4.xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                float3 local = IN.worldPos - _WorldCenter; // vector from center in world units
                float3 d = abs(local); // distance along each axis from center
                float3 faceDist = _HalfExtents - d; // distance inward from face along each axis

                // near-face if faceDist < thickness
                int nearX = faceDist.x < _Thickness ? 1 : 0;
                int nearY = faceDist.y < _Thickness ? 1 : 0;
                int nearZ = faceDist.z < _Thickness ? 1 : 0;

                int nearCount = nearX + nearY + nearZ;

                // If pixel is near at least two faces => it's on an edge (frame)
                if (nearCount >= 2)
                {
                    return _EdgeColor;
                }
                else
                {
                    return _FillColor;
                }
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}