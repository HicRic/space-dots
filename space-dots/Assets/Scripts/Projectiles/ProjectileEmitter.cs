using Unity.Entities;

[GenerateAuthoringComponent]
public struct ProjectileEmitter : IComponentData
{
    public uint ProjectileId;
    public float EmissionFrequency;
    public double LastEmissionTime;
    public Entity InheritVelocityFromEntity;
}

