using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class ProjectileCollisionSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecbs;
    BuildPhysicsWorld buildPhysicsWorld;
    EndFramePhysicsSystem endFramePhysicsSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
    }

    protected override void OnUpdate()
    {
        CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();
        Dependency = JobHandle.CombineDependencies(Dependency, endFramePhysicsSystem.GetOutputDependency());


        Entities
            .WithAll<ProjectileTag>()
            .ForEach((Entity entity, in Translation translation) =>
            {
                NativeList<int> allHits = new NativeList<int>(Allocator.Temp);

                // we ought to store projectile's old position, and raycast old->new to determine hit
                // gonna try this first and see what happens though

                float3 min = new float3(-0.01f, -0.01f, -0.01f) + translation.Value;
                float3 max = new float3(0.01f, 0.01f, 0.01f) + translation.Value;

                bool hit = collisionWorld.OverlapAabb(new OverlapAabbInput
                {
                    Aabb = new Aabb {Max = max.xyz, Min = min.xyz},
                    Filter = CollisionFilter.Default
                }, 
                ref allHits);

                if (hit)
                {
                    ecb.DestroyEntity(entity);
                }

                allHits.Dispose();

            }).Schedule();

        ecbs.AddJobHandleForProducer(Dependency);
    }
}
