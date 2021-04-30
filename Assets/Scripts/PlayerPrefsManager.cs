using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager
{
    public static void SaveFloat(string dataName, float dataValue)
    {
        PlayerPrefs.SetFloat(dataName, dataValue);
    }

    public static void SaveInt(string dataName, int dataValue)
    {
        PlayerPrefs.SetInt(dataName, dataValue);
    }

    public static float LoadFloat(string dataName)
    {
        return PlayerPrefs.GetFloat(dataName);
    }

    public static int LoadInt(string dataName)
    {

        return PlayerPrefs.GetInt(dataName);
    }

    public static bool CheckForDataName(string dataName)
    {
        if (PlayerPrefs.HasKey(dataName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}