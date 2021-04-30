using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventsManager
{
    public static Action<float, bool> onRunAfterSpeedUpdate;
    public static Action<float, bool> onHealthPropertieUpdate;
    public static Action<int, bool> onProjectilesNumPropertieUpdate;
    public static Action<bool> onHumansNumPropertieUpdate;
    public static Action<int> onStageStart;
    public static Action onLevelStart;
    public static Action onHumanDeath;
    public static Action onProjectileDeactivate;



    public static void OnRunAfterSpeedUpdate(float newSpeed, bool hasToReset)
    {
        onRunAfterSpeedUpdate?.Invoke(newSpeed, hasToReset);
    }

    public static void OnHealthPropertieUpdate(float newHealth, bool hasToReset)
    {
        onHealthPropertieUpdate?.Invoke(newHealth, hasToReset);
    }

    public static void OnProjectilesNumPropertieUpdate(int newProjectilesNumber, bool hasToReset)
    {
        onProjectilesNumPropertieUpdate?.Invoke(newProjectilesNumber, hasToReset);
    }
    public static void OnHumansNumPropertieUpdate(bool hasToReset)
    {
        onHumansNumPropertieUpdate?.Invoke(hasToReset);
    }

    public static void OnHumanDeath()
    {
        onHumanDeath?.Invoke();
    }

    public static void OnProjectileDeactivate()
    {
        onProjectileDeactivate?.Invoke();
    }

    public static void OnStageStart(int stageNumber)
    {
        onStageStart?.Invoke(stageNumber);
    }

    public static void OnLevelStart()
    {
        onLevelStart?.Invoke();
    }

}
