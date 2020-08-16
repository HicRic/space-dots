using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class TransformMatcher : IComponentData
{
    public Transform TransformToSet;
}

