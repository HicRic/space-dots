using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PickupSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecbs;

    protected override void OnCreate()
    {
        base.OnCreate();
        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();

        ComponentDataFromEntity<Translation> translations = GetComponentDataFromEntity<Translation>(true);
        ComponentDataFromEntity<PickupAttractor> attractors = GetComponentDataFromEntity<PickupAttractor>(true);

        JobHandle outputDepend = Entities
            .WithAll<PickupTag>()
            .WithReadOnly(translations)
            .WithReadOnly(attractors)
            .ForEach((ref Entity entity, in Translation translation, in FollowEntity followEntity) =>
            {
                if (translations.HasComponent(followEntity.Target) && attractors.HasComponent(followEntity.Target))
                {
                    float distSq = math.distancesq(translation.Value, translations[followEntity.Target].Value);
                    float pickupRadius = attractors[followEntity.Target].PickupRadius;

                    if (distSq <= pickupRadius * pickupRadius)
                    {
                        ecb.AddComponent(entity, new PickupCollect { Target = followEntity.Target });
                        ecb.RemoveComponent<PickupTag>(entity);
                        ecb.RemoveComponent<FollowEntity>(entity);
                    }
                }
            }).Schedule(Dependency);

        ecbs.AddJobHandleForProducer(outputDepend);
        Dependency = outputDepend;
    }
}
