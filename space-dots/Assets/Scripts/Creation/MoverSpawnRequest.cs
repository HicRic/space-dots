using Unity.Entities;
using Unity.Mathematics;

public struct MoverSpawnRequest : IComponentData
{
    public uint ConfigId;
    public float3 Position;
    public float2 Velocity;
}

