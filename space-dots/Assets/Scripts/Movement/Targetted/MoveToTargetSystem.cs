﻿using Unity.Burst;
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
            .ForEach((ref Translation translation, ref MoveSpeed speed, in TargetPosition target) =>
            {
                speed.Value += speed.DeltaPerSecond * deltaTime;

                float2 distance = target.Value - translation.Value.xy;
                float2 dir = math.normalizesafe(distance);
                float2 delta = dir * speed.Value * deltaTime;
                float2 finalMove = math.select(delta, distance, math.lengthsq(delta) > math.lengthsq(distance));
                translation.Value.xy += finalMove;
            }).Schedule();

        Entities
            .ForEach((ref MotionIntent intent, in Translation translation, in TargetPosition target) =>
            {
                float2 direction = target.Value - translation.Value.xy;
                intent.DirectionNormalized = math.normalizesafe(direction);
                intent.ThrottleNormalized = 1f;
            }).Schedule();
    }
}
