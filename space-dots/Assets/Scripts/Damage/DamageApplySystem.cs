using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class DamageApplySystem : SystemBase
{
    private EntityQuery damageInstanceQuery;
    private EndSimulationEntityCommandBufferSystem ecbs;

    protected override void OnCreate()
    {
        base.OnCreate();
        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();

        // Gather all damage done to an entity in the hash map.
        // This helps us avoid issues where an entity has suffered multiple DamageInstances in a single frame.
        NativeHashMap<Entity, int> damageAccumulator = new NativeHashMap<Entity, int>(damageInstanceQuery.CalculateEntityCount(), Allocator.TempJob);


        // First, we run an accumulation job over all DamageInstances to gather all damage done to each entity.
        // After it has run, damageAccumulator will contain the total damage to apply to each entity.
        // e.g. With 3 DamageInstances applying 3, 2 and 1 damage to the same Entity, damageAccumulator will have
        // exactly one key for that Entity, with value 3+2+1=6.

        JobHandle accumulateJob = Entities
            .WithStoreEntityQueryInField(ref damageInstanceQuery)
            .ForEach((in DamageInstance damageInstance) =>
            {
                damageAccumulator.TryGetValue(damageInstance.DamagedEntity, out int damage);
                damageAccumulator[damageInstance.DamagedEntity] = damage + damageInstance.DamageAmount;
            }).Schedule(Dependency);

        ComponentDataFromEntity<DamageTaken> damageTakenComponents = GetComponentDataFromEntity<DamageTaken>();

        // Then we run a job with a dependency on that accumulateJob.
        // This job iterates over the damageAccumulator map and writes the damage to the entities.
        // Since we don't want to iterate over entities here, we use Job.WithCode instead.
        JobHandle applyDamageJob = Job.WithCode(() =>
        {
            NativeKeyValueArrays<Entity, int> entityDamagePairs = damageAccumulator.GetKeyValueArrays(Allocator.Temp);

            for (int i = 0; i < entityDamagePairs.Length; ++i)
            {
                Entity entity = entityDamagePairs.Keys[i];
                int damageToApply = entityDamagePairs.Values[i];

                // Since we know each entity appears exactly once in the damageAccumulator,
                // we can test for presence of the DamageTaken component and queue up a command in the ECB,
                // to either modify it or add it.
                if (damageTakenComponents.HasComponent(entity))
                {
                    DamageTaken dmg = damageTakenComponents[entity];
                    dmg.Value += damageToApply;
                    ecb.SetComponent(entity, dmg);
                }
                else
                {
                    ecb.AddComponent(entity, new DamageTaken { Value = damageToApply });
                }
            }

            entityDamagePairs.Dispose();

        }).Schedule(accumulateJob);

        // We can dispose of the map when the applyDamageJob has completed.
        damageAccumulator.Dispose(applyDamageJob);

        // applyDamageJob produces ECB commands, so the ECBS must depend on it.
        ecbs.AddJobHandleForProducer(applyDamageJob);

        // the system has completed when applyDamageJob has completed.
        Dependency = applyDamageJob;
    }
}
