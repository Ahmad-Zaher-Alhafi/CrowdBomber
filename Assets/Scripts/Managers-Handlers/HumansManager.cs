using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumansManager : MonoBehaviour
{
    [SerializeField] private PropertiesManager propertiesManager;
    [SerializeField] private Human humanPrefab;
    [SerializeField] private Transform humansParent;
    [SerializeField] private BordersManger bordersManger;

    private List<Human> deadHumans = new List<Human>();
    private List<Human> posionedHumans = new List<Human>();
    private List<Human> humans = new List<Human>();
    private int humansCounter = 0;
    private int numOfHumans;
    private int initialNumOfHumans;

    private void Start()
    {
        EventsManager.onHumansNumPropertieUpdate += OnHumansNumUpdate;

        numOfHumans = (int)propertiesManager.GetPropertieValue(Constants.PropertiesTypes.HumansNum);
        initialNumOfHumans = numOfHumans;
    }


    /// <summary>
    /// To add a human to some list
    /// </summary>
    /// <param name="listName">list that you want to add the human to</param>
    /// <param name="humanToAdd">Human that you want to add it</param>
    public void AddToList(Constants.ListsNames listName, Human humanToAdd)
    {
        switch (listName)
        {
            case Constants.ListsNames.Humans:
                humans.Add(humanToAdd);
                break;
            case Constants.ListsNames.DeadHumans:
                deadHumans.Add(humanToAdd);
                break;
            case Constants.ListsNames.PosionedHumans:
                posionedHumans.Add(humanToAdd);
                break;
            default:
                break;
        }
    }

    public Human GetClosestHuman(Human human)
    {
        if (humans.Count <= 0)
        {
            return null;
        }

        Human closestHuman = humans[0];
        float closestDistance = Mathf.Infinity;

        foreach (Human h in humans)
        {
            if (h != human && !h.IsPoisoned() && h.isActiveAndEnabled)
            {
                float distance = (h.transform.position - human.transform.position).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestHuman = h;
                    closestDistance = distance;
                }
            }
        }

        if (closestHuman != null && !closestHuman.IsPoisoned())
        {
            return closestHuman;
        }

        return null;
    }

    /// <summary>
    /// To remove a human from some list
    /// </summary>
    /// <param name="listName">List that you want to remove this human from</param>
    /// <param name="humanToRemove">Human that you want to remove it</param>
    public void RemoveFromList(Constants.ListsNames listName, Human humanToRemove)
    {
        switch (listName)
        {
            case Constants.ListsNames.Humans:
                humans.Remove(humanToRemove);
                break;
            case Constants.ListsNames.DeadHumans:
                deadHumans.Remove(humanToRemove);
                break;
            case Constants.ListsNames.PosionedHumans:
                posionedHumans.Remove(humanToRemove);
                break;
            default:
                break;
        }
    }

    public int GetListCount(Constants.ListsNames listName)
    {
        switch (listName)
        {
            case Constants.ListsNames.Humans:
                return humans.Count;
            case Constants.ListsNames.DeadHumans:
                return deadHumans.Count;
            case Constants.ListsNames.PosionedHumans:
                return posionedHumans.Count;
            default:
                return 0;
        }
    }

    public void CreateNeededHumans()
    {
        while (humans.Count < numOfHumans && deadHumans.Count > 0)
        {
            deadHumans[0].ResetForNewStage();
            deadHumans[0].gameObject.SetActive(true);
            AddToList(Constants.ListsNames.Humans, deadHumans[0]);
            RemoveFromList(Constants.ListsNames.DeadHumans, deadHumans[0]);
        }

        while (humans.Count < numOfHumans)
        {
            CreateHuman(false);
        }
    }

    public void OnHumansNumUpdate(bool hasToReset)
    {
        if (hasToReset)
        {
            numOfHumans = initialNumOfHumans;
        }
        else
        {
            CreateHuman(true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isItNew">True if the player pressed the add button to add a human, false if the GamePropertiesModifyier is creating the number of humans that has been saved</param>
    public void CreateHuman(bool isItNew)
    {
        Human newHuman = Instantiate(humanPrefab, bordersManger.GetActiveBorders().GetRandomPointInsideBorders(), Quaternion.identity);
        newHuman.name = "Human" + humansCounter;
        newHuman.transform.parent = humansParent;
        AddToList(Constants.ListsNames.Humans, newHuman);

        if (isItNew)
        {
            newHuman.ShowNewSpawnParticles();
        }
    }

    private void OnDestroy()
    {
        EventsManager.onHumansNumPropertieUpdate -= OnHumansNumUpdate;
    }
}
