using Unity.Entities;
using Unity.Mathematics;

public struct DamageInstance : IComponentData
{
    public float DamageAmount;
    public Entity DamagedEntity;
}

