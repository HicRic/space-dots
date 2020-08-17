using System;
using TMPro;
using Unity.Entities;

[GenerateAuthoringComponent]
public class HudBinding_CurrencyXP : IComponentData
{
    public TextMeshProUGUI Text;
    public int SetXpValue;
}

