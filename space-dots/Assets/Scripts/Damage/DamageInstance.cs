using Unity.Entities;
using Unity.Mathematics;

public struct DamageInstance : IComponentData
{
    public int DamageAmount;
    public Entity DamagedEntity;
}

