using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver : MonoBehaviour
{
    private List<DataToBeSaved> dataToBeSaved = new List<DataToBeSaved>();

    public class DataToBeSaved
    {
        public Constants.DataNames DataName;
        public float DataValue;

        public DataToBeSaved(Constants.DataNames dataName, float dataValue)
        {
            DataName = dataName;
            DataValue = dataValue;
        }
    }

    public void AddDataToBeSaved(Constants.DataNames dataName, float dataValue)
    {
        foreach (DataToBeSaved data in dataToBeSaved)
        {
            if (data.DataName == dataName)
            {
                data.DataValue = dataValue;
                return;
            }
        }

        dataToBeSaved.Add(new DataToBeSaved(dataName, dataValue));
    }

    private void SaveData()
    {
        foreach (DataToBeSaved data in dataToBeSaved)
        {
            PlayerPrefsManager.SaveFloat(data.DataName.ToString(), data.DataValue);
        }

        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool pause)
    {
        SaveData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
}
