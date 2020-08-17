using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, GenerateAuthoringComponent]
public struct CurrencyXP : IComponentData
{
    public int Amount;
}
