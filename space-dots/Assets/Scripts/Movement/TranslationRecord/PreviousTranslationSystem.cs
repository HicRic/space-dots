using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class PreviousTranslationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PreviousTranslation previous, in Translation current) =>
        {
            previous.Value = current.Value;
        }).Schedule();
    }
}
