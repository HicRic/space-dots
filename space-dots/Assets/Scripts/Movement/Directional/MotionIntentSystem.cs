using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MotionIntentSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .ForEach((
            ref LinearAcceleration linearAcceleration,
            ref Rotation rotation,
            in LocalToWorld worldTransform,
            in MotionPowerLimits limits,
            in MotionIntent intent) =>
        {
            float throttle = math.lerp(0, limits.LinearAccelerationLimit, intent.ThrottleNormalized);
            linearAcceleration.Value = intent.DirectionNormalized * throttle;

            float2 facing = worldTransform.Up.xy;
            float desiredAngleDelta = math.acos(math.dot(facing, intent.DirectionNormalized));
            float frameAngleDelta = limits.AngularVelocityLimit * deltaTime;
            frameAngleDelta = math.min(frameAngleDelta, desiredAngleDelta);

            float3 crossProd = math.cross(new float3(facing, 0), new float3(intent.DirectionNormalized, 0));
            frameAngleDelta *= math.sign(crossProd.z);

            quaternion deltaQuaternion = quaternion.RotateZ(frameAngleDelta);
            rotation.Value = math.mul(rotation.Value, deltaQuaternion);

        }).Schedule();
    }
}
