using Unity.Entities;
using Unity.Mathematics;

public class LinearAccelerationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float time = Time.DeltaTime;
        
        Entities.ForEach((ref LinearVelocity velocity, in LinearAcceleration acceleration) =>
        {
            velocity.Value += acceleration.Value * time;
        }).Schedule();

        Entities.ForEach((ref LinearVelocity velocity, in LinearAcceleration acceleration, in MotionPowerLimits limits) =>
        {
            float2 newVelocity = velocity.Value + (acceleration.Value * time);
            if (math.lengthsq(newVelocity) > limits.LinearVelocityLimit * limits.LinearVelocityLimit)
            {
                newVelocity = math.normalize(newVelocity) * limits.LinearVelocityLimit;
            }
            velocity.Value = newVelocity;
        }).Schedule();
    }
}
