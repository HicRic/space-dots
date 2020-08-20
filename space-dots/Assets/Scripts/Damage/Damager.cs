using System;
using Unity.Entities;

[Serializable]
public struct Damager : IComponentData
{
    public int Amount;
}

