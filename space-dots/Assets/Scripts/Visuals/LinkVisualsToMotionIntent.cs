using Unity.Entities;

[GenerateAuthoringComponent]
public struct LinkVisualsToMotionIntent : IComponentData
{
    public Entity motionIntent;
}

