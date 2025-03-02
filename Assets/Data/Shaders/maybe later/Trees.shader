Shader "Custom/Trees"
{
Properties
{
    _Color("Color", Color) = (1, 1, 1, 1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _ScaleFactor("Scale Factor", Float) = 0.5
}

SubShader
{
    Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

    Pass
    {
        CGPROGRAM
        fixed4 _Color;
        sampler2D _MainTex;
        float _ScaleFactor;  // Declare the scale factor

        #pragma vertex vert;
        #pragma fragment frag;

        struct Appdata
        {
            float4 vertex:POSITION;
            float2 uv:TEXCOORD0;
        };

        struct v2f
        {
            float4 vertex:POSITION;
            float2 uv:TEXCOORD0;
        };

        // Frustum culling check
        bool IsInFrustum(float4 clipPos)
        {
            return (clipPos.x >= -clipPos.w && clipPos.x <= clipPos.w &&
                    clipPos.y >= -clipPos.w && clipPos.y <= clipPos.w &&
                    clipPos.z >= 0 && clipPos.z <= clipPos.w);
        }

        v2f vert(Appdata v, uint id:SV_InstanceID)
        {
            int x = (id / 1000) / _ScaleFactor / 10;
            int y = 0;
            int z = fmod(id, 1000) / _ScaleFactor / 10;

            float3 pos = float3(x, y, z);
            float3 vertPos = v.vertex.xyz + pos;
            
            // Apply the scale factor to shrink the mesh
            vertPos *= _ScaleFactor;  // Shrink the mesh

            float4 vertPos4 = float4(vertPos, 1.0);
            float4 clipPos = mul(UNITY_MATRIX_VP, vertPos4);

            // Perform frustum culling
            if (!IsInFrustum(clipPos))
            {
                clipPos.w = 0; // If out of view, set to zero so it's discarded
            }

            v2f o;
            o.vertex = clipPos;
            o.uv = v.uv;
            return o;
        }

        fixed4 frag(v2f i):SV_Target
        {
            fixed4 col = tex2D(_MainTex, i.uv);
            col = col * _Color;
            return col;
        }

        ENDCG
    }
}
}