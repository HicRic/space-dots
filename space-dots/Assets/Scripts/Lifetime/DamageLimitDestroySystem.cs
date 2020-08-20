using Unity.Entities;
using Unity.Jobs;

public class DamageLimitDestroySystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecbs;

    protected override void OnCreate()
    {
        base.OnCreate();

        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();

        Entities.ForEach((Entity entity, in DamageLimit damageLimit, in DamageTaken damageTaken) =>
        {
            bool destroy = damageTaken.Value >= damageLimit.Value;
            if (destroy)
            {
                ecb.DestroyEntity(entity);
            }
        }).Schedule();

        ecbs.AddJobHandleForProducer(Dependency);
    }
}
