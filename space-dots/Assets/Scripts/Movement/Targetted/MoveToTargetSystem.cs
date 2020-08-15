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
                float2 distance = target.Value - translation.Value.xy;
                float2 dir = math.normalizesafe(distance);
                float2 delta = dir * speed.Value * deltaTime;
                float2 finalMove = math.select(delta, distance, math.lengthsq(delta) > math.lengthsq(distance));
                translation.Value.xy += finalMove;

            }).Schedule();
    }
}
