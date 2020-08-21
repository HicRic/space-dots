using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class InputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        MoveInputData data = GetSingleton<MoveInputData>();
        float2 moveDirection = data.InputDirection;
        
        // Anything with MoveInputDrivenTag should have its MotionIntent filled out by this input data
        Entities
            .WithAll<MoveInputDrivenTag>()
            .ForEach((ref MotionIntent intent) =>
            {
                intent.DirectionNormalized = math.normalizesafe(moveDirection);
                intent.ThrottleNormalized = math.clamp(math.length(moveDirection), 0f, 1f);
            }).Schedule();

        AimInputData aimData = GetSingleton<AimInputData>();
        float2 aimDirection = aimData.InputDirection;

        // AimInputDrivenTag entities get their motion intent set by that input data too.
        Entities
            .WithAll<AimInputDrivenTag>()
            .ForEach((ref MotionIntent intent) =>
            {
                intent.DirectionNormalized = math.normalizesafe(aimDirection);
                intent.ThrottleNormalized = 1f;
            }).Schedule();

        //if (HasSingleton<FireInputData>())
        //{
        //    FireInputData fireData = GetSingleton<FireInputData>();
        //    bool enable = fireData.IsFirePressed;

        //    Entities
        //        .WithoutBurst()
        //        .WithStructuralChanges()
        //        .WithAll<FireInputLinkTag>()
        //        .ForEach((ref Entity entity) =>
        //        {
        //            if (enable)
        //            {
        //                EntityManager.AddComponent<ActiveTag>(entity);
        //            }
        //            else if (EntityManager.HasComponent<ActiveTag>(entity))
        //            {
        //                EntityManager.RemoveComponent<ActiveTag>(entity);
        //            }
        //        }).Run();
            
        //    Entity inputEntity = GetSingletonEntity<FireInputData>();
        //    EntityManager.RemoveComponent<FireInputData>(inputEntity);
        //}
    }
}
