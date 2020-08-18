using Unity.Entities;
using Unity.Jobs;

public class PickupCollectSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecbs;
    private EntityQuery collectablesQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        collectablesQuery = GetEntityQuery(typeof(PickupCollect));
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();

        ComponentDataFromEntity<CurrencyIron> ironComps = GetComponentDataFromEntity<CurrencyIron>(true);
        ComponentDataFromEntity<CurrencyXP> xpComps = GetComponentDataFromEntity<CurrencyXP>(true);

        JobHandle outputDepend = Entities
            .WithAll<CurrencyXP>()
            .WithReadOnly(xpComps)
            .ForEach((Entity entity, in PickupCollect pickupCollect) =>
            {
                CurrencyXP awardXp = xpComps[entity];

                if (xpComps.HasComponent(pickupCollect.Target))
                {
                    CurrencyXP targetXp = xpComps[pickupCollect.Target];
                    targetXp.Amount += awardXp.Amount;
                    ecb.SetComponent(pickupCollect.Target, targetXp);
                }
                else
                {
                    ecb.AddComponent(pickupCollect.Target, awardXp);
                }

                // todo could also crate a feedback entity here to show XP gain

            }).Schedule(Dependency);
        
        ecb.DestroyEntity(collectablesQuery);
        ecbs.AddJobHandleForProducer(outputDepend);
        Dependency = outputDepend;
    }
}
