using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveToTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities
            .ForEach((ref Translation translation, in TargetPosition target, in MoveSpeed speed) =>
            {
                float3 distance = target.Value - translation.Value;
                float3 dir = math.normalizesafe(distance);
                float3 delta = dir * speed.Value * deltaTime;
                float3 finalMove = math.select(delta, distance, math.lengthsq(delta) > math.lengthsq(distance));
                translation.Value += finalMove;

            }).Schedule();
    }
}
