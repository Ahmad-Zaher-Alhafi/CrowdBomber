using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanUI : MonoBehaviour
{

    [SerializeField] private float timeToDeactivateMoneyText;
    [SerializeField] private GameObject healthCircleBackground;
    [SerializeField] private Transform healthCircle;
    [SerializeField] private Transform moneyTextPoint;
    [SerializeField] private Human human;
    [SerializeField] private float healthCircleSizeOnStage1;
    [SerializeField] private float healthCircleSizeOnStage2;
    [SerializeField] private float healthCircleSizeOnStage3;
    [SerializeField] private string[] specialWords;
    [SerializeField] private HumanHealth humanHealth;
    
    private Vector3 initialHealthCircleScale;
    private Camera maincamera;
    private float initialFontSize;
    private HumanMoneyText humanMoneyText;

    private void Start()
    {
        EventsManager.onStageStart += SetHealthCircleScale;

        maincamera = Camera.main;
        initialHealthCircleScale = healthCircle.localScale;
        GetHumanMoneyText();
    }

    void Update()
    {
        transform.LookAt(maincamera.transform);
        humanMoneyText.MoneyTextParent.LookAt(maincamera.transform);
        humanMoneyText.MoneyTextParent.position = moneyTextPoint.position;

        if (healthCircle.localScale.x > 0)
        {
            healthCircle.localScale = Vector3.one * Mathf.InverseLerp(0, human.GetPropertie(Constants.PropertiesTypes.Health), humanHealth.Health);
        }
        else
        {
            healthCircle.localScale = initialHealthCircleScale;
            healthCircleBackground.SetActive(false);
        }
    }

    public void GetHumanMoneyText()
    {
        humanMoneyText = human.GetHumanMoneyText();
        initialFontSize = humanMoneyText.MoneyText.fontSize;
    }

    private void SetHealthCircleScale(int stageNumber)
    {
        if (stageNumber == 1)
        {
            transform.localScale = Vector3.one * healthCircleSizeOnStage1;
            humanMoneyText.MoneyTextParent.transform.localScale = Vector3.one * healthCircleSizeOnStage1;
        }
        else if (stageNumber == 2)
        {
            transform.localScale = Vector3.one * healthCircleSizeOnStage2;
            humanMoneyText.MoneyTextParent.transform.localScale = Vector3.one * healthCircleSizeOnStage2;
        }
        else if (stageNumber == 3)
        {
            transform.localScale = Vector3.one * healthCircleSizeOnStage3;
            humanMoneyText.MoneyTextParent.transform.localScale = Vector3.one * healthCircleSizeOnStage3;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hasToShowSpecialWord"></param>
    /// <param name="numOfHumansThatBeingHit">Not needed if hasToShowSpecialWord = false</param>
    public void ShowUI(bool hasToShowSpecialWord, int numOfHumansThatBeingHit = 1)
    {
        healthCircleBackground.SetActive(true);

        if (hasToShowSpecialWord)
        {
            ShowSpecialText(numOfHumansThatBeingHit);
        }
        else
        {
            ShowMoneyText();
        }
    }

    public void ShowSpecialText(int numOfHumansThatBeingHit)
    {
        humanMoneyText.MoneyText.text = specialWords[Mathf.Clamp(numOfHumansThatBeingHit - 2, 0, specialWords.Length - 1)];
        humanMoneyText.MoneyText.fontSize *= 1.3f;
    }

    public void ShowMoneyText()
    {
        humanMoneyText.MoneyText.fontSize = initialFontSize;
        humanMoneyText.MoneyText.text = "$" + human.HumanMoneyValue.ToString();
        humanMoneyText.MoneyTextParent.gameObject.SetActive(true);

        Invoke("DeactivateMoneyText", timeToDeactivateMoneyText);
    }

    private void DeactivateMoneyText()
    {
        humanMoneyText.MoneyTextParent.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventsManager.onStageStart -= SetHealthCircleScale;
    }
}