using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HumanCanves : MonoBehaviour
{
    public GameObject MoneyTextPrefab;
    public float SecondsToDeactivateMoneyTexy;
    public Transform HealthCircle;
    public Transform MoneyTextPoint;
    public Human Human;

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

        if (moneyText == null)
        {
            moneyText = Instantiate(MoneyTextPrefab, MoneyTextPoint.position, Quaternion.identity, humanTextCanves).GetComponent<TextMeshProUGUI>();
        }
    }

    void Start()
    {
        initialHealthCircleScale = HealthCircle.localScale;
        gamePropertiesModifyier = FindObjectOfType<GamePropertiesModifyier>();
        delay = new WaitForSeconds(SecondsToDeactivateMoneyTexy);
    }


    void Update()
    {
        transform.LookAt(maincamera.transform);
        moneyText.transform.LookAt(maincamera.transform);
        moneyText.transform.position = MoneyTextPoint.position;

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

    public void ShowMoneyText()
    {
        print("show");
        moneyText.text = "+" + Human.ZombieMoneyValue.ToString();
        moneyText.gameObject.SetActive(true);
        StartCoroutine(DeactivateMoneyText());
    }

    //public void Reset()
    //{
    //    HealthCircle.localScale = initialHealthCircleScale;
    //}

    private IEnumerator DeactivateMoneyText()
    {
        yield return delay;
        moneyText.gameObject.SetActive(false);
    }
}