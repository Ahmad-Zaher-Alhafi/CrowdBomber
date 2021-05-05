using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrower : MonoBehaviour
{
    //This code will threw a ball with an arc path according the mouse click position
    //This code is using the Kinematic Equations To understand this code more watch this video https://www.youtube.com/watch?v=IvT8hjy6q4o

    [SerializeField] private CannonCanves cannonCanves;
    [SerializeField] private ProjectilesManager projectilesManager;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private PropertiesManager propertiesManager;
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private Transform shootingFromPoint;
    //this height can be changed according to your needs
    [SerializeField] private float height = 5;
    //gravity can be changed also according to what you need
    [SerializeField] private float gravity = -18;
    [SerializeField] private Transform projectilesParent;

    private int initialProjectilesNumber;
    private int projectilesNumber;
    public int ProjectilesNumber
    {
        get => projectilesNumber;
        private set => projectilesNumber = value;
    }

    private Queue<Rigidbody> projectiles = new Queue<Rigidbody>();
    private Vector3 targetPosition = Vector3.positiveInfinity;
    private AudioManager audioManager;

    void Start()
    {
        EventsManager.onProjectilesNumPropertieUpdate += UpdateProjectilesNumber;
        EventsManager.onStageStart += ResetFroNextStage;

        audioManager = FindObjectOfType<AudioManager>();
        ProjectilesNumber = (int)propertiesManager.GetPropertieValue(Constants.PropertiesTypes.ProjectilesNum);
        initialProjectilesNumber = ProjectilesNumber;
        //this line will change the phsyics gravity accorsing to your code gravity
        Physics.gravity = Vector3.up * gravity;
    }

    public void OrderToLaunchProjectile(Vector3 targetPosition)
    {
        if (targetPosition != Vector3.positiveInfinity /*&& !GameManager.IsStartingNextStage*/)
        {
            this.targetPosition = targetPosition;
            audioManager.PlayCannonSound();
            projectilesManager.UpdateActiveProjectilesNumberInScene(1);
            LaunchProjectile();
            ProjectilesNumber--;
            //reset the position variable
            this.targetPosition = Vector3.positiveInfinity;
            cannonCanves.UpdateProjectilesNumTxt(ProjectilesNumber);
        }
    }

    public void LaunchProjectile()
    {
        Rigidbody objectToThrowRig = GetProjectile();
        objectToThrowRig.useGravity = true;
        objectToThrowRig.velocity = CalculateLaunchVelocity(objectToThrowRig.transform);
    }

    private Vector3 CalculateLaunchVelocity(Transform ProjectileToLaunch)
    {
        //the displacement on y axis is the distince between the ball y position and the target y position
        float displacementY = targetPosition.y - ProjectileToLaunch.position.y;
        //same for displacements on x and z axises
        Vector3 displacementXZ = new Vector3(targetPosition.x - ProjectileToLaunch.position.x, 0, targetPosition.z - ProjectileToLaunch.position.z);
        //the start velocity on x and z axis can be calculated using the Kinematic Equations
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (displacementY - height) / gravity));
        //same thing for velocity on y axis
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        //the result of the start velocity on 3 axises
        return velocityXZ + velocityY;
    }
    private Rigidbody GetProjectile()
    {
        if (projectiles.Count > 0)
        {
            return ResetUsedProjectile(DequeueProjectile());
        }
        else
        {
            Rigidbody projectile = Instantiate(projectilePrefab, shootingFromPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            projectile.transform.parent = projectilesParent;
            return projectile;
        }
    }

    private Rigidbody ResetUsedProjectile(Rigidbody usedProjectile)
    {
        usedProjectile.transform.position = shootingFromPoint.position;
        usedProjectile.useGravity = false;
        usedProjectile.gameObject.SetActive(true);
        return usedProjectile;
    }

    public void EnqueueProjectile(Rigidbody projectileRig)
    {
        projectiles.Enqueue(projectileRig);
    }

    public Rigidbody DequeueProjectile()
    {
        return projectiles.Dequeue();
    }

    private void UpdateProjectilesNumber(int newProjectilesNumber, bool hasToReset)
    {
        if (hasToReset)
        {
            ProjectilesNumber = initialProjectilesNumber;
        }
        else
        {
            ProjectilesNumber = newProjectilesNumber;
        }
        
        cannonCanves.UpdateProjectilesNumTxt(ProjectilesNumber);
    }

    public void ResetFroNextStage(int ignore)
    {
        UpdateProjectilesNumber((int)propertiesManager.GetPropertieValue(Constants.PropertiesTypes.ProjectilesNum), false);
    }

    private void OnDestroy()
    {
        EventsManager.onProjectilesNumPropertieUpdate += UpdateProjectilesNumber;
    }
}