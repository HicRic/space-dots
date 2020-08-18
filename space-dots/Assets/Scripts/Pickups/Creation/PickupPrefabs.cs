﻿using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PickupPrefabs : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
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