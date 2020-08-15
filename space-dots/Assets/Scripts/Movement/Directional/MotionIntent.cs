using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MotionIntent : IComponentData
{
    // 0 = no acceleration thanks
    // 1 = max acceleration please
    public float ThrottleNormalized;

    // Which direction do we want to face?
    public float2 DirectionNormalized;
}

