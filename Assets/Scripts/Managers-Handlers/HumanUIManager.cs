using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanUIManager : MonoBehaviour
{
    [SerializeField] private HumanMoneyText humanMoneyTxtParentPrefab;
    [SerializeField] private Transform humansTxtsCanves;

    public HumanMoneyText CreateHumanMonetText()
    {
        return Instantiate(humanMoneyTxtParentPrefab, humansTxtsCanves);
    }
}
