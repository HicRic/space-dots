using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class MoverSpawnSystem : SystemBase
{
    private EntityCommandBufferSystem ecbs;
    private EntityQuery spawnQuery;
    private EntityQuery prefabsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        ecbs = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();

        RequireForUpdate(spawnQuery);

        EntityQueryDesc desc = new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<ConfigId>()
            },
            Options = EntityQueryOptions.IncludePrefab
        };

        prefabsQuery = GetEntityQuery(desc);
        RequireForUpdate(prefabsQuery);
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();

        NativeArray<Entity> prefabEntities = prefabsQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<ConfigId> configIds = prefabsQuery.ToComponentDataArray<ConfigId>(Allocator.TempJob);

        JobHandle outDepends = Entities
            .WithStoreEntityQueryInField(ref spawnQuery)
            .WithDisposeOnCompletion(prefabEntities)
            .WithDisposeOnCompletion(configIds)
            .ForEach((ref Entity entity, in MoverSpawnRequest spawn) =>
            {
                for (var i = 0; i < configIds.Length; i++)
                {
                    ConfigId configId = configIds[i];
                    if (configId.Value == spawn.ConfigId)
                    {
                        Entity newMoverEntity = ecb.Instantiate(prefabEntities[i]);
                        ecb.AddComponent(newMoverEntity, new Translation { Value = spawn.Position });
                        ecb.AddComponent(newMoverEntity, new LinearVelocity { Value = spawn.Velocity});
                        return;
                    }
                }
            }).Schedule(Dependency);

        ecb.DestroyEntity(spawnQuery);

        ecbs.AddJobHandleForProducer(outDepends);
        Dependency = outDepends;
    }
}
