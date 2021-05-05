using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Human : MonoBehaviour
{
    [SerializeField] private float humanMoneyValue;
    public float HumanMoneyValue
    {
        get => humanMoneyValue;
        private set => humanMoneyValue = value;
    }

    [SerializeField] private HumanUI humanCanves;
    [SerializeField] private ParticleSystem newSpawnParticles;
    [SerializeField] private float timeToDisable;//time to disable the dead zombie's body
    [SerializeField] private HumanSensor humanSensor;
    [SerializeField] private Animator animator;
    [SerializeField] private HumanClothing humanClothing;

    private HumanUIManager humanUIManager;
    private Collider humanCollider;
    private AudioManager audioManager;
    private PropertiesManager propertiesHandler;
    private MoneyManager moneyManager;
    private HumanHealth humanHealth;
    private HumanPoisoner humanPoisoner;
    private BordersManger bordersManger;
    private HumansManager humansManager;
    private HumanAI humanAI;

    void Start()
    {
        humanAI = GetComponent<HumanAI>();
        humanHealth = GetComponent<HumanHealth>();
        humanUIManager = FindObjectOfType<HumanUIManager>();
        humansManager = FindObjectOfType<HumansManager>();
        moneyManager = FindObjectOfType<MoneyManager>();
        humanPoisoner = GetComponent<HumanPoisoner>();
        bordersManger = FindObjectOfType<BordersManger>();
        propertiesHandler = FindObjectOfType<PropertiesManager>();
        audioManager = FindObjectOfType<AudioManager>();
        humanCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// Deactivate and Reactivate the human collider to make sure that when the human get poisoned he collide again with the objects that were collided with it before
    /// </summary>
    public void RefreshCollider()
    {
        humanCollider.enabled = false;
        humanCollider.enabled = true;
    }

    public Human GetClosestHuman()
    {
        return humansManager.GetClosestHuman(this);
    }

    public HumanMoneyText GetHumanMoneyText()
    {
        return humanUIManager.CreateHumanMonetText();
    }

    public void OrderToRunFrom(Human humanToRunFrom)
    {
        humanAI.OrderToRunFrom(humanToRunFrom);
    }

    public void RemoveHumanThatRunAwayFrom(Human humanThatRunFrom)
    {
        humanAI.RemoveHumanThatRunAwayFrom(humanThatRunFrom);
    }

    public void PlayPopSound()
    {
        audioManager.PlayPopSound();
    }

    public void CollectHumanMoney()
    {
        moneyManager.ChangeMoneyAmount(HumanMoneyValue);
    }

    public Border GetActiveBorders()
    {
        return bordersManger.GetActiveBorders();
    }

    public void ActivateHumanSensor()
    {
        humanSensor.gameObject.SetActive(true);
    }

    public void UpdateHumanAnimator(string animationClipName, float animatorPlaySpeed)
    {
        animator.speed = animatorPlaySpeed;
        animator.Play(animationClipName);
    }

    public void ShowHumanUI(bool hasToShowSpecialText, int numOfHumansThatBeingHit = 1)
    {
        humanCanves.gameObject.SetActive(true);
        humanCanves.ShowUI(hasToShowSpecialText, numOfHumansThatBeingHit);
    }

    public float GetPropertie(Constants.PropertiesTypes propertieType)
    {
        return propertiesHandler.GetPropertieValue(propertieType);
    }

    public bool IsDead()
    {
        return humanHealth.IsDead;
    }

    public bool IsPoisoned()
    {
        return humanPoisoner.IsPoisoned;
    }

    public void ResetForNewStage()
    {
        humanPoisoner.ResetForNewStage();
        humanClothing.ResetForNewStage();
        humanHealth.ResetForNewStage();
        humanAI.ResetForNewStage();
        humanSensor.ResetForNextStage();
    }

    public void OrderToShowSpecialText(int numOfHumansThatBeingHit)
    {
        humanCanves.ShowSpecialText(numOfHumansThatBeingHit);
    }

    public void ShowNewSpawnParticles()
    {
        newSpawnParticles.Play();
    }

    public void Die()
    {
        AddToList(Constants.ListsNames.DeadHumans);
        RemoveFromList(Constants.ListsNames.PosionedHumans);

        Invoke("DisableAfterTime", timeToDisable);
    }

    public void AddToList(Constants.ListsNames listName)
    {
        humansManager.AddToList(listName, this);
    }

    public void RemoveFromList(Constants.ListsNames listName)
    {
        humansManager.RemoveFromList(listName, this);
    }

    private void DisableAfterTime()
    {
        EventsManager.OnHumanDeath();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (humanHealth.IsDead)
        {
            return;
        }

        if (other.CompareTag(Constants.ProjectileTag) && !humanPoisoner.IsPoisoned)
        {
            humanPoisoner.GetPoisoned();
        }

        if (CompareTag(Constants.HumanTag))
        {
            if (humanPoisoner.IsPoisoned && other.CompareTag(Constants.HumanTag))
            {
                HumanPoisoner humanZombie = other.GetComponent<HumanPoisoner>();

                if (!humanZombie.IsPoisoned)
                {
                    humanZombie.GetPoisoned();
                }
            }
        }
    }
}
