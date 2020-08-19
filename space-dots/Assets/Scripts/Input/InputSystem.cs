using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class InputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        MoveInputData data = GetSingleton<MoveInputData>();
        float2 inputDirection = data.InputDirection;
        
        // Anything with InputDrivenTag should have its MotionIntent filled out by this input data
        Entities
            .WithAll<InputDrivenTag>()
            .ForEach((ref MotionIntent intent) =>
            {
                intent.DirectionNormalized = math.normalizesafe(inputDirection);
                intent.ThrottleNormalized = math.clamp(math.length(inputDirection), 0f, 1f);
            }).Schedule();

        if (HasSingleton<FireInputData>())
        {
            FireInputData fireData = GetSingleton<FireInputData>();
            bool enable = fireData.IsFirePressed;

            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithAll<FireInputLinkTag>()
                .ForEach((ref Entity entity) =>
                {
                    if (enable)
                    {
                        EntityManager.AddComponent<ActiveTag>(entity);
                    }
                    else if (EntityManager.HasComponent<ActiveTag>(entity))
                    {
                        EntityManager.RemoveComponent<ActiveTag>(entity);
                    }
                }).Run();
            
            Entity inputEntity = GetSingletonEntity<FireInputData>();
            EntityManager.RemoveComponent<FireInputData>(inputEntity);
        }
    }
}
