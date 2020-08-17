using Unity.Entities;
using Unity.Jobs;

public class PickupCollectSystem : SystemBase
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

        ComponentDataFromEntity<CurrencyIron> ironComps = GetComponentDataFromEntity<CurrencyIron>();
        ComponentDataFromEntity<CurrencyXP> xpComps = GetComponentDataFromEntity<CurrencyXP>();

        JobHandle outputDepend = Entities
            .WithAll<CurrencyXP>()
            .ForEach((Entity entity, in PickupCollect pickupCollect) =>
            {
                CurrencyXP awardXp = xpComps[entity];

                if (xpComps.HasComponent(pickupCollect.Target))
                {
                    CurrencyXP targetXp = xpComps[pickupCollect.Target];
                    targetXp.Amount += awardXp.Amount;
                    xpComps[pickupCollect.Target] = targetXp;
                }
                else
                {
                    ecb.AddComponent(pickupCollect.Target, awardXp);
                }

                ecb.DestroyEntity(entity);

                // todo could also crate a feedback entity here to show XP gain

            }).Schedule(Dependency);

        ecbs.AddJobHandleForProducer(outputDepend);
        Dependency = outputDepend;
    }
}
