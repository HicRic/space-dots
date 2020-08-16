using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class PickupEmitterSystem : SystemBase
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

        JobHandle outDepends = Entities.ForEach((ref PickupEmitter emitter, in Translation translation) =>
        {
            while(emitter.EmissionFrequency > 0 && emitter.LastEmissionTime + emitter.EmissionFrequency < elapsedTime)
            {
                emitter.LastEmissionTime += emitter.EmissionFrequency;
                Entity spawn = ecb.CreateEntity();
                ecb.AddComponent(spawn, new PickupSpawn
                {
                    Position = translation.Value,
                    PickupId = emitter.PickupId
                });
            }
        }).Schedule(Dependency);

        ecbs.AddJobHandleForProducer(outDepends);
        Dependency = outDepends;
    }
}
