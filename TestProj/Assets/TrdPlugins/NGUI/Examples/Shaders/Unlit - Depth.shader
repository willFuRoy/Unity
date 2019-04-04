Shader "Unlit/Depth"
{
	SubShader
	{
		Lod 10000

		Tags
		{
			"Queue" = "Geometry+1"
			"RenderType"="Opaque"
		}
		
		Pass
        {
            ZWrite On
            ZTest LEqual
            ColorMask 0
        }
	}
}