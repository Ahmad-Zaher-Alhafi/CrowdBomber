using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePropertiesModifyier : MonoBehaviour
{
    public TextMeshProUGUI ProjectilesNumberTxt;
    public GameManager GameManager;
    public Human HumanPrefab;
    public Transform HumansParent;
    public float ZombieRunningSpeedIncreaser;//how much should the running speed of the zombie being increased each after speed adding
    public float MaxZombieRunningSpeed;
    public float MaxZombieHealth;
    public float ZombieRunningSpeed;
    public float ZombieHealth;
    public float ZombieHealthIncreaser;//how much should the health of the zombie being increased after each health adding from the player
    public int ProjectilesNumber;
    public MainCanves MainCanves;
    public ProjectileThrower ProjectileThrower;
    public float CurrentMoneyValue;
    public HumanCanvesStuff HumanCanvesStuff;
    public Transform HumanCanves;
    public int InitialNumOfHumansInLevel;
    public bool HasToDeleteSaveFiles;

    [HideInInspector] public int CurrentNumOfHumansInLevel;
    [HideInInspector] public float InitialZombieRunningSpeed;
    [HideInInspector] public float InitialHealth;
    [HideInInspector] public float AddHealthPowerUpCost;
    [HideInInspector] public float AddProjectilePowerUpCost;
    [HideInInspector] public float AddHumanPowerUpCost;
    [HideInInspector] public float AddRunSpeedPowerUpCost;


    private int maxAddedProjectiles;
    private Transform RightBorder;
    private Transform LeftBorder;
    private Transform UpBorder;
    private Transform DownBorder;
    private WaitForSeconds delay;
    private int currentStageNumber;
    private int humansCounter = 0;

    private void Awake()
    {
        if (HasToDeleteSaveFiles)
        {
            PlayerPrefs.DeleteAll();
        }

        InitializeProperties();
        delay = new WaitForSeconds(.5f);
        
        if (DataManager.CheckForDataName(Constants.ZombieRunningSpeedDataName))
        {
            ZombieRunningSpeed = DataManager.LoadFloat(Constants.ZombieRunningSpeedDataName);
        }

        if (DataManager.CheckForDataName(Constants.NumOfHumansDataName))
        {
            CurrentNumOfHumansInLevel = DataManager.LoadInt(Constants.NumOfHumansDataName);
        }

        if (DataManager.CheckForDataName(Constants.ZombieHealthDataName))
        {
            ZombieHealth = DataManager.LoadFloat(Constants.ZombieHealthDataName);
        }

        if (DataManager.CheckForDataName(Constants.ProjectilesNumberDataName))
        {
            ProjectilesNumber = DataManager.LoadInt(Constants.ProjectilesNumberDataName);
            maxAddedProjectiles = ProjectilesNumber;
        }

        if (DataManager.CheckForDataName(Constants.StageNumberDataName))
        {
            currentStageNumber = DataManager.LoadInt(Constants.StageNumberDataName);
        }

        if (DataManager.CheckForDataName(Constants.MoneyValueDataName))
        {
            CurrentMoneyValue = DataManager.LoadFloat(Constants.MoneyValueDataName);
        }

        if (DataManager.CheckForDataName(Constants.AddHealthPowerUpCostDataName))
        {
            AddHealthPowerUpCost = DataManager.LoadFloat(Constants.AddHealthPowerUpCostDataName);
        }

        if (DataManager.CheckForDataName(Constants.AddHumanPowerUpCostDataName))
        {
            AddHumanPowerUpCost= DataManager.LoadFloat(Constants.AddHumanPowerUpCostDataName);
        }

        if (DataManager.CheckForDataName(Constants.AddProjectilePowerUpCostDataName))
        {
            AddProjectilePowerUpCost = DataManager.LoadFloat(Constants.AddProjectilePowerUpCostDataName);
        }

        if (DataManager.CheckForDataName(Constants.AddRunSpeedPowerUpCostDataName))
        {
            AddRunSpeedPowerUpCost = DataManager.LoadFloat(Constants.AddRunSpeedPowerUpCostDataName);
        }
    }

    private void Start()
    {
        SetStageBorders(currentStageNumber);
        MainCanves.UpdateStageSlider(currentStageNumber);
        UpdateMoneyValue(0);
        MainCanves.UpdatePowerUpsCosts(false);
        UpdateProjectilesText();
        PrepareNextStage(false, currentStageNumber);
    }

    private void InitializeProperties()
    {
        InitialZombieRunningSpeed = ZombieRunningSpeed;
        InitialHealth = ZombieHealth;
        maxAddedProjectiles = ProjectilesNumber;
        currentStageNumber = GameManager.StageNumber;
        AddHealthPowerUpCost = MainCanves.AddHealthPowerUp.PowerUpCost;
        AddProjectilePowerUpCost = MainCanves.AddProjectilePowerUp.PowerUpCost;
        AddHumanPowerUpCost = MainCanves.AddHumanPowerUp.PowerUpCost;
        AddRunSpeedPowerUpCost = MainCanves.AddRunSpeedPowerUp.PowerUpCost;
        CurrentNumOfHumansInLevel = InitialNumOfHumansInLevel;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hasToResetSpeed">True if you want to reset the zombies speed in the new level, false if you want to incearse the zombie speed</param>
    public void UpdateZombieRunningSpeed(bool hasToResetSpeed)
    {
        if (hasToResetSpeed)
        {
            ZombieRunningSpeed = InitialZombieRunningSpeed;
        }
        else
        {
            ZombieRunningSpeed = Mathf.Clamp(ZombieRunningSpeed + ZombieRunningSpeedIncreaser, InitialZombieRunningSpeed, MaxZombieRunningSpeed);
        }

        EventsManager.OnZombieRunningSpeedModifying(hasToResetSpeed, ZombieRunningSpeed);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isItNew">True if the player pressed the add button to add a human, false if the GamePropertiesModifyier is creating the number of humans that has been saved</param>
    public void CreateHuman(bool isItNew)
    {
        humansCounter++;
        Human newHuman = Instantiate(HumanPrefab, GetRandomCreatPoint(), Quaternion.identity);
        newHuman.name = "Human" + humansCounter;
        newHuman.transform.parent = HumansParent;
        GameManager.Humans.Add(newHuman);
        CreateHumanCanvesStuff(newHuman);


        if (isItNew)
        {
            CurrentNumOfHumansInLevel++;
        }
    }

    /// <summary>
    /// Create the HumanCanvesStuff the contains the health slider and the money text of the zombie and assign it to it
    /// </summary>
    /// <param name="human">the human the you want to assgin the created HumanCanvessStuff to it</param>
    public void CreateHumanCanvesStuff(Human human)
    {
        HumanCanvesStuff humanCanves = Instantiate(HumanCanvesStuff.gameObject, human.HealthSliderPoint.position, Quaternion.identity, HumanCanves).GetComponent<HumanCanvesStuff>();
        humanCanves.Human = human;
        human.HumanCanvesStuff = humanCanves;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hasToResetSpeed">True if you want to reset the zombies health in the new level, false if you want to incearse the zombie health</param>
    public void UpdateZombieHealth(bool hasToResetSpeed)
    {
        if (hasToResetSpeed)
        {
            ZombieHealth = InitialHealth;
        }
        else
        {
            ZombieHealth = Mathf.Clamp(ZombieHealth + ZombieHealthIncreaser, InitialHealth, MaxZombieHealth);
        }

        EventsManager.onZombieHealthModifying(hasToResetSpeed, ZombieHealth);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isItNewLevel">if it is a new level</param>
    /// <param name="isItNewStage">if it is a new stage</param>
    /// <param name="numOfProjectile">num of projectiles to add to current projectiles (numOfProjectile = 0 by default if it was new level or new stage)</param>
    public void UpdateProjectilesNumber(bool isItNewLevel,bool isItNewStage, int numOfProjectile = 0)
    {
        if (isItNewLevel)
        {
            ProjectilesNumber = ProjectileThrower.InitialProjectilesNumber;
            maxAddedProjectiles = ProjectilesNumber;
            ProjectileThrower.UpdateProjectilesNumber(isItNewLevel);
        }
        else if (isItNewStage)
        {
            ProjectilesNumber = maxAddedProjectiles;
            ProjectileThrower.UpdateProjectilesNumber(isItNewLevel, maxAddedProjectiles);
        }
        else
        {
            ProjectilesNumber += numOfProjectile;
            if (numOfProjectile > 0)
            {
                maxAddedProjectiles += numOfProjectile;
            }
            ProjectileThrower.UpdateProjectilesNumber(isItNewLevel, ProjectilesNumber);
        }

        UpdateProjectilesText();
    }

    public void UpdateMoneyValue(float moneyToAdd)
    {
        CurrentMoneyValue += moneyToAdd;
        MainCanves.UpdateMoneyTxt(CurrentMoneyValue);
    }

    public void UpdateAddHealthCost(float newValue)
    {
        AddHealthPowerUpCost = newValue;
    }
    public void UpdateAddHumanCost(float newValue)
    {
        AddHumanPowerUpCost = newValue;
    }
    public void UpdateAddProjectileCost(float newValue)
    {
        AddProjectilePowerUpCost = newValue;
    }
    public void UpdateAddRunSpeedCost(float newValue)
    {
        AddRunSpeedPowerUpCost = newValue;
    }

    public Vector3 GetRandomCreatPoint()
    {
        return new Vector3(Random.Range(LeftBorder.position.x, RightBorder.position.x), 0, Random.Range(DownBorder.position.z, UpBorder.position.z));
    }

    private void UpdateProjectilesText()
    {
        ProjectilesNumberTxt.text = ProjectilesNumber.ToString();
    }

    /// <summary>
    /// set the borders that the humans are gonna use of the level according to the current stage number
    /// </summary>
    /// <param name="stageNumber">current stage number</param>
    private void SetStageBorders(int stageNumber)
    {
        if (stageNumber == 1)
        {
            RightBorder = GameManager.RightBorderStage1;
            LeftBorder = GameManager.LeftBorderStage1;
            UpBorder = GameManager.UpBorderStage1;
            DownBorder = GameManager.DownBorderStage1;
        }
        else if (stageNumber == 2)
        {
            RightBorder = GameManager.RightBorderStage2;
            LeftBorder = GameManager.LeftBorderStage2;
            UpBorder = GameManager.UpBorderStage2;
            DownBorder = GameManager.DownBorderStage2;
        }
        else if (stageNumber == 3)
        {
            RightBorder = GameManager.RightBorderStage3;
            LeftBorder = GameManager.LeftBorderStage3;
            UpBorder = GameManager.UpBorderStage3;
            DownBorder = GameManager.DownBorderStage3;
        }
    }

    public void PrepareNextStage(bool isItNewLevel, int stageNumber)
    {
        if (isItNewLevel)
        {
            //PlayerPrefs.DeleteAll();
            //InitializeProperties();
            //ProjectileThrower.UpdateProjectilesNumber(true);
            UpdateProjectilesNumber(true, false);
            MainCanves.UpdatePowerUpsCosts(true);
            UpdateZombieHealth(true);
            UpdateZombieRunningSpeed(true);
            CurrentNumOfHumansInLevel = InitialNumOfHumansInLevel;

            foreach (Human human in GameManager.DeadZombies)
            {
                human.Reset(true);
            }
        }
        else
        {
            UpdateProjectilesNumber(false, true);
        }

        UpdateProjectilesText();
        currentStageNumber = stageNumber;
        SetStageBorders(stageNumber);
        GameManager.SetCameraStagePos(stageNumber);
    }

    public void CreateNeededHumans()
    {
        if (GameManager.DeadZombies.Count > 0)
        {
            while (GameManager.Humans.Count < CurrentNumOfHumansInLevel && GameManager.DeadZombies.Count > 0)
            {
                GameManager.Humans.Add(GameManager.DeadZombies[0]);
                GameManager.DeadZombies.Remove(GameManager.DeadZombies[0]);
            }
        }

        while (GameManager.Humans.Count < CurrentNumOfHumansInLevel)
        {
            CreateHuman(false);
        }
    }

    private void OnApplicationQuit()
    {
        DataManager.SaveFloat(Constants.ZombieRunningSpeedDataName, ZombieRunningSpeed);
        DataManager.SaveInt(Constants.NumOfHumansDataName, CurrentNumOfHumansInLevel);
        DataManager.SaveFloat(Constants.ZombieHealthDataName, ZombieHealth);
        DataManager.SaveInt(Constants.ProjectilesNumberDataName, maxAddedProjectiles);
        DataManager.SaveInt(Constants.StageNumberDataName, currentStageNumber);
        DataManager.SaveFloat(Constants.MoneyValueDataName, CurrentMoneyValue);
        DataManager.SaveFloat(Constants.AddHealthPowerUpCostDataName, AddHealthPowerUpCost);
        DataManager.SaveFloat(Constants.AddHumanPowerUpCostDataName, AddHumanPowerUpCost);
        DataManager.SaveFloat(Constants.AddProjectilePowerUpCostDataName, AddProjectilePowerUpCost);
        DataManager.SaveFloat(Constants.AddRunSpeedPowerUpCostDataName, AddRunSpeedPowerUpCost);
    }
}
