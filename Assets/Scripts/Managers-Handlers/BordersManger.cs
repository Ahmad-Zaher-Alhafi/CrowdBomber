using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BordersManger : MonoBehaviour
{
    [SerializeField] private List<Border> borders;
    [SerializeField] private GameManager gameManager;



    private void Start()
    {
        EventsManager.onStageStart += SetBordersActivation;
    }

    /// <summary>
    /// To actiavate or deactivate the borders according to the current stage
    /// </summary>
    /// <param name="stage1">true to activate first stage borders else false</param>
    /// <param name="stage2">true to activate second stage borders else false</param>
    /// <param name="stage3">true to activate third stage borders else false</param>
    public void SetBordersActivation(int stageNumber)
    {
        foreach (Border border in borders)
        {
            if (border.BorderStageNumber == stageNumber)
            {
                border.UpdateActivationState(true);
            }
            else
            {
                border.UpdateActivationState(false);
            }
        }
    }

    public Border GetActiveBorders()
    {
        foreach (Border border in borders)
        {
            if (border.BorderStageNumber == gameManager.StageNumber)
            {
                return border;
            }
        }

        Debug.LogError("Borders not found");
        return null;
    }

    private void OnDestroy()
    {
        EventsManager.onStageStart -= SetBordersActivation;
    }
}
