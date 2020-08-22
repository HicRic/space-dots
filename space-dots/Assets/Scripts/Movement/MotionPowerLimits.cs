using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MotionPowerLimits : IComponentData
{
    public float LinearAccelerationLimit;
    public float LinearVelocityLimit;
    public float AngularVelocityLimit;
}

