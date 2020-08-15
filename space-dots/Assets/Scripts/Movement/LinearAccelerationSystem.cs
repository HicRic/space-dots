using Unity.Entities;

public class LinearAccelerationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float time = Time.DeltaTime;
        Entities.ForEach((ref LinearVelocity velocity, in LinearAcceleration acceleration) =>
        {
            velocity.Value += acceleration.Value * time;
        }).Schedule();
    }
}
