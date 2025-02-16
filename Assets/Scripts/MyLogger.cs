using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyLogger
{
    public static void Log(string args)
    {
#if UNITY_EDITOR
        Debug.Log(args);
#endif
    }
}
