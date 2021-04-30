using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private float moneyAmount;
    [SerializeField] private DataSaver dataSaver;
    [SerializeField] private MainCanves mainCanves;

    public float MoneyAmount
    {
        get => moneyAmount;
        private set => moneyAmount = value;
    }

    private void Awake()
    {
        if (PlayerPrefsManager.CheckForDataName(Constants.DataNames.MoneyAmount.ToString()))
        {
            MoneyAmount = PlayerPrefsManager.LoadFloat(Constants.DataNames.MoneyAmount.ToString());
        }
        else
        {
            MoneyAmount = MoneyAmount;
        }
    }

    private void Start()
    {
        mainCanves.UpdateMoneyTxt(MoneyAmount);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="moneyToIncreaseDecrease">this can be positive or nigative</param>
    public void ChangeMoneyAmount(float moneyToIncreaseDecrease)
    {
        MoneyAmount += moneyToIncreaseDecrease;
        mainCanves.UpdateMoneyTxt(MoneyAmount);
        dataSaver.AddDataToBeSaved(Constants.DataNames.MoneyAmount, MoneyAmount);
    }
}
