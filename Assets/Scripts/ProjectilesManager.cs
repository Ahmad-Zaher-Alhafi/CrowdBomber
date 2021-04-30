using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilesManager : MonoBehaviour
{
    private int numOfActiveProjectilesInScene;
    public int NumOfActiveProjectilesInScene
    {
        get => numOfActiveProjectilesInScene;
        private set => numOfActiveProjectilesInScene = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numToIncreaseDecrease">This can be positive or nigative</param>
    public void UpdateActiveProjectilesNumberInScene(int numToIncreaseDecrease)
    {
        NumOfActiveProjectilesInScene += numToIncreaseDecrease;
    }
}
