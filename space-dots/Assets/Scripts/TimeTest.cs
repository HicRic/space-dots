using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeTest : MonoBehaviour
{
    [SerializeField]
    private Toggle updateTextToggle;

    [SerializeField]
    private TextMeshProUGUI unityDeltaFps;

    [SerializeField]
    private TextMeshProUGUI unityFrameTimeMs;

    [SerializeField]
    private TextMeshProUGUI stopwatchFrameTimeMs;

    [SerializeField]
    private TextMeshProUGUI unitySinceStartTxt;

    [SerializeField]
    private TextMeshProUGUI stopwatchSinceStartText;

    [SerializeField]
    private TextMeshProUGUI unityDeltaSumTxt;

    [SerializeField]
    private TextMeshProUGUI stopwatchDeltaSumText;

    private Stopwatch stopwatch;
    private Stopwatch stopwatchSinceStart;
    private float unityDeltaSum;
    private double stopwatchDeltaSum;
    private StringBuilder sb;

    // Start is called before the first frame update
    void Start()
    {
        sb = new StringBuilder(100);
        stopwatch = new Stopwatch();
        stopwatchSinceStart = new Stopwatch();
        stopwatchSinceStart.Start();
    }

    // Update is called once per frame
    void Update()
    {
        float unityDelta = Time.deltaTime;
        double stopwatchDelta = stopwatch.Elapsed.TotalSeconds;
        unityDeltaSum += unityDelta;
        stopwatchDeltaSum += stopwatchDelta;
        stopwatch.Restart();


        if (!updateTextToggle.isOn)
        {
            return;
        }


        // TODO apparenetly this stuff still allocs, argh
        sb.Clear();
        sb.Append(unityDelta * 1000f);
        unityFrameTimeMs.SetText(sb);

        sb.Clear();
        sb.Append(stopwatchDelta * 1000f);
        stopwatchFrameTimeMs.SetText(sb);

        sb.Clear();
        sb.Append(1f / unityDelta);
        unityDeltaFps.SetText(sb);

        sb.Clear();
        sb.Append(stopwatchSinceStart.Elapsed.TotalSeconds);
        stopwatchSinceStartText.SetText(sb);
        
        sb.Clear();
        sb.Append(Time.timeSinceLevelLoad);
        unitySinceStartTxt.SetText(sb);

        sb.Clear();
        sb.Append(unityDeltaSum);
        unityDeltaSumTxt.SetText(sb);

        sb.Clear();
        sb.Append(stopwatchDeltaSum);
        stopwatchDeltaSumText.SetText(sb);

    }
}
