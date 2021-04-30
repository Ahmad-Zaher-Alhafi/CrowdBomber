using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HumanMoneyText : MonoBehaviour
{
    [SerializeField] private Transform moneyTextParent;
    [SerializeField] private TextMeshProUGUI moneyText;

    public Transform MoneyTextParent
    {
        get => moneyTextParent;
    }
    public TextMeshProUGUI MoneyText
    {
        get => moneyText;
    }
}
