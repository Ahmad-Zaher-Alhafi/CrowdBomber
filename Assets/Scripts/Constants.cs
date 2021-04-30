using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    //Data Playerprefs Names
    //public static string ZombieRunningSpeedDataName = "ZombieRunningSpeed";
    //public static string NumOfHumansDataName = "NumOfHumans";
    //public static string HealthDataName = "Health";
    //public static string ProjectilesNumberDataName = "ProjectilesNumber";
    //public static string StageNumberDataName = "StageNumber";
    //public static string MoneyValueDataName = "MoneyValue";
    //public static string AddHealthPowerUpCostDataName = "AddHealthPowerUpCost";
    //public static string AddHumanPowerUpCostDataName = "AddHumanPowerUpCost";
    //public static string AddProjectilePowerUpCostDataName = "AddProjectilePowerUpCost";
    //public static string AddRunSpeedPowerUpCostDataName = "AddRunSpeedPowerUpCost";
    //public static string LevelNumberDataName = "LevelNumber";

    //Layers
    public static int GroundLayerNumber = 8;

    //Tags
    public static string HumanTag = "Human";
    public static string ProjectileTag = "Projectile";
    public static string WallTag = "Wall";
    public static string HumanTextCanvesTag = "HumanTextCanves";

    //Animation Clips Names
    public static string WalkAnimationClipName = "walk";
    public static string RunAnimationClipName = "run";
    public static string WalkduckedAnimationClipName = "walk ducked";

    //Enums

    //Lists names in HumansZombiesManager class
    public enum ListsNames
    {
        Humans,
        DeadHumans,
        PosionedHumans,
    }

    //Data playerProbs names
    public enum DataNames
    {
        RunAfterSpeed,
        RunAfterSpeedCost,
        HumansNum,
        HumansNumCost,
        Health,
        HealthCost,
        ProjectilesNum,
        ProjectilesNumCost,
        StageNumber,
        MoneyAmount,
        LevelNumber,


        enumLength
    }

    //Human Agent Speed Types
    public enum SpeedTypes
    {
        WalkSpeed,
        RunFromSpeed,
        RunAfterSpeed,
    }

    //Properties types
    public enum PropertiesTypes
    {
        RunAfterSpeed,
        HumansNum,
        Health,
        ProjectilesNum,
    }
}
