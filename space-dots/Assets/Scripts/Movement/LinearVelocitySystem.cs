using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class LinearVelocitySystem : SystemBase
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
        float delta = Time.DeltaTime;

        JobHandle addSpeedToFacingJob = Entities
            .ForEach((Entity entity, ref LinearVelocity velocity, ref SpawnSpeed speed, in Rotation rotation) =>
        {
            float2 facing = math.mul(rotation.Value, math.up()).xy;
            facing *= speed.Value;
            velocity.Value += facing;
            entityCommandBuffer.RemoveComponent<SpawnSpeed>(entity);
        }).Schedule(Dependency);

        ecbs.AddJobHandleForProducer(addSpeedToFacingJob);

        JobHandle addVelocityToTranslationJob = Entities.ForEach((ref Translation translation, in LinearVelocity velocity) =>
        {
            float2 moveDelta = velocity.Value * delta;
            translation.Value += new float3(moveDelta, 0);
        }).Schedule(addSpeedToFacingJob);

        Dependency = addVelocityToTranslationJob;
    }
}
