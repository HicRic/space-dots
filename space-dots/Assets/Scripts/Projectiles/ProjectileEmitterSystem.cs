using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ProjectileEmitterSystem : SystemBase
{
    private EntityCommandBufferSystem ecbs;

    protected override void OnCreate()
    {
        base.OnCreate();

        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();
        double elapsedTime = Time.ElapsedTime;

        Entities
            .WithoutBurst()
            .ForEach((ref ProjectileEmitter emitter, in LocalToWorld l2w) =>
        {
            while (emitter.EmissionFrequency > 0 && emitter.LastEmissionTime + emitter.EmissionFrequency < elapsedTime)
            {
                emitter.LastEmissionTime += emitter.EmissionFrequency;
                Entity spawn = ecb.CreateEntity();
                float2 inheritVelocity = EntityManager.GetComponentData<LinearVelocity>(emitter.InheritVelocityFromEntity).Value;
                ecb.AddComponent(spawn, new MoverSpawnRequest
                {
                    Position = l2w.Position.xy,
                    Rotation = l2w.Rotation,
                    ConfigId = emitter.ProjectileId,
                    Velocity = inheritVelocity
                });
            }
        }).Run();
    }
}
