using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanves : MonoBehaviour
{
    public Slider StagesSlider;
    public TextMeshProUGUI StagesText;
    public TextMeshProUGUI MoneyTxt;

    public CanvesPowerUp AddHealthPowerUp;
    public CanvesPowerUp AddHumanPowerUp;
    public CanvesPowerUp AddProjectilePowerUp;
    public CanvesPowerUp AddRunSpeedPowerUp;
    

    private GamePropertiesModifyier gamePropertiesModifyier;
    private GameManager gameManager;

    [System.Serializable]
    public struct CanvesPowerUp
    {
        public TextMeshProUGUI PowerUpCostText;
        public float PowerUpCost;
        public float PowerUpCostIncreaseValue;
        public float InitialPowerUpcost;
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gamePropertiesModifyier = FindObjectOfType<GamePropertiesModifyier>();
    }

    void Start()
    {
        StagesSlider.maxValue = gameManager.MaxStagesNumInLevel;
        StagesSlider.minValue = 1;
        UpdatePowerUpsCostsTxts();
    }

    public void AddZombieRunningSpeed()
    {
        if (gamePropertiesModifyier.CurrentMoneyValue >= AddRunSpeedPowerUp.PowerUpCost)
        {
            gamePropertiesModifyier.UpdateZombieRunningSpeed(false);
            gamePropertiesModifyier.UpdateMoneyValue(-AddRunSpeedPowerUp.PowerUpCost);
            AddRunSpeedPowerUp.PowerUpCost *= AddRunSpeedPowerUp.PowerUpCostIncreaseValue;
            UpdatePowerUpsCostsTxts();
            UpdateMoneyTxt(gamePropertiesModifyier.CurrentMoneyValue);
            gamePropertiesModifyier.UpdateAddRunSpeedCost(AddRunSpeedPowerUp.PowerUpCost);
        }
    }

    public void AddHuman()
    {
        if (gamePropertiesModifyier.CurrentNumOfHumansInLevel >= gamePropertiesModifyier.MaxNumOfHumansInLevel)
        {
            return;
        }

        if (gamePropertiesModifyier.CurrentMoneyValue >= AddHumanPowerUp.PowerUpCost)
        {
            gamePropertiesModifyier.CreateHuman(true);
            gamePropertiesModifyier.UpdateMoneyValue(-AddHumanPowerUp.PowerUpCost);
            AddHumanPowerUp.PowerUpCost *= AddHumanPowerUp.PowerUpCostIncreaseValue;
            UpdatePowerUpsCostsTxts();
            UpdateMoneyTxt(gamePropertiesModifyier.CurrentMoneyValue);
            gamePropertiesModifyier.UpdateAddHumanCost(AddHumanPowerUp.PowerUpCost);
        }
    }

    public void IncreaseHealth()
    {
        if (gamePropertiesModifyier.CurrentMoneyValue >= AddHealthPowerUp.PowerUpCost)
        {
            gamePropertiesModifyier.UpdateZombieHealth(false);
            gamePropertiesModifyier.UpdateMoneyValue(-AddHealthPowerUp.PowerUpCost);
            AddHealthPowerUp.PowerUpCost *= AddHealthPowerUp.PowerUpCostIncreaseValue;
            UpdatePowerUpsCostsTxts();
            UpdateMoneyTxt(gamePropertiesModifyier.CurrentMoneyValue);
            gamePropertiesModifyier.UpdateAddHealthCost(AddHealthPowerUp.PowerUpCost);
        }
    }

    public void AddProjectile()
    {
        if (gamePropertiesModifyier.CurrentMoneyValue >= AddProjectilePowerUp.PowerUpCost)
        {
            gamePropertiesModifyier.UpdateProjectilesNumber(false, false, 1);
            gamePropertiesModifyier.UpdateMoneyValue(-AddProjectilePowerUp.PowerUpCost);
            AddProjectilePowerUp.PowerUpCost *= AddProjectilePowerUp.PowerUpCostIncreaseValue;
            UpdatePowerUpsCostsTxts();
            UpdateMoneyTxt(gamePropertiesModifyier.CurrentMoneyValue);
            gamePropertiesModifyier.UpdateAddProjectileCost(AddProjectilePowerUp.PowerUpCost);
        }
    }

    public void UpdateStageSlider(int StageNum)
    {
        StagesSlider.value = StageNum;
        StagesText.text = "Stage " + StageNum + "/" + gameManager.MaxStagesNumInLevel;
    }

    public void UpdateMoneyTxt(float moneyValue)
    {
        MoneyTxt.text = "$" + moneyValue.ToString();
    }

    public void UpdatePowerUpsCosts(bool isItNewLevel)
    {
        if (isItNewLevel)
        {
            AddHumanPowerUp.PowerUpCost = AddHumanPowerUp.InitialPowerUpcost;
            AddHealthPowerUp.PowerUpCost = AddHealthPowerUp.InitialPowerUpcost;
            AddRunSpeedPowerUp.PowerUpCost = AddRunSpeedPowerUp.InitialPowerUpcost;
            AddProjectilePowerUp.PowerUpCost = AddProjectilePowerUp.InitialPowerUpcost;

            gamePropertiesModifyier.UpdateAddHealthCost(AddHealthPowerUp.PowerUpCost);
            gamePropertiesModifyier.UpdateAddProjectileCost(AddProjectilePowerUp.PowerUpCost);
            gamePropertiesModifyier.UpdateAddRunSpeedCost(AddRunSpeedPowerUp.PowerUpCost);
            gamePropertiesModifyier.UpdateAddHumanCost(AddHumanPowerUp.PowerUpCost);
        }
        else
        {
            AddHumanPowerUp.PowerUpCost = gamePropertiesModifyier.AddHumanPowerUpCost;
            AddHealthPowerUp.PowerUpCost = gamePropertiesModifyier.AddHealthPowerUpCost;
            AddRunSpeedPowerUp.PowerUpCost = gamePropertiesModifyier.AddRunSpeedPowerUpCost;
            AddProjectilePowerUp.PowerUpCost = gamePropertiesModifyier.AddProjectilePowerUpCost;
        }

        UpdatePowerUpsCostsTxts();
    }

    private void UpdatePowerUpsCostsTxts()
    {
        AddHumanPowerUp.PowerUpCostText.text = "$" + AddHumanPowerUp.PowerUpCost.ToString();
        AddHealthPowerUp.PowerUpCostText.text = "$" + AddHealthPowerUp.PowerUpCost.ToString();
        AddRunSpeedPowerUp.PowerUpCostText.text = "$" + AddRunSpeedPowerUp.PowerUpCost.ToString();
        AddProjectilePowerUp.PowerUpCostText.text = "$" + AddProjectilePowerUp.PowerUpCost.ToString();
    }
}
