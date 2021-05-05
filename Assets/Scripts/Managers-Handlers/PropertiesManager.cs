using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesManager : MonoBehaviour
{
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private DataSaver dataSaver;
    [SerializeField] private Propertie healthPropertie;
    [SerializeField] private Propertie humansNumPropertie;
    [SerializeField] private Propertie projectilesNumPropertie;
    [SerializeField] private Propertie runAfterSpeedPropertie;

    [System.Serializable]
    public struct Propertie
    {
        public Constants.PropertiesTypes PropertieType;
        public float InitialValue;
        public float Value;
        public float ValueIncreaser;
        public float InitialCost;
        public float Cost;
        public float CostMultiplayer;
    }

    private void Awake()
    {
        LoadPropertiesData();
    }

    private void Start()
    {
        EventsManager.onLevelStart += ResetForNewLevel;
    }

    public void LoadPropertiesData()
    {
        if (PlayerPrefsManager.CheckForDataName(Constants.DataNames.Health.ToString()))
        {
            healthPropertie.Value = PlayerPrefsManager.LoadFloat(Constants.DataNames.Health.ToString());
        }

        if (PlayerPrefsManager.CheckForDataName(Constants.DataNames.RunAfterSpeed.ToString()))
        {
            runAfterSpeedPropertie.Value = PlayerPrefsManager.LoadFloat(Constants.DataNames.RunAfterSpeed.ToString());
        }

        if (PlayerPrefsManager.CheckForDataName(Constants.DataNames.ProjectilesNum.ToString()))
        {
            projectilesNumPropertie.Value = PlayerPrefsManager.LoadFloat(Constants.DataNames.ProjectilesNum.ToString());
        }

        if (PlayerPrefsManager.CheckForDataName(Constants.DataNames.HumansNum.ToString()))
        {
            humansNumPropertie.Value = PlayerPrefsManager.LoadFloat(Constants.DataNames.HumansNum.ToString());
        }

        if (PlayerPrefsManager.CheckForDataName(Constants.DataNames.HealthCost.ToString()))
        {
            healthPropertie.Cost = PlayerPrefsManager.LoadFloat(Constants.DataNames.HealthCost.ToString());
        }

        if (PlayerPrefsManager.CheckForDataName(Constants.DataNames.RunAfterSpeedCost.ToString()))
        {
            runAfterSpeedPropertie.Cost = PlayerPrefsManager.LoadFloat(Constants.DataNames.RunAfterSpeedCost.ToString());
        }

        if (PlayerPrefsManager.CheckForDataName(Constants.DataNames.ProjectilesNumCost.ToString()))
        {
            projectilesNumPropertie.Cost = PlayerPrefsManager.LoadFloat(Constants.DataNames.ProjectilesNumCost.ToString());
        }

        if (PlayerPrefsManager.CheckForDataName(Constants.DataNames.HumansNumCost.ToString()))
        {
            humansNumPropertie.Cost = PlayerPrefsManager.LoadFloat(Constants.DataNames.HumansNumCost.ToString());
        }
    }

    public bool TryUpdateHealthPropertie()
    {
        if (moneyManager.MoneyAmount >= healthPropertie.Cost)
        {
            moneyManager.ChangeMoneyAmount(-healthPropertie.Cost);
            healthPropertie.Value += healthPropertie.ValueIncreaser;
            healthPropertie.Cost *= healthPropertie.CostMultiplayer;
            EventsManager.OnHealthPropertieUpdate(healthPropertie.Value, false);
            dataSaver.AddDataToBeSaved(Constants.DataNames.Health, healthPropertie.Value);
            dataSaver.AddDataToBeSaved(Constants.DataNames.HealthCost, healthPropertie.Cost);
            return true;
        }

        return false;
    }

    public bool TryUpdateRunAfterPropertie()
    {
        if (moneyManager.MoneyAmount >= runAfterSpeedPropertie.Cost)
        {
            moneyManager.ChangeMoneyAmount(-runAfterSpeedPropertie.Cost);
            runAfterSpeedPropertie.Value += runAfterSpeedPropertie.ValueIncreaser;
            runAfterSpeedPropertie.Cost *= runAfterSpeedPropertie.CostMultiplayer;
            EventsManager.OnRunAfterSpeedUpdate(runAfterSpeedPropertie.Value, false);
            dataSaver.AddDataToBeSaved(Constants.DataNames.RunAfterSpeed, runAfterSpeedPropertie.Value);
            dataSaver.AddDataToBeSaved(Constants.DataNames.RunAfterSpeedCost, runAfterSpeedPropertie.Cost);
            return true;
        }

        return false;
    }

    public bool TryUpdateProjectilesNumPropertie()
    {
        if (moneyManager.MoneyAmount >= projectilesNumPropertie.Cost)
        {
            moneyManager.ChangeMoneyAmount(-projectilesNumPropertie.Cost);
            projectilesNumPropertie.Value += projectilesNumPropertie.ValueIncreaser;
            projectilesNumPropertie.Cost *= projectilesNumPropertie.CostMultiplayer;
            EventsManager.OnProjectilesNumPropertieUpdate((int)projectilesNumPropertie.Value, false);
            dataSaver.AddDataToBeSaved(Constants.DataNames.ProjectilesNum, projectilesNumPropertie.Value);
            dataSaver.AddDataToBeSaved(Constants.DataNames.ProjectilesNumCost, projectilesNumPropertie.Cost);
            return true;
        }

        return false;
    }

    public bool TryUpdateHumansNumPropertie()
    {
        if (moneyManager.MoneyAmount >= humansNumPropertie.Cost)
        {
            moneyManager.ChangeMoneyAmount(-humansNumPropertie.Cost);
            humansNumPropertie.Value += humansNumPropertie.ValueIncreaser;
            humansNumPropertie.Cost *= humansNumPropertie.CostMultiplayer;
            EventsManager.OnHumansNumPropertieUpdate(false);
            dataSaver.AddDataToBeSaved(Constants.DataNames.HumansNum, humansNumPropertie.Value);
            dataSaver.AddDataToBeSaved(Constants.DataNames.HumansNumCost, humansNumPropertie.Cost);
            return true;
        }

        return false;
    }

    public float GetPropertieValue(Constants.PropertiesTypes propertieType)
    {
        switch (propertieType)
        {
            case Constants.PropertiesTypes.RunAfterSpeed:
                return runAfterSpeedPropertie.Value;
            case Constants.PropertiesTypes.HumansNum:
                return humansNumPropertie.Value;
            case Constants.PropertiesTypes.Health:
                return healthPropertie.Value;
            case Constants.PropertiesTypes.ProjectilesNum:
                return projectilesNumPropertie.Value;
            default:
                return 0;
        }
    }

    public float GetPropertieCost(Constants.PropertiesTypes propertieType)
    {
        switch (propertieType)
        {
            case Constants.PropertiesTypes.RunAfterSpeed:
                return runAfterSpeedPropertie.Cost;
            case Constants.PropertiesTypes.HumansNum:
                return humansNumPropertie.Cost;
            case Constants.PropertiesTypes.Health:
                return healthPropertie.Cost;
            case Constants.PropertiesTypes.ProjectilesNum:
                return projectilesNumPropertie.Cost;
            default:
                return 0;
        }
    }

    public float GetPropertieInitialCost(Constants.PropertiesTypes propertieType)
    {
        switch (propertieType)
        {
            case Constants.PropertiesTypes.RunAfterSpeed:
                return runAfterSpeedPropertie.InitialCost;
            case Constants.PropertiesTypes.HumansNum:
                return humansNumPropertie.InitialCost;
            case Constants.PropertiesTypes.Health:
                return healthPropertie.InitialCost;
            case Constants.PropertiesTypes.ProjectilesNum:
                return projectilesNumPropertie.InitialCost;
            default:
                return 0;
        }
    }

    private void ResetForNewLevel()
    {
        healthPropertie.Value = healthPropertie.InitialValue;
        humansNumPropertie.Value = humansNumPropertie.InitialValue;
        projectilesNumPropertie.Value = projectilesNumPropertie.InitialValue;
        runAfterSpeedPropertie.Value = runAfterSpeedPropertie.InitialValue;
        healthPropertie.Cost = healthPropertie.InitialCost;
        humansNumPropertie.Cost = humansNumPropertie.InitialCost;
        projectilesNumPropertie.Cost = projectilesNumPropertie.InitialCost;
        runAfterSpeedPropertie.Cost = runAfterSpeedPropertie.InitialCost;

        EventsManager.OnHealthPropertieUpdate(healthPropertie.Value, true);
        EventsManager.OnRunAfterSpeedUpdate(runAfterSpeedPropertie.Value, true);
        EventsManager.OnProjectilesNumPropertieUpdate((int)projectilesNumPropertie.Value, true);
        EventsManager.OnHumansNumPropertieUpdate(true);

        dataSaver.AddDataToBeSaved(Constants.DataNames.Health, healthPropertie.Value);
        dataSaver.AddDataToBeSaved(Constants.DataNames.HealthCost, healthPropertie.Cost);
        dataSaver.AddDataToBeSaved(Constants.DataNames.RunAfterSpeed, runAfterSpeedPropertie.Value);
        dataSaver.AddDataToBeSaved(Constants.DataNames.RunAfterSpeedCost, runAfterSpeedPropertie.Cost);
        dataSaver.AddDataToBeSaved(Constants.DataNames.ProjectilesNum, projectilesNumPropertie.Value);
        dataSaver.AddDataToBeSaved(Constants.DataNames.ProjectilesNumCost, projectilesNumPropertie.Cost);
        dataSaver.AddDataToBeSaved(Constants.DataNames.HumansNum, humansNumPropertie.Value);
        dataSaver.AddDataToBeSaved(Constants.DataNames.HumansNumCost, humansNumPropertie.Cost);
    }

    private void OnDestroy()
    {
        EventsManager.onLevelStart -= ResetForNewLevel;
    }
}
