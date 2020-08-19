using Unity.Entities;
using Unity.Jobs;

public class LifespanSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float time = (float)Time.ElapsedTime;

        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .WithNone<TimedDestroy>()
            .ForEach((ref Entity entity, in Lifespan lifespan) =>
            {
                EntityManager.AddComponentData(entity, new TimedDestroy
                {
                    DestroyTime = time + lifespan.Value
                });
            }).Run();
    }
}
