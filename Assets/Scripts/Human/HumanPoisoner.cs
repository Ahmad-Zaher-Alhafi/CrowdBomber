using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPoisoner : MonoBehaviour
{
    //get posioned after you get hit by the projectile after a delay (it is needed because the projectile collider is big which lead that the human get poisned before the ball reach the graound and explod)
    [SerializeField] private float timeToGetPosioned;
    [SerializeField] private GameObject poisonParticles;
    [SerializeField]private HumanClothing humanClothing;

    private bool isPoisoned;
    public bool IsPoisoned
    {
        get => isPoisoned;
        private set => isPoisoned = value;
    }

    private Human human;


    private void Start()
    {
        human = GetComponent<Human>();
    }

    public void GetPoisoned()
    {
        Invoke("GetPosionAfterDelay", timeToGetPosioned);
    }

    private void GetPosionAfterDelay()
    {
        if (IsPoisoned)
        {
            return;
        }

        IsPoisoned = true;

        Handheld.Vibrate();

        humanClothing.ApplyPoisonMaterial();
        human.PlayPopSound();
        human.RefreshCollider();
        human.CollectHumanMoney();
        human.ShowHumanUI(false);
        human.ActivateHumanSensor();
        human.AddToList(Constants.ListsNames.PosionedHumans);
        human.RemoveFromList(Constants.ListsNames.Humans);
        poisonParticles.SetActive(true);
    }

    public void ResetForNewStage()
    {
        IsPoisoned = false;
        poisonParticles.SetActive(false);
    }
}
