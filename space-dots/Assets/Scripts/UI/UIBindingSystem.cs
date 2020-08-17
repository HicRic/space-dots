using Unity.Entities;

public class UIBindingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .ForEach((HudBinding_CurrencyXP currencyBinding) =>
            {
                if (HasSingleton<HudObservedTag>())
                {
                    Entity hudObservedEntity = GetSingletonEntity<HudObservedTag>();
                    if (EntityManager.HasComponent<CurrencyXP>(hudObservedEntity))
                    {
                        CurrencyXP targetXP = EntityManager.GetComponentData<CurrencyXP>(hudObservedEntity);
                        if (currencyBinding.SetXpValue != targetXP.Amount)
                        {
                            currencyBinding.SetXpValue = targetXP.Amount;
                            currencyBinding.Text.SetText(targetXP.Amount.ToString());
                        }
                    }
                }
            }).Run();
    }
}
