using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Human : MonoBehaviour
{
    public Material PoisonedMaterial;
    public float HealthDecreaser;
    public float NavSampleDistance;//the distance the will be used to check if there is a place to run to in that distance position
    [Header("The allowed distance (between the zombie that the human running from and the human that is runnnig away) that will be used to decide which direction to choose for escabing")]
    public Vector3 ZHDistance;//the allowed distance (between the zombie that the human running from and the human that is runnnig away) that will use to decide which way to use for escabing
    public Sensor sensor;
    public float WalkingSpeed;
    public float HumanRunningSpeed;
    public Renderer[] MeshesRenderers;
    public float secondsToDisable;//time to disable the dead zombie's body
    public float DelayToGetPosioned;//get posioned after you get hit by the projectile after a delay (it is needed because the projectile collider is big which lead that the human get poisned before the ball reach the graound and explod)
    public float ZombieMoneyValue;
    public Transform HealthSliderPoint;
    public Animator Animator;
    public HumanCanves HumanCanves;

    [HideInInspector] public bool IsPoisened;
    [HideInInspector] public float Health;

    private float zombieRunningSpeed;
    private List<Human> zombiesThatRunnigFrom = new List<Human>();
    private NavMeshAgent agent;
    private bool isDead;
    private GameManager gameManager;
    private bool hasToRunAway;
    private Human zombieToRunFrom;

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

    private Vector3 runTo;//position that the human will check if he can run to

    //the offset between the human and the zombie that is runnig after him
    private float offsetOnX;
    private float offsetOnZ;
    //

    private Rigidbody rig;
    private WaitForSeconds delay;
    private WaitForSeconds getPoisonDelay;
    private NavMeshHit hit;
    private float initialZombieRunningSpeed;
    private float initialHealth;
    private GamePropertiesModifyier gamePropertiesModifyier;
    private Transform RightBorder;
    private Transform LeftBorder;
    private Transform UpBorder;
    private Transform DownBorder;
    private HumanClothing humanClothing;
    private Collider humanCollider;

    void Start()
    {
        EventsManager.onZombieSpeedModifying += UpdateZombieRunningSpeed;
        EventsManager.onZombieHealthModifying += UpdateHealth;

        humanClothing = GetComponent<HumanClothing>();
        gamePropertiesModifyier = FindObjectOfType<GamePropertiesModifyier>();
        rig = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        delay = new WaitForSeconds(secondsToDisable);
        getPoisonDelay = new WaitForSeconds(DelayToGetPosioned);
        gameManager = FindObjectOfType<GameManager>();
        initialZombieRunningSpeed = gamePropertiesModifyier.InitialZombieRunningSpeed;
        initialHealth = gamePropertiesModifyier.InitialHealth;
        humanCollider = GetComponent<Collider>();
        

        InitializeParameters();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (IsPoisened)
        {
            GetClosestHumanPosition();
            DecreaseHealth();
        }

        if (agent.isOnNavMesh && agent.remainingDistance < .3f)
        {
            if (!hasToRunAway)
            {
                agent.SetDestination(GetRandomPatrolPoint());
            }
        }

        if (hasToRunAway)
        {
            RunFrom();
        }
    }


    public void InitializeParameters()
    {
        SetStageBorders();

        isDead = false;
        IsPoisened = false;
        agent.speed = WalkingSpeed;
        Health = gamePropertiesModifyier.ZombieHealth;
        zombieRunningSpeed = gamePropertiesModifyier.ZombieRunningSpeed;


        if (!agent.isOnNavMesh)
        {
            transform.position = RightBorder.position - Vector3.right * 3;
            agent.enabled = false;
            agent.enabled = true;
        }
    }

    public void SetStageBorders()
    {
        if (gamePropertiesModifyier.CurrentStageNumber == 1)
        {
            RightBorder = gameManager.RightBorderStage1;
            LeftBorder = gameManager.LeftBorderStage1;
            UpBorder = gameManager.UpBorderStage1;
            DownBorder = gameManager.DownBorderStage1;
        }
        else if (gamePropertiesModifyier.CurrentStageNumber == 2)
        {
            RightBorder = gameManager.RightBorderStage2;
            LeftBorder = gameManager.LeftBorderStage2;
            UpBorder = gameManager.UpBorderStage2;
            DownBorder = gameManager.DownBorderStage2;
        }
        else if (gamePropertiesModifyier.CurrentStageNumber == 3)
        {
            RightBorder = gameManager.RightBorderStage3;
            LeftBorder = gameManager.LeftBorderStage3;
            UpBorder = gameManager.UpBorderStage3;
            DownBorder = gameManager.DownBorderStage3;
        }
    }

    public void OrderToRunFrom(Human zombieToRunFrom)
    {
        zombiesThatRunnigFrom.Add(zombieToRunFrom);
        agent.speed = HumanRunningSpeed;
        hasToRunAway = true;
        this.zombieToRunFrom = zombieToRunFrom;
        Animator.Play(Constants.RunAnimationClipName);
    }

    public void OrderToStopRunnigAway(Human zombieToRunFrom)
    {
        if (IsPoisened)
        {
            zombiesThatRunnigFrom.Clear();
        }

        if (zombiesThatRunnigFrom.Contains(zombieToRunFrom))
        {
            zombiesThatRunnigFrom.Remove(zombieToRunFrom);
        }

        if (zombiesThatRunnigFrom.Count > 0)
        {
            this.zombieToRunFrom = zombiesThatRunnigFrom[0];
        }
        else
        {
            Animator.speed = 1;
            agent.speed = WalkingSpeed;
            hasToRunAway = false;
            this.zombieToRunFrom = null;
            Animator.Play(Constants.WalkAnimationClipName);
        }
    }

    public void RunFrom()
    {
        if (zombieToRunFrom.isDead)
        {
            OrderToStopRunnigAway(zombieToRunFrom);
            return;
        }

        runTo = Vector3.zero;
        offsetOnX = Mathf.Abs(transform.position.x) - Mathf.Abs(zombieToRunFrom.transform.position.x);
        offsetOnZ = Mathf.Abs(transform.position.z) - Mathf.Abs(zombieToRunFrom.transform.position.z);

        FindEscabeWay();

        if (NavMesh.SamplePosition(runTo, out hit, NavSampleDistance, 1 << NavMesh.GetAreaFromName("Walkable")))
        {
            if (Mathf.Abs(runTo.x) < RightBorder.position.x && Mathf.Abs(runTo.x) > LeftBorder.position.x && Mathf.Abs(runTo.z) < UpBorder.position.z && Mathf.Abs(runTo.z) > DownBorder.position.z)
            { 
                agent.SetDestination(runTo);
            }
            else
            {
                if (isGoingUp)
                {
                    //print("limit up");
                    goingUpProbability--;

                    if (CheckIfOnTheNav(Vector3.left * NavSampleDistance) && Mathf.Abs(offsetOnX) < ZHDistance.x)
                    {
                        goingLeftProbability++;
                    }
                    if(CheckIfOnTheNav(Vector3.right * NavSampleDistance) && Mathf.Abs(offsetOnX) < ZHDistance.x)
                    {
                        goingRightProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.back * NavSampleDistance) && Mathf.Abs(offsetOnZ) < ZHDistance.z)
                    {
                        goingDownProbability++;
                    }
                }
                else if (isGoingDown)
                {
                    //print("limit down");
                    goingDownProbability--;

                    if (CheckIfOnTheNav(Vector3.left * NavSampleDistance) && Mathf.Abs(offsetOnX) < ZHDistance.x)
                    {
                        goingLeftProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.right * NavSampleDistance) && Mathf.Abs(offsetOnX) < ZHDistance.x)
                    {
                        goingRightProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.forward * NavSampleDistance) && Mathf.Abs(offsetOnZ) < ZHDistance.z)
                    {
                        goingUpProbability++;
                    }
                }
                else if (isGoingLeft)
                {
                    //print("limit left");
                    goingLeftProbability--;

                    if (CheckIfOnTheNav(Vector3.right * NavSampleDistance) && Mathf.Abs(offsetOnX) < ZHDistance.x)
                    {
                        goingRightProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.forward * NavSampleDistance) && Mathf.Abs(offsetOnZ) < ZHDistance.z)
                    {
                        goingUpProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.back * NavSampleDistance) && Mathf.Abs(offsetOnZ) < ZHDistance.z)
                    {
                        goingDownProbability++;
                    }
                }
                else if (isGoingRight)
                {
                    //print("limit right");
                    goingRightProbability--;

                    if (CheckIfOnTheNav(Vector3.left * NavSampleDistance) && Mathf.Abs(offsetOnX) < ZHDistance.x)
                    {
                        goingLeftProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.forward * NavSampleDistance) && Mathf.Abs(offsetOnZ) < ZHDistance.z)
                    {
                        goingUpProbability++;
                    }
                    if (CheckIfOnTheNav(Vector3.back * NavSampleDistance) && Mathf.Abs(offsetOnZ) < ZHDistance.z)
                    {
                        goingDownProbability++;
                    }
                }
            }
        }

        //print(isGoingRight + " isGoingRight " + isGoingLeft + " isGoingLeft " + isGoingDown + " isGoingDown " + isGoingUp + " isGoingUp ");
    }

    private bool CheckIfOnTheNav(Vector3 pos)//to check if this position is on the Navmesh
    {
        runTo = transform.position + pos;

        if (NavMesh.SamplePosition(runTo, out hit, NavSampleDistance, 1 << NavMesh.GetAreaFromName("Walkable")))
        {
            if (Mathf.Abs(runTo.x) < RightBorder.position.x && Mathf.Abs(runTo.x) > LeftBorder.position.x && Mathf.Abs(runTo.z) < UpBorder.position.z && Mathf.Abs(runTo.z) > DownBorder.position.z)
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
            if (CheckIfOnTheNav(Vector3.left * NavSampleDistance) && Mathf.Abs(offsetOnX) < ZHDistance.x)
            {
                goingLeftProbability++;
            }
            else if (CheckIfOnTheNav(Vector3.forward * NavSampleDistance) && Mathf.Abs(offsetOnZ) < ZHDistance.z)
            {
                goingUpProbability++;
            }
            else if (CheckIfOnTheNav(Vector3.back * NavSampleDistance) && Mathf.Abs(offsetOnZ) < ZHDistance.z)
            {
                goingDownProbability++;
            }
            else if (CheckIfOnTheNav(Vector3.right * NavSampleDistance) && Mathf.Abs(offsetOnX) < ZHDistance.x)
            {
                goingRightProbability++;
            }
        }

        if (goingLeftProbability > 0)
        {
            //print("left");
            isGoingRight = false;
            isGoingLeft = true;
            isGoingDown = false;
            isGoingUp = false;
            runTo = transform.position + Vector3.left * NavSampleDistance;
        }
        else if (goingRightProbability > 0)
        {
            //print("right");
            isGoingRight = true;
            isGoingDown = false;
            isGoingLeft = false;
            isGoingUp = false;
            runTo = transform.position + Vector3.right * NavSampleDistance;
        }
        else if (goingDownProbability > 0)
        {
            //print("down");
            isGoingDown = true;
            isGoingRight = false;
            isGoingLeft = false;
            isGoingUp = false;
            runTo = transform.position + Vector3.back * NavSampleDistance;
        }
        else if (goingUpProbability > 0)
        {
            //print("up");
            isGoingUp = true;
            isGoingDown = false;
            isGoingRight = false;
            isGoingLeft = false;
            runTo = transform.position + Vector3.forward * NavSampleDistance;
        }

        //print("left " + leftScore + " right " + rightScore + " up " + upScore + " down " + downScore);
    }

    private Vector3 GetRandomPatrolPoint()
    {
        return new Vector3(Random.Range(LeftBorder.position.x, RightBorder.position.x), 0, Random.Range(DownBorder.position.z, UpBorder.position.z));
    }

    private void GetClosestHumanPosition()
    {
        if (gameManager.Humans.Count <= 0)
        {
            return;
        }

        Human closestHuman = gameManager.Humans[0];
        float closestDistance = Mathf.Infinity;

        foreach (Human human in gameManager.Humans)
        {
            if (human != this && !human.IsPoisened && human.isActiveAndEnabled)
            {
                float distance = (human.transform.position - transform.position).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestHuman = human;
                    closestDistance = distance;
                }
            }
        }

        if (closestHuman != null && !closestHuman.IsPoisened)
        {
            agent.SetDestination(closestHuman.transform.position);
        }
    }

    private void DecreaseHealth()
    {
        if (!isDead)
        {
            Health -= Time.deltaTime * HealthDecreaser;
        }

        if (Health <= 0)
        {
            isDead = true;
            agent.enabled = false;
            rig.useGravity = false;
            sensor.gameObject.SetActive(false);
            Animator.speed = 1;
            Animator.Play(Constants.WalkduckedAnimationClipName);
            gameManager.Zombies.Remove(this);
            gameManager.DeadZombies.Add(this);
            StartCoroutine(DisableAfterTime());
        }
    }

    private void GetPoisoned()
    {
        if (IsPoisened)
        {
            return;
        }

        IsPoisened = true;

        foreach (Renderer renderer in MeshesRenderers)
        {
            renderer.material.color = PoisonedMaterial.color;
        }

        humanCollider.enabled = false;
        humanCollider.enabled = true;
        OrderToStopRunnigAway(zombieToRunFrom);
        gamePropertiesModifyier.UpdateMoneyValue(ZombieMoneyValue);
        HumanCanves.gameObject.SetActive(true);
        HumanCanves.ShowMoneyText();
        hasToRunAway = false;
        sensor.gameObject.SetActive(true);
        agent.speed = zombieRunningSpeed;
        Animator.speed = zombieRunningSpeed / 2;
        Animator.Play(Constants.WalkduckedAnimationClipName);
        gameManager.Humans.Remove(this);
        gameManager.Zombies.Add(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead)
        {
            return;
        }

        if (other.CompareTag(Constants.ProjectileTag) && !IsPoisened)
        {
            StartCoroutine(GetPosionAfterDelay());
        }

        if (CompareTag(Constants.HumanTag))
        {
            if (IsPoisened && other.CompareTag(Constants.HumanTag))
            {
                Human human = other.GetComponent<Human>();
                if (!human.IsPoisened)
                {
                    human.GetPoisoned();
                    human.GetClosestHumanPosition();
                }
            }
        }
    }

    private IEnumerator DisableAfterTime()
    {
        yield return delay;
        EventsManager.onZombieDeath();
        gameObject.SetActive(false);
    }

    private IEnumerator GetPosionAfterDelay()
    {
        yield return getPoisonDelay;
        GetPoisoned();
        GetClosestHumanPosition();
    }

    private void UpdateZombieRunningSpeed(bool hasToResetSpeed, float newspeed = 0)
    {
        if (hasToResetSpeed)
        {
            zombieRunningSpeed = initialZombieRunningSpeed;
        }
        else
        {
            zombieRunningSpeed = newspeed; 
        }
    }

    private void UpdateHealth(bool hasToResetSpeed, float newHealth = 0)
    {
        if (hasToResetSpeed)
        {
            Health = initialHealth;
        }
        else
        {
            Health = newHealth;
        }
    }

    public void Reset(bool isItNewLevel)
    {
        humanClothing.CreateRandomclothing();
        InitializeParameters();

        foreach (Renderer renderer in MeshesRenderers)
        {
            renderer.material.color = Color.white;
        }
    }

    private void OnDestroy()
    {
        EventsManager.onZombieSpeedModifying -= UpdateZombieRunningSpeed;
        EventsManager.onZombieHealthModifying -= UpdateHealth;
    }
}
