using Unity.Entities;
using Unity.Jobs;

public class CommandBuffers
{
    public EntityCommandBufferSystem UpdateSystem;
    public EntityCommandBufferSystem PostUpdateSystem;

    public EntityCommandBuffer CreatePostUpdateBuffer()
    {
        return PostUpdateSystem.CreateCommandBuffer();
    }

    public EntityCommandBuffer CreateUpdateBuffer()
    {
        return UpdateSystem.CreateCommandBuffer();
    }

    public CommandBuffers(World world)
    {
        UpdateSystem = world.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        PostUpdateSystem = world.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
    }

    public void AddUpdateDependency(JobHandle dependency)
    {
        UpdateSystem.AddJobHandleForProducer(dependency);
    }

    internal void AddPostUpdateDependency(JobHandle dependency)
    {
        PostUpdateSystem.AddJobHandleForProducer(dependency);
    }
}
