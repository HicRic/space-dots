using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class FollowEntitySystem : SystemBase
{
    private EntityCommandBufferSystem ecbs;

    protected override void OnCreate()
    {
        base.OnCreate();

        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer entityCommandBuffer = ecbs.CreateCommandBuffer();
        ComponentDataFromEntity<Translation> translationData = GetComponentDataFromEntity<Translation>(true);

        JobHandle outputDepends = Entities
            .WithReadOnly(translationData)
            .ForEach((ref Entity entity, ref TargetPosition targetPosition, in FollowEntity follow) =>
            {
                if (translationData.HasComponent(follow.Target))
                {
                    // If our FollowEntity's Translation is found, update our TargetPosition
                    Translation target = translationData[follow.Target];
                    targetPosition.Value = target.Value.xy;
                }
                else
                {
                    // Can't find the FollowEntity Translation, so we can't follow any more.
                    entityCommandBuffer.RemoveComponent<FollowEntity>(entity);
                }
            }).Schedule(Dependency);

        ecbs.AddJobHandleForProducer(outputDepends);
        Dependency = outputDepends;
    }
}
