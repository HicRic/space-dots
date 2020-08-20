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
    private EntityArchetype damageInstanceArchetype;

    protected override void OnCreate()
    {
        base.OnCreate();
        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
        damageInstanceArchetype = EntityManager.CreateArchetype(typeof(DamageInstance), typeof(Translation));
    }

    protected override void OnUpdate()
    {
        CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();
        Dependency = JobHandle.CombineDependencies(Dependency, endFramePhysicsSystem.GetOutputDependency());
        EntityArchetype dmgArchetype = damageInstanceArchetype;

        Entities
            .WithAll<ProjectileTag>()
            .ForEach((Entity projectileEntity, in Translation translation, in PreviousTranslation previousTranslation, in Damager damager) =>
            {
                NativeList<int> allHits = new NativeList<int>(Allocator.Temp);

                bool hit = collisionWorld.CastRay(new RaycastInput
                {
                    Start = previousTranslation.Value,
                    End = translation.Value,
                    Filter = CollisionFilter.Default
                }, out RaycastHit closestHit);

                if (hit)
                {
                    ecb.DestroyEntity(projectileEntity);

                    Entity damageInstanceEntity = ecb.CreateEntity(dmgArchetype);
                    ecb.SetComponent(damageInstanceEntity, new DamageInstance
                    {
                        DamageAmount = damager.Amount,
                        DamagedEntity = closestHit.Entity
                    });
                    ecb.SetComponent(damageInstanceEntity, new Translation { Value = translation.Value });
                }

                allHits.Dispose();

            }).Schedule();

        ecbs.AddJobHandleForProducer(Dependency);
    }
}
