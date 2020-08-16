using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "Config/Root Config", order = 0)]
public class Config : ScriptableObject
{
    public PickupConfig[] Pickups;
}
