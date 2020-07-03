using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class FollowEntitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        ComponentDataFromEntity<Translation> translationData = GetComponentDataFromEntity<Translation>(true);
        Entities
            .WithReadOnly(translationData)
            .ForEach((ref TargetPosition targetPosition, in FollowEntity follow) =>
            {
                if (translationData.Exists(follow.Target))
                {
                    Translation target = translationData[follow.Target];
                    targetPosition.Value = target.Value;
                }
            }).Schedule();
    }
}
