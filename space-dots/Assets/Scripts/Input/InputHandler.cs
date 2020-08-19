using System;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputAsset;
    
    private Entity inputDataEntity;
    private InputAction FireAction;

    private void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        inputDataEntity = entityManager.CreateEntity(typeof(MoveInputData), typeof(FireInputData));
#if UNITY_EDITOR
        entityManager.SetName(inputDataEntity, "InputData");
#endif

        FireAction = inputAsset.FindAction("Fire");
        FireAction.started += HandleFireStarted;
        FireAction.canceled += HandleFireCancelled;
    }

    private void HandleFireCancelled(InputAction.CallbackContext obj)
    {
        SetIsFiring(false);
    }

    private void HandleFireStarted(InputAction.CallbackContext obj)
    {
        SetIsFiring(true);
    }

    private void OnDestroy()
    {
        if (World.DefaultGameObjectInjectionWorld?.EntityManager != null)
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            entityManager.DestroyEntity(inputDataEntity);
        }

        FireAction.started -= HandleFireStarted;
        FireAction.canceled -= HandleFireCancelled;
    }

    public void OnMove(InputValue input)
    {
        Vector2 move = input.Get<Vector2>();
           
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        MoveInputData data = entityManager.GetComponentData<MoveInputData>(inputDataEntity);
        data.InputDirection = new float2(move.x, move.y);
        entityManager.SetComponentData(inputDataEntity, data);
    }
    
    void SetIsFiring(bool isFiring)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        FireInputData data = new FireInputData
        {
            IsFirePressed = isFiring
        };

        entityManager.AddComponentData(inputDataEntity, data);
    }
}
