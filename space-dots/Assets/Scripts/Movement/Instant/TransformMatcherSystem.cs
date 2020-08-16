using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(EndFrameLocalToParentSystem))]
public class TransformMatcherSystem : SystemBase
{
    protected override void OnUpdate()
    {
        
        Entities
            .WithoutBurst()
            .ForEach((in LocalToWorld l2w, in TransformMatcher matcher) =>
        {
            matcher.TransformToSet.position = l2w.Position;
            matcher.TransformToSet.rotation = l2w.Rotation;
        }).Run();
    }
}
