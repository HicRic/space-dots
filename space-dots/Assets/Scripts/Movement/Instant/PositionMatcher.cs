using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class PositionMatcher : IComponentData
{
    public GameObject ObjectToMove;
}

