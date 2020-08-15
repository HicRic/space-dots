using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class LinearVelocitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        float delta = Time.DeltaTime;
        Entities.ForEach((ref Translation translation, in LinearVelocity velocity) =>
        {
            float2 moveDelta = velocity.Value * delta;
            translation.Value += new float3(moveDelta, 0);
        }).Schedule();
    }
}
