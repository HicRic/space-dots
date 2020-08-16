using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class PickupSpawnSystem : SystemBase
{
    private EntityCommandBufferSystem ecbs;
    private EntityQuery spawnEntityQuery;
    private EntityQuery pickupPrefabsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        ecbs = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();

        RequireForUpdate(spawnEntityQuery);

        EntityQueryDesc desc = new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<PickupTag>(),
                ComponentType.ReadOnly<ConfigId>()
            },
            Options = EntityQueryOptions.IncludePrefab
        };

        pickupPrefabsQuery = GetEntityQuery(desc);
        RequireForUpdate(pickupPrefabsQuery);
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();

        NativeArray<Entity> prefabEntities = pickupPrefabsQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<ConfigId> configIds = pickupPrefabsQuery.ToComponentDataArray<ConfigId>(Allocator.TempJob);

        JobHandle outDepends = Entities
            .WithStoreEntityQueryInField(ref spawnEntityQuery)
            .WithDisposeOnCompletion(prefabEntities)
            .WithDisposeOnCompletion(configIds)
            .ForEach((ref Entity entity, in PickupSpawn spawn) =>
            {
                for (var i = 0; i < configIds.Length; i++)
                {
                    ConfigId configId = configIds[i];
                    if (configId.Value == spawn.PickupId)
                    {
                        Entity newPickupEntity = ecb.Instantiate(prefabEntities[i]);
                        ecb.AddComponent(newPickupEntity, new Translation { Value = spawn.Position });
                        return;
                    }
                }
            }).Schedule(Dependency);

        ecb.DestroyEntity(spawnEntityQuery);

        ecbs.AddJobHandleForProducer(outDepends);
        Dependency = outDepends;
    }
}
