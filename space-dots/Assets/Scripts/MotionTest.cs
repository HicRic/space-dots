using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTest : MonoBehaviour
{
    [SerializeField]
    private Transform fixedMotion;
    [SerializeField]
    private Transform deltaTimeMotion;
    [SerializeField]
    private Transform vsyncRoundedDeltaTimeMotion;

    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private float startX = -10f;

    [SerializeField]
    private float length = 20f;
    
    private int screenRefreshHz;

    void Start()
    {
        screenRefreshHz = Screen.currentResolution.refreshRate;

        SetX(fixedMotion, startX);
        SetX(deltaTimeMotion, startX);
        SetX(vsyncRoundedDeltaTimeMotion, startX);
    }

    void SetX(Transform transform, float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    void MoveX(Transform transform, float x)
    {
        transform.position = new Vector3(transform.position.x + x, transform.position.y, transform.position.z);
    }
    
    void Update()
    {
        // wait a second for any hiccups to pass
        if (Time.timeSinceLevelLoad < 1f)
        {
            return;
        }

        float fixedStep = speed / screenRefreshHz;
        
        float deltaStep = Time.deltaTime * speed;
        
        float interval = 1f / screenRefreshHz;

        // How many fractional intervals have passed, according to delta time?
        float deltaIntervalFractions = Time.deltaTime / interval;
        int deltaIntervals = 0;

        // In reality, it is impossible for less then 1 interval to have passed, due to vsync.
        // Ensure the value is min 1.
        if (deltaIntervalFractions < 1f)
        {
            deltaIntervals = 1;
        }
        else
        {
            // Otherwise, floor the fractional intervals and see how that looks.
            // I suspect we need more of a fudge...1.99999 intervals is probably 2 but we'll send it to 1. but 1.6 intervals? probably 1...
            deltaIntervals = Mathf.FloorToInt(deltaIntervalFractions);
        }

        float roundedDeltaStep = deltaIntervals * interval * speed;

        MoveX(fixedMotion, fixedStep);
        MoveX(deltaTimeMotion, deltaStep);
        MoveX(vsyncRoundedDeltaTimeMotion, roundedDeltaStep);

        if (fixedMotion.position.x >= length + startX)
        {
            MoveX(fixedMotion, -length);
        }

        if (deltaTimeMotion.position.x >= length + startX)
        {
            MoveX(deltaTimeMotion, -length);
        }

        if (vsyncRoundedDeltaTimeMotion.position.x >= length + startX)
        {
            MoveX(vsyncRoundedDeltaTimeMotion, -length);
        }
    }
}
