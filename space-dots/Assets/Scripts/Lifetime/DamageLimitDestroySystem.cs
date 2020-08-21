using Unity.Entities;

public class DamageLimitDestroySystem : SystemBase
{
    private CommandBuffers buffers;

    protected override void OnCreate()
    {
        base.OnCreate();

        buffers = new CommandBuffers(World);
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer destroyECB = buffers.CreatePostUpdateBuffer();
        EntityCommandBuffer effectECB = buffers.CreateUpdateBuffer();

        Entities
            .ForEach((Entity entity, in DamageLimit damageLimit, in DamageTaken damageTaken) =>
        {
            bool destroy = damageTaken.Value >= damageLimit.Value;
            if (destroy)
            {
                effectECB.AddComponent<DyingTag>(entity);
                destroyECB.DestroyEntity(entity);
            }
        }).Schedule();

        buffers.AddUpdateDependency(Dependency);
        buffers.AddPostUpdateDependency(Dependency);
    }
}
