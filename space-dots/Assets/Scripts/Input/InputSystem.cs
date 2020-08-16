using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class InputSystem : SystemBase
{
    private EntityQuery inputQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        inputQuery = GetEntityQuery(ComponentType.ReadOnly<MoveInputData>());
    }

    protected override void OnUpdate()
    {
        // For now, just assume all MoveInputData should be smushed together to create a single value for this frame.
        // We'll probably only have one element in this array anyway.
        NativeArray<MoveInputData> inputData = inputQuery.ToComponentDataArray<MoveInputData>(Allocator.Temp);

        float2 inputDirection = new float2();
        foreach (MoveInputData data in inputData)
        {
            inputDirection += data.InputDirection;
        }

        inputData.Dispose();

        // Anything with InputDrivenTag should have its MotionIntent filled out by this input data
        Entities
            .WithAll<InputDrivenTag>()
            .ForEach((ref MotionIntent intent) =>
            {
                intent.DirectionNormalized = math.normalizesafe(inputDirection);
                intent.ThrottleNormalized = math.clamp(math.length(inputDirection), 0f, 1f);
            }).Schedule();
    }
}
