using System;
using UnityEngine;
using Athana.Api;

public class AthanaLogger
{
    private static string TAG = "Athana Unity";

    public static void D(string message)
    {
        if (!AthanaInterface.DebugMode)
        {
            return;
        }
        Debug.Log("Debug [" + TAG + "] " + message);
    }

    public static void W(string message)
    {
        Debug.LogWarning("Warn [" + TAG + "] " + message);
    }

    public static void E(string message)
    {
        Debug.LogWarning("Error [" + TAG + "] " + message);
    }

    public static void LogException(Exception exception)
    {
        Debug.LogException(exception);
    }
}