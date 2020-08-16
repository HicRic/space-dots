using Unity.Entities;

[GenerateAuthoringComponent]
public struct PickupEmitter : IComponentData
{
    public uint PickupId;
    public float EmissionFrequency;
    public double LastEmissionTime;
}

