using Unity.Entities;
using Unity.Jobs;

public class TimedDestroySystem : SystemBase
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

        double time = Time.ElapsedTime;
        JobHandle jobDepend = Entities.ForEach((Entity entity, in TimedDestroy timedDestroy) =>
        {
            if (timedDestroy.DestroyTime <= time)
            {
                ecb.DestroyEntity(entity);
            }
        }).Schedule(Dependency);

        ecbs.AddJobHandleForProducer(jobDepend);
        Dependency = jobDepend;
    }
}
