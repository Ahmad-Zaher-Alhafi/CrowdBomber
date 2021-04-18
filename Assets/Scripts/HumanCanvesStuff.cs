using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HumanCanvesStuff : MonoBehaviour
{
    public TextMeshProUGUI MoneyText;
    public float SecondsToDeactivateMoneyTexy;
    public Slider HealthSilder;
    [HideInInspector] public Human Human;

    private WaitForSeconds delay;


    void Start()
    {
        delay = new WaitForSeconds(SecondsToDeactivateMoneyTexy);
        HealthSilder.maxValue = Human.Health;
        HealthSilder.value = Human.Health;
        HealthSilder.minValue = 0;
    }


    void Update()
    {
        transform.position = Human.HealthSliderPoint.position;
        transform.LookAt(Camera.main.transform);

        if (HealthSilder.value > 0)
        {
            HealthSilder.value = Human.Health;
        }
        else
        {
            HealthSilder.gameObject.SetActive(false);
        }
    }

    public void ActivateHumanCanvesStuff()
    {
        MoneyText.text = "+" + Human.ZombieValue.ToString();
        MoneyText.gameObject.SetActive(true);
        HealthSilder.gameObject.SetActive(true);
        StartCoroutine(DeactivateMoneyText());
    }

    public void Reset()
    {
        HealthSilder.value = Human.Health;
    }

    private IEnumerator DeactivateMoneyText()
    {
        yield return delay;
        MoneyText.gameObject.SetActive(false);
    }
}