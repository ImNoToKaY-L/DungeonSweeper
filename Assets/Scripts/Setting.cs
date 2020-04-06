using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting :MonoBehaviour
{
    public static bool DebugTracer = true;

    public void DebugTracerOn()
    {
        DebugTracer = true;
    }
    public void DebugTracerOff()
    {
        DebugTracer = false;
    }

}
