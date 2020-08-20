using Unity.Entities;

[GenerateAuthoringComponent]
public struct DamageLimit : IComponentData
{
    public int Value;
}

