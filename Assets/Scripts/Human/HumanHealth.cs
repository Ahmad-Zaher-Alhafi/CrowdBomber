using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHealth : MonoBehaviour
{
    [SerializeField] private float healthDecreaser;

    private float health;
    public float Health
    {
        get => health;
        private set => health = value;
    }

    private bool isDead;
    public bool IsDead
    {
        get => isDead;
        private set => isDead = value;
    }

    private float initialHealth;
    private Human human;
    private HumanPoisoner humanPoisoner;

    void Start()
    {
        human = GetComponent<Human>();
        humanPoisoner = GetComponent<HumanPoisoner>();
        Health = human.GetPropertie(Constants.PropertiesTypes.Health);
        initialHealth = Health;
        EventsManager.onHealthPropertieUpdate += UpdateHealth;
    }

    void Update()
    {
        if (humanPoisoner.IsPoisoned)
        {
            DecreaseHealth();
        }
    }

    private void DecreaseHealth()
    {
        if (!IsDead)
        {
            Health -= Time.deltaTime * healthDecreaser;
        }

        if (Health <= 0)
        {
            IsDead = true;
            human.Die();
        }
    }


    private void UpdateHealth(float newHealth, bool hasToReset)
    {
        if (hasToReset)
        {
            Health = initialHealth;
        }
        else
        {
            Health = newHealth;
        }
    }

    public void ResetForNewStage()
    {
        IsDead = false;
        Health = human.GetPropertie(Constants.PropertiesTypes.Health);
    }
    private void OnDestroy()
    {
        EventsManager.onHealthPropertieUpdate -= UpdateHealth;
    }
}
