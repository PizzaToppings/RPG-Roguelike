void ToonShader_float(in float3 Normal, in float ToonRampSmoothness, in float3 ClipSpacePos, in float3 WorldPos, in float4 ToonRampTinting,
    in float ToonRampOffset, out float3 ToonRampOutput, out float3 Direction)
{
    // Default values (prevents uninitialized outputs)
    ToonRampOutput = float3(0, 0, 0);
    Direction = float3(0, 0, 1); // Default to a valid direction

    #ifdef SHADERGRAPH_PREVIEW
        ToonRampOutput = float3(0.5,0.5,0);
        Direction = float3(0.5,0.5,0);
    #else
        #if SHADOWS_SCREEN
            half4 shadowCoord = ComputeScreenPos(ClipSpacePos);
        #else
            half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        #endif

        Light light;

        #if defined(_MAIN_LIGHT_SHADOWS_CASCADE) || defined(_MAIN_LIGHT_SHADOWS)
            light = GetMainLight(shadowCoord);
        #else
            light = GetMainLight();
        #endif

        // Ensure Direction is initialized properly
        if (light.color.r + light.color.g + light.color.b > 0) // Check if light is valid
        {
            Direction = light.direction;
        }

        half d = dot(Normal, Direction) * 0.5 + 0.5;
        half toonRamp = smoothstep(ToonRampOffset, ToonRampOffset + ToonRampSmoothness, d);

        toonRamp *= light.shadowAttenuation;

        ToonRampOutput = light.color * (toonRamp + ToonRampTinting);
    #endif
}
