﻿using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, GenerateAuthoringComponent]
public struct MoveSpeed : IComponentData
{
    public float Value;
    public float DeltaPerSecond;
}
