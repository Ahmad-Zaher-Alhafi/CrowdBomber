using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanAI : MonoBehaviour
{
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runFromSpeed;
    [Header("The allowed distance (between the zombie that the human running from and the human that is runnnig away) that will be used to decide which direction to choose for escabing")]
    [SerializeField] private Vector3 zHDistance;

    [SerializeField] private float navSampleDistance;//the distance the will be used to check if there is a place to run to in that distance position
    private float runAfterSpeed;

    private List<Human> humansThatRunFrom = new List<Human>();
    private NavMeshAgent agent;
    private bool hasToRunAway;
    private Human humanToRunFrom;
    //Probability of runing toward these directions
    private int goingRightProbability = 0;
    private int goingLeftProbability = 0;
    private int goingUpProbability = 0;
    private int goingDownProbability = 0;
    //

    private bool isGoingLeft;
    private bool isGoingRight;
    private bool isGoingUp;
    private bool isGoingDown;

    private Vector3 runTo = Vector3.zero;//position that the human will check if he can run to

    //the offset between the human and the zombie that is runnig after him
    private float offsetOnX;
    private float offsetOnZ;
    //
    private NavMeshHit hit;
    private float initialRunAfterSpeed;
    private Human human;
    private HumanHealth humanHealth;
    private HumanPoisoner humanPoisoner;
    private Border _activeBorder;


    // Start is called before the first frame update
    void Start()
    {
        EventsManager.onRunAfterSpeedUpdate += UpdateRunAfterSpeed;
        human = GetComponent<Human>();
        humanHealth = GetComponent<HumanHealth>();
        humanPoisoner = GetComponent<HumanPoisoner>();

        agent = GetComponent<NavMeshAgent>();
        _activeBorder = human.GetActiveBorders();
        initialRunAfterSpeed = human.GetPropertie(Constants.PropertiesTypes.RunAfterSpeed);
        runAfterSpeed = initialRunAfterSpeed;
        UpdateHumanAgentSpeed(Constants.SpeedTypes.WalkSpeed);

        if (!agent.isOnNavMesh)
        {
            transform.position = _activeBorder.RightBorder.position - Vector3.right * 3;
            agent.enabled = false;
            agent.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (humanHealth.IsDead)
        {
            return;
        }

        if (humanPoisoner.IsPoisoned)
        {
            RunAfter();
        }
        else if (hasToRunAway)
        {
            RunFrom();
        }
        else
        {
            PatrolAround();
        }
    }

    private void PatrolAround()
    {
        if (agent.isOnNavMesh && agent.remainingDistance < .3f)
        {
            agent.SetDestination(GetRandomPatrolPoint());
        }
        
    }

    public void RunAfter()
    {
        Human closestHuman = human.GetClosestHuman();

        if (closestHuman != null)
        {
            agent.SetDestination(closestHuman.transform.position);
            UpdateHumanAgentSpeed(Constants.SpeedTypes.RunAfterSpeed);
            human.UpdateHumanAnimator(Constants.WalkduckedAnimationClipName, runAfterSpeed / 2);
        }
    }

    public void UpdateHumanAgentSpeed(Constants.SpeedTypes speedType)
    {
        switch (speedType)
        {
            case Constants.SpeedTypes.WalkSpeed:
                agent.speed = walkingSpeed;
                break;
            case Constants.SpeedTypes.RunFromSpeed:
                agent.speed = runFromSpeed;
                break;
            case Constants.SpeedTypes.RunAfterSpeed:
                agent.speed = runAfterSpeed;
                break;
            default:
                break;
        }
    }

    public void OrderToRunFrom(Human humanToRunFrom)
    {
        humansThatRunFrom.Add(humanToRunFrom);
        UpdateHumanAgentSpeed(Constants.SpeedTypes.RunFromSpeed);
        this.humanToRunFrom = humanToRunFrom;
        human.UpdateHumanAnimator(Constants.RunAnimationClipName, 1);
        hasToRunAway = true;
    }

    public void StopRunnigAway()
    {
        hasToRunAway = false;
        humansThatRunFrom.Clear();
        humanToRunFrom = null;
        ResetPropabilities();
        UpdateHumanAgentSpeed(Constants.SpeedTypes.WalkSpeed);
        human.UpdateHumanAnimator(Constants.WalkAnimationClipName, 1);
    }

    private void ResetPropabilities()
    {
        goingRightProbability = 0;
        goingLeftProbability = 0;
        goingUpProbability = 0;
        goingDownProbability = 0;
    }

    public void RemoveHumanThatRunAwayFrom(Human humanThatRunFrom)
    {
        if (humansThatRunFrom.Contains(humanThatRunFrom))
        {
            humansThatRunFrom.Remove(humanThatRunFrom);
        }

        if (humansThatRunFrom.Count > 0)
        {
            humanToRunFrom = humansThatRunFrom[0];
        }
        else
        {
            StopRunnigAway();
        }
    }

    public void RunFrom()
    {
        if (humanToRunFrom.IsDead())
        {
            RemoveHumanThatRunAwayFrom(humanToRunFrom);
            return;
        }
        else if (humanPoisoner.IsPoisoned)
        {
            StopRunnigAway();
        }

        runTo = Vector3.zero;
        offsetOnX = transform.position.x - humanToRunFrom.transform.position.x;
        offsetOnZ = transform.position.z - humanToRunFrom.transform.position.z;

        FindEscabeWay();

        if (NavMesh.SamplePosition(runTo, out hit, navSampleDistance, 1 << NavMesh.GetAreaFromName("Walkable")))
        {
            if (_activeBorder.ChechIfInsideBorder(runTo))
            {
                agent.SetDestination(runTo);
            }
            else
            {
                if (isGoingUp)
                {
                    goingUpProbability--;

                    if (CheckIfOnTheNav(Vector3.left * navSampleDistance) && offsetOnX < zHDistance.x)
                    {
                        goingLeftProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.right * navSampleDistance) && offsetOnX > zHDistance.x)
                    {
                        goingRightProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.back * navSampleDistance) && offsetOnZ < zHDistance.z)
                    {
                        goingDownProbability++;
                    }
                }
                else if (isGoingDown)
                {
                    goingDownProbability--;

                    if (CheckIfOnTheNav(Vector3.left * navSampleDistance) && offsetOnX < zHDistance.x)
                    {
                        goingLeftProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.right * navSampleDistance) && offsetOnX > zHDistance.x)
                    {
                        goingRightProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.forward * navSampleDistance) && offsetOnZ > zHDistance.z)
                    {
                        goingUpProbability++;
                    }
                }
                else if (isGoingLeft)
                {
                    goingLeftProbability--;

                    if (CheckIfOnTheNav(Vector3.right * navSampleDistance) && offsetOnX > zHDistance.x)
                    {
                        goingRightProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.forward * navSampleDistance) && offsetOnZ > zHDistance.z)
                    {
                        goingUpProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.back * navSampleDistance) && offsetOnZ < zHDistance.z)
                    {
                        goingDownProbability++;
                    }
                }
                else if (isGoingRight)
                {
                    goingRightProbability--;

                    if (CheckIfOnTheNav(Vector3.left * navSampleDistance) && offsetOnX < zHDistance.x)
                    {
                        goingLeftProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.forward * navSampleDistance) && offsetOnZ > zHDistance.z)
                    {
                        goingUpProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.back * navSampleDistance) && offsetOnZ < zHDistance.z)
                    {
                        goingDownProbability++;
                    }
                }
            }
        }
    }

    private bool CheckIfOnTheNav(Vector3 pos)//to check if this position is on the Navmesh
    {
        runTo = transform.position + pos;

        if (NavMesh.SamplePosition(runTo, out hit, navSampleDistance, 1 << NavMesh.GetAreaFromName("Walkable")))
        {
            if (_activeBorder.ChechIfInsideBorder(runTo))
            {
                return true;
            }
        }

        return false;
    }

    private void FindEscabeWay()
    {
        if (goingLeftProbability == 0 && goingRightProbability == 0 && goingUpProbability == 0 && goingDownProbability == 0)
        {
            if (offsetOnX < offsetOnZ)
            {
                if (CheckIfOnTheNav(Vector3.left * navSampleDistance) && offsetOnX < zHDistance.x)
                {
                    goingLeftProbability++;
                }
                else if (CheckIfOnTheNav(Vector3.right * navSampleDistance) && offsetOnX > zHDistance.x)
                {
                    goingRightProbability++;
                }
            }
            else
            {
                if (CheckIfOnTheNav(Vector3.forward * navSampleDistance) && offsetOnZ > zHDistance.z)
                {
                    goingUpProbability++;
                }
                else if (CheckIfOnTheNav(Vector3.back * navSampleDistance) && offsetOnZ < zHDistance.z)
                {
                    goingDownProbability++;
                }
            }
        }

        if (goingLeftProbability > 0)
        {
            isGoingRight = false;
            isGoingLeft = true;
            isGoingDown = false;
            isGoingUp = false;
            runTo = transform.position + Vector3.left * navSampleDistance;
        }
        else if (goingRightProbability > 0)
        {
            isGoingRight = true;
            isGoingDown = false;
            isGoingLeft = false;
            isGoingUp = false;
            runTo = transform.position + Vector3.right * navSampleDistance;
        }
        else if (goingDownProbability > 0)
        {
            isGoingDown = true;
            isGoingRight = false;
            isGoingLeft = false;
            isGoingUp = false;
            runTo = transform.position + Vector3.back * navSampleDistance;
        }
        else if (goingUpProbability > 0)
        {
            isGoingUp = true;
            isGoingDown = false;
            isGoingRight = false;
            isGoingLeft = false;
            runTo = transform.position + Vector3.forward * navSampleDistance;
        }
    }

    private Vector3 GetRandomPatrolPoint()
    {
        return _activeBorder.GetRandomPointInsideBorders();
    }

    private void UpdateRunAfterSpeed(float newspeed, bool hasToReset)
    {
        if (hasToReset)
        {
            runAfterSpeed = initialRunAfterSpeed;
        }
        else
        {
            runAfterSpeed = newspeed;
        }
    }

    public void ResetForNewStage()
    {
        human.UpdateHumanAnimator(Constants.WalkAnimationClipName, 1);
        UpdateHumanAgentSpeed(Constants.SpeedTypes.WalkSpeed);
        transform.position = GetRandomPatrolPoint();
    }

    private void OnDestroy()
    {
        EventsManager.onRunAfterSpeedUpdate -= UpdateRunAfterSpeed;
    }
}
