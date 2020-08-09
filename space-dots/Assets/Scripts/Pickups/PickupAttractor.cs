using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, GenerateAuthoringComponent]
public struct PickupAttractor : IComponentData
{
    public float AttractionRadius;
}
