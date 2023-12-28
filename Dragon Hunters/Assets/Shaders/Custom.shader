Shader "Custom/TransparentStandard" {
    Properties{
        _Color("Main Color", Color) = (.5,.5,.5,1)
        _MainTex("Base (RGB)", 2D) = "white" { }
    }

        SubShader{
            Tags { "Queue" = "Overlay" }
            LOD 100

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ColorMask RGB

            Pass {
                Name "FORWARD"
                Tags { "LightMode" = "ForwardBase" }
            }
    }
}
