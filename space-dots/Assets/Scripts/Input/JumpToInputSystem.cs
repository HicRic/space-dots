using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class JumpToInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Vector2Control pos = Touchscreen.current?.primaryTouch?.position;
        if (pos != null)
        {
            Vector2 val = pos.ReadValue();
            SetPos(val);
        }

        if (Mouse.current?.leftButton != null && Mouse.current.position != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            SetPos(mousePos);
        }
    }

    private void SetPos(Vector2 pos)
    {
        Camera camera = Camera.main;
        if (!camera)
        {
            return;
        }

        Entities
            .ForEach((ref Translation translation, in JumpToInput jumpToInput) =>
            {
                Ray ray = camera.ScreenPointToRay(pos);
                if(Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    translation.Value = hitInfo.point;
                }
            })
            .WithoutBurst()
            .Run();
    }
}
