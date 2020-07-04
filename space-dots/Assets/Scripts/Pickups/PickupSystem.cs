using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PickupSystem : SystemBase
{
    private EntityQuery attractorQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        attractorQuery = GetEntityQuery(ComponentType.ReadOnly<PickupAttractor>(), ComponentType.ReadOnly<Translation>());
    }

    protected override void OnUpdate()
    {
        NativeArray<Translation> attractorTranslations =
            attractorQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out JobHandle arrayJob);
        JobHandle inputDepends = JobHandle.CombineDependencies(Dependency, arrayJob);

        JobHandle outputDepends = Entities
            .WithAll<PickupTag>()
            .WithNone<FollowEntity>()
            .WithDeallocateOnJobCompletion(attractorTranslations)
            .ForEach((ref Entity pickupEntity, in Translation translation) =>
            {
                for (var i = 0; i < attractorTranslations.Length; i++)
                {
                    Translation attractorTranslation = attractorTranslations[i];
                    float3 seperation = attractorTranslation.Value - translation.Value;
                    float distSq = math.lengthsq(seperation);
                }

                //float3 dir = math.normalizesafe(distance);
                //float3 delta = dir * speed.Value * deltaTime;
                //float3 finalMove = math.select(delta, distance, math.lengthsq(delta) > math.lengthsq(distance));
                //translation.Value += finalMove;
            }).Schedule(inputDepends);

        Dependency = outputDepends;
    }
}
