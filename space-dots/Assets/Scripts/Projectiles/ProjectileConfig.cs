using System;
using UnityEngine;

[Serializable]
public class ProjectileConfig
{
    public uint Id;
    public GameObject Prefab;
    public SpawnSpeed SpawnSpeed;
    public Lifespan Lifespan;
}
