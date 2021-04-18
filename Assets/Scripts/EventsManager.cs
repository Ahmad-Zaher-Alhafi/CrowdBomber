using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventsManager
{
    public static Action<bool, float> onZombieSpeedModifying;
    public static Action<bool, float> onZombieHealthModifying;
    public static Action onZombieDeath;
    public static Action onProjectileDeactivate;



    public static void OnZombieRunningSpeedModifying(bool hasToResetSpeed, float newSpeed)
    {
        onZombieSpeedModifying?.Invoke(hasToResetSpeed, newSpeed);
    }

    public static void OnZombieHealthModifying(bool hasToResetSpeed, float newHealth)
    {
        onZombieHealthModifying?.Invoke(hasToResetSpeed, newHealth);
    }

    public static void OnZombieDeath()
    {
        onZombieDeath?.Invoke();
    }

    public static void OnProjectileDeactivate()
    {
        onProjectileDeactivate?.Invoke();
    }
}
