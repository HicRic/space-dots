using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PickupSystem : SystemBase
{
    private EntityQuery attractorQuery;
    private EntityCommandBufferSystem ecbs;

    protected override void OnCreate()
    {
        base.OnCreate();
        attractorQuery = GetEntityQuery(ComponentType.ReadOnly<PickupAttractor>(), ComponentType.ReadOnly<Translation>());
        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer entityCommandBuffer = ecbs.CreateCommandBuffer();

        NativeArray<Entity> attractorEntities = attractorQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle entityArrayJob);
        NativeArray<Translation> attractorTranslations = attractorQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out JobHandle translationArrayJob);
        NativeArray<PickupAttractor> attractors = attractorQuery.ToComponentDataArrayAsync<PickupAttractor>(Allocator.TempJob, out JobHandle attractorArrayJob);

        JobHandle inputDepends = JobHandle.CombineDependencies(Dependency, translationArrayJob, entityArrayJob);
        inputDepends = JobHandle.CombineDependencies(inputDepends, attractorArrayJob);

        // Iterate all entities with PickupTag that don't have a FollowEntity
        JobHandle outputDepends = Entities
            .WithAll<PickupTag>()
            .WithNone<FollowEntity>()
            .WithDisposeOnCompletion(attractorEntities)
            .WithDisposeOnCompletion(attractorTranslations)
            .WithDisposeOnCompletion(attractors)
            .ForEach((ref Entity pickupEntity, in Translation translation) =>
            {
                // Find the closest PickupAttractor to this entity
                int closestIdx = -1;
                float closestDistSq = float.MaxValue;
                for (var i = 0; i < attractorTranslations.Length; i++)
                {
                    Translation attractorTranslation = attractorTranslations[i];
                    float2 separation = attractorTranslation.Value.xy - translation.Value.xy;

                    float attractionRadius = attractors[i].AttractionRadius;
                    float distSq = math.lengthsq(separation);
                    if (distSq < attractionRadius * attractionRadius && distSq < closestDistSq)
                    {
                        closestDistSq = distSq;
                        closestIdx = i;
                    }
                }

                if (closestIdx > -1)
                {
                    // Tell the entity to follow the closest attractor
                    entityCommandBuffer.AddComponent(pickupEntity,
                        new FollowEntity
                        {
                            Target = attractorEntities[closestIdx]
                        });
                    entityCommandBuffer.AddComponent<TargetPosition>(pickupEntity);
                }

            }).Schedule(inputDepends);

        ecbs.AddJobHandleForProducer(outputDepends);
        Dependency = outputDepends;
    }
}
