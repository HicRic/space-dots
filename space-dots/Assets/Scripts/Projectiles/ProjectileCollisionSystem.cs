using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

// We query the physics system, so we need to update after it.
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class ProjectileCollisionSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem endSimECBS;
    
    private BuildPhysicsWorld buildPhysicsWorld;
    private EndFramePhysicsSystem endFramePhysicsSystem;

    private EntityArchetype damageInstanceArchetype;
    private EntityQuery damageInstanceQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        endSimECBS = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();

        damageInstanceArchetype = EntityManager.CreateArchetype(typeof(DamageInstance), typeof(Translation));
        damageInstanceQuery = GetEntityQuery(typeof(DamageInstance));
    }

    protected override void OnUpdate()
    {
        EntityArchetype dmgArchetype = damageInstanceArchetype;
        EntityCommandBuffer endSimECB = endSimECBS.CreateCommandBuffer();
        CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;

        // Clean up any previous DamageInstance entities before we make new ones
        endSimECB.DestroyEntity(damageInstanceQuery);

        // We want to query the physics world, so we need to make sure all physics jobs have finished.
        // To do this, we add the EndFramePhysicsSystem's output job handle as a dependency.
        Dependency = JobHandle.CombineDependencies(Dependency, endFramePhysicsSystem.GetOutputDependency());

        Entities
            .WithAll<ProjectileTag>()
            .ForEach((Entity projectileEntity, in Translation translation, in PreviousTranslation previousTranslation, in Damager damager) =>
            {
                // Not sure what a great initial capacity is here.
                // It seems pretty unlikely that a ray will intersect with > 4 things, so I'll go with that.
                NativeList<int> allHits = new NativeList<int>(4, Allocator.Temp);

                bool hit = collisionWorld.CastRay(new RaycastInput
                {
                    Start = previousTranslation.Value,
                    End = translation.Value,
                    Filter = CollisionFilter.Default // Might need to use filter to do friend/foe filtering later
                }, out RaycastHit closestHit);

                if (hit)
                {
                    endSimECB.DestroyEntity(projectileEntity);
                    
                    Entity damageInstanceEntity = endSimECB.CreateEntity(dmgArchetype);
                    endSimECB.SetComponent(damageInstanceEntity, new DamageInstance
                    {
                        DamageAmount = damager.Amount,
                        DamagedEntity = closestHit.Entity
                    });
                    endSimECB.SetComponent(damageInstanceEntity, new Translation { Value = closestHit.Position });
                }

                allHits.Dispose();

            }).Schedule();

        endSimECBS.AddJobHandleForProducer(Dependency);
    }
}
