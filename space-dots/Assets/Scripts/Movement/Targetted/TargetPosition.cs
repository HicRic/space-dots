using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, GenerateAuthoringComponent]
public struct TargetPosition : IComponentData
{
    public float2 Value;
}
