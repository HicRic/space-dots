﻿using System;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    private Entity inputDataEntity;

    private void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        inputDataEntity = entityManager.CreateEntity(typeof(MoveInputData));
        entityManager.SetName(inputDataEntity, "InputData");
    }

    private void OnDestroy()
    {
        if (World.DefaultGameObjectInjectionWorld?.EntityManager != null)
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            entityManager.DestroyEntity(inputDataEntity);
        }
    }

    public void OnMove(InputValue input)
    {
        Vector2 move = input.Get<Vector2>();
        tmp.SetText(move.ToString());
        
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        MoveInputData data = entityManager.GetComponentData<MoveInputData>(inputDataEntity);
        data.InputDirection = new float2(move.x, move.y);
        entityManager.SetComponentData(inputDataEntity, data);
    }

    public void OnFire(InputValue input)
    {
        tmp.SetText("fire!");
    }
}
