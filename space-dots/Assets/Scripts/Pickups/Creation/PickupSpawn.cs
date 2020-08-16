using Unity.Entities;
using Unity.Mathematics;

public struct PickupSpawn : IComponentData
{
    public uint PickupId;
    public float3 Position;
}

