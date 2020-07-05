using UnityEngine;
using System.Collections;

public class Startup
{
    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        Application.targetFrameRate = 300;
    }
}
