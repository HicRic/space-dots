using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PrefabBridge : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    [SerializeField] private Config Config = null;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        foreach (PickupConfig configPickup in Config.Pickups)
        {
            Entity ent = conversionSystem.GetPrimaryEntity(configPickup.Prefab);
            dstManager.AddComponentData(ent, new PickupTag());
            dstManager.AddComponentData(ent, configPickup.MoveSpeed);
            dstManager.AddComponentData(ent, new ConfigId { Value = configPickup.Id });

            if (configPickup.Lifespan.Value > 0)
            {
                dstManager.AddComponentData(ent, configPickup.Lifespan);
            }
        }

        foreach (ProjectileConfig projectileConfig in Config.Projectiles)
        {
            Entity ent = conversionSystem.GetPrimaryEntity(projectileConfig.Prefab);
            dstManager.AddComponentData(ent, new ProjectileTag());
            dstManager.AddComponentData(ent, projectileConfig.SpawnSpeed);
            dstManager.AddComponentData(ent, new ConfigId { Value = projectileConfig.Id });
            dstManager.AddComponent<PreviousTranslation>(ent);
            dstManager.AddComponentData(ent, projectileConfig.Damager);

            if (projectileConfig.Lifespan.Value > 0)
            {
                dstManager.AddComponentData(ent, projectileConfig.Lifespan);
            }
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (PickupConfig pickup in Config.Pickups)
        {
            referencedPrefabs.Add(pickup.Prefab);
        }

        foreach (ProjectileConfig projectile in Config.Projectiles)
        {
            referencedPrefabs.Add(projectile.Prefab);
        }
    }
}