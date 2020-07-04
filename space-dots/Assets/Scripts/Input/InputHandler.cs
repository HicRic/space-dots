using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    public void OnMove(InputValue input)
    {
        Vector2 move = input.Get<Vector2>();
        tmp.SetText(move.ToString());
        Debug.Log("move: " + move);
    }

    public void OnFire(InputValue input)
    {
        tmp.SetText("fire!");
        Debug.Log("fire!");
    }
}
