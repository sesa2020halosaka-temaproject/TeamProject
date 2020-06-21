Shader "Unlit/Fuck"
{
	
		Properties{
			_Color("Color", Color) = (1,1,1,1)
		}

			SubShader{
				Tags {"Queue" = "Geometry-1" "RenderType" = "Transparent"}

				Pass{
					Zwrite On
					ColorMask 0
				}
		}
	
	
}
