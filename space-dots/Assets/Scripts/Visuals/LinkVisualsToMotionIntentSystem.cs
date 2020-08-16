using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class LinkVisualsToMotionIntentSystem : SystemBase
{
    protected override void OnUpdate()
    {
        ComponentDataFromEntity<MotionIntent> intents = GetComponentDataFromEntity<MotionIntent>(true);

        Entities
            .WithoutBurst()
            .ForEach((TrailRenderer renderer, in LinkVisualsToMotionIntent link) =>
            {
                if (intents.HasComponent(link.motionIntent))
                {
                    MotionIntent intent = intents[link.motionIntent];
                    renderer.emitting = intent.ThrottleNormalized > 0;
                }
            }).Run();

        Entities
            .WithoutBurst()
            .ForEach((ParticleSystem system, in LinkVisualsToMotionIntent link) =>
            {
                if (intents.HasComponent(link.motionIntent))
                {
                    MotionIntent intent = intents[link.motionIntent];
                    bool emitFx = intent.ThrottleNormalized > 0;
                    if (emitFx && !system.isEmitting)
                    {
                        system.Play(true);
                    }
                    else if(!emitFx && system.isEmitting)
                    {
                        system.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    }
                }
            }).Run();
    }
}
