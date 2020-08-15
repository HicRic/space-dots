using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct LinearAcceleration : IComponentData
{
    public float2 Value;
}

