using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "Config/Root Config", order = 0)]
public class Config : ScriptableObject
{
    public PickupConfig[] Pickups;
    public ProjectileConfig[] Projectiles;

    public void WriteUniqueIds()
    {
        uint largestId = 0;

        foreach (PickupConfig pickup in Pickups)
        {
            largestId = math.max(largestId, pickup.Id); 
        }

        foreach (ProjectileConfig projectile in Projectiles)
        {
            largestId = math.max(largestId, projectile.Id);
        }

        uint nextFreeId = largestId + 1;

        HashSet<uint> ids = new HashSet<uint>();
        
        // consider 0 invalid
        ids.Add(0);

        foreach (PickupConfig pickup in Pickups)
        {
            if (ids.Contains(pickup.Id))
            {
                pickup.Id = nextFreeId;
                ids.Add(pickup.Id);
                ++nextFreeId;
            }
        }

        foreach (ProjectileConfig projectile in Projectiles)
        {
            if (ids.Contains(projectile.Id))
            {
                projectile.Id = nextFreeId;
                ids.Add(projectile.Id);
                ++nextFreeId;
            }
        }
    }
}
