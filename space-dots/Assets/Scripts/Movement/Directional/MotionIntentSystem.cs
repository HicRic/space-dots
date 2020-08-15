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
            in MotionPowerLimits limits,
            in MotionIntent intent) =>
        {
            float throttle = math.lerp(0, limits.LinearAccelerationLimit, intent.ThrottleNormalized);

            float2 facing = math.normalize(math.rotate(rotation.Value, new float3(0f,1f,0f)).xy);
            linearAcceleration.Value = facing * throttle;

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
