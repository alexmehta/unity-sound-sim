Shader "Custom/Vertex"
{
    Properties
    {
        _PointSize("Point size", Float) = 20
        _Color("Main Color", Color) = (1,1,1,1)
    }
        SubShader
    {
        Pass
        {
            LOD 200

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag               

            uniform float _PointSize = 20;
            uniform float4 _Color = (1, 1, 1, 1);


            struct VertexInput
            {
                float4 vertex: POSITION;
                float4 color: COLOR;
             };

             struct VertexOutput
             {
                 float4 pos: SV_POSITION;
                 float4 col: COLOR;
                 float size : PSIZE;
              };

              VertexOutput vert(VertexInput v)
              {
                  VertexOutput o;
                  o.pos = UnityObjectToClipPos(v.vertex);
                  o.col = _Color;
                  o.size = _PointSize;

                  return o;
              }

              float4 frag() : COLOR
              {
                  return _Color;
              }
              ENDCG
          }
    }
}