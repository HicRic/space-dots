using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ProjectilePrefabs : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    [SerializeField] private Config Config = null;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        foreach (ProjectileConfig config in Config.Projectiles)
        {
            Entity ent = conversionSystem.GetPrimaryEntity(config.Prefab);
            dstManager.AddComponentData(ent, new ProjectileTag());
            dstManager.AddComponentData(ent, config.SpawnSpeed);
            dstManager.AddComponentData(ent, new ConfigId { Value = config.Id });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (PickupConfig pickup in Config.Pickups)
        {
            referencedPrefabs.Add(pickup.Prefab);
        }
    }
}
