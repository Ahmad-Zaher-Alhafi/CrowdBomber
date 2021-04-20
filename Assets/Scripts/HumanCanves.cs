using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HumanCanves : MonoBehaviour
{
    public GameObject MoneyTextParentPrefab;
    public float SecondsToDeactivateMoneyTexy;
    public Transform HealthCircle;
    public Transform MoneyTextPoint;
    public Human Human;
    public float HealthCircleSizeOnStage1;
    public float HealthCircleSizeOnStage2;
    public float HealthCircleSizeOnStage3;


    private Transform moneyTextParent;
    private Transform humanTextCanves;
    private TextMeshProUGUI moneyText;
    private Vector3 initialHealthCircleScale;
    private WaitForSeconds delay;
    private GamePropertiesModifyier gamePropertiesModifyier;
    private Camera maincamera;

    private void Awake()
    {
        maincamera = Camera.main;
        humanTextCanves = GameObject.FindGameObjectWithTag(Constants.HumanTextCanvesTag).transform;
        delay = new WaitForSeconds(SecondsToDeactivateMoneyTexy);

        if (moneyText == null)
        {
            moneyTextParent = Instantiate(MoneyTextParentPrefab, MoneyTextPoint.position, Quaternion.identity, humanTextCanves).transform;
            moneyText = moneyTextParent.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        initialHealthCircleScale = HealthCircle.localScale;
        gamePropertiesModifyier = FindObjectOfType<GamePropertiesModifyier>();
        SetHealthCircleScale();
    }

    void Update()
    {
        transform.LookAt(maincamera.transform);
        moneyTextParent.LookAt(maincamera.transform);
        moneyTextParent.position = MoneyTextPoint.position;

        if (HealthCircle.localScale.x > 0)
        {
            HealthCircle.localScale = Vector3.one * Mathf.InverseLerp(0, gamePropertiesModifyier.MaxZombieHealth, Human.Health);
        }
        else
        {
            HealthCircle.localScale = initialHealthCircleScale;
            gameObject.SetActive(false);
        }
    }

    private void SetHealthCircleScale()
    {
        if (gamePropertiesModifyier.CurrentStageNumber == 1)
        {
            transform.localScale = Vector3.one * HealthCircleSizeOnStage1;
            moneyTextParent.transform.localScale = Vector3.one * HealthCircleSizeOnStage1;
        }
        else if (gamePropertiesModifyier.CurrentStageNumber == 2)
        {
            transform.localScale = Vector3.one * HealthCircleSizeOnStage2;
            moneyTextParent.transform.localScale = Vector3.one * HealthCircleSizeOnStage2;
        }
        else if (gamePropertiesModifyier.CurrentStageNumber == 3)
        {
            transform.localScale = Vector3.one * HealthCircleSizeOnStage3;
            moneyTextParent.transform.localScale = Vector3.one * HealthCircleSizeOnStage3;
        }
    }

    public void ShowMoneyText()
    {
        moneyText.text = "$" + Human.ZombieMoneyValue.ToString();
        moneyTextParent.gameObject.SetActive(true);
        SetHealthCircleScale();
        StartCoroutine(DeactivateMoneyText());
    }

    private IEnumerator DeactivateMoneyText()
    {
        yield return delay;
        moneyTextParent.gameObject.SetActive(false);
    }
}