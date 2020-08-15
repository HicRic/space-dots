using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class PositionMatcherSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .ForEach((in LocalToWorld l2w, in PositionMatcher positionMatcher) =>
        {
            positionMatcher.ObjectToMove.transform.position = l2w.Position;
        }).Run();
    }
}
