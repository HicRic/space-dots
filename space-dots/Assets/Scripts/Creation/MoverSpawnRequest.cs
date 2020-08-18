using Unity.Entities;
using Unity.Mathematics;

public struct MoverSpawnRequest : IComponentData
{
    public uint ConfigId;
    public float2 Position;
    public float2 Velocity;
    public quaternion Rotation;
}

