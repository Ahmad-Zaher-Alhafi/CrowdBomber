using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileThrower : MonoBehaviour
{
    //this code will threw a ball with an arc path according the mouse click position
    //To understand this code more watch this video https://www.youtube.com/watch?v=IvT8hjy6q4o
    //Read this article for better understanding about how to use rayscast to get the mouse position in the world https://gamedevbeginner.com/how-to-convert-the-mouse-position-to-world-space-in-unity-2d-3d/#screen_to_world_3d
    //this code is using the Kinematic Equations

    public GamePropertiesModifyier GamePropertiesModifyier;
    public Transform ProjectilePrefab;
    public Transform ShootingFromPoint;
    public float Height = 5;//this height can be changed according to your needs
    public float Gravity = -18;//gravity can be changed also according to what you need
    public Transform TargetSign;
    public Transform Cannon;
    public Transform CannonBase;
    public Transform ProjectilesParent;
    public int InitialProjectilesNumber;
    [HideInInspector] public Queue<Rigidbody> Projectiles = new Queue<Rigidbody>();

    private Vector3 initialCannonBaseEuralAngle;
    private Vector3 initialCannonEuralAngle;
    private Vector3 targetPosition = Vector3.positiveInfinity;
    private Plane plane;
    private Ray ray;
    private Camera mainCamera;
    private float distance;
    private int projectilesNumber;

    void Start()
    {
        projectilesNumber = GamePropertiesModifyier.ProjectilesNumber;
        initialCannonBaseEuralAngle = CannonBase.eulerAngles;
        initialCannonEuralAngle = Cannon.eulerAngles;
        mainCamera = Camera.main;
        plane = new Plane(Vector3.up, 0);//creat a virtual infinent plane
        Physics.gravity = Vector3.up * Gravity;//this line will change the phsyics gravity accorsing to your code gravity
    }

    void Update()
    {
        GetMousePosition();
        LaunchProjectile();
    }

    private Vector3 CalculateLaunchVelocity(Transform ProjectileToLaunch)
    {
        float displacementY = targetPosition.y - ProjectileToLaunch.position.y;//the displacement on y axis is the distince between the ball y position and the target y position
        Vector3 displacementXZ = new Vector3(targetPosition.x - ProjectileToLaunch.position.x, 0, targetPosition.z - ProjectileToLaunch.position.z);//same for displacements on x and z axises

        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * Height / Gravity) + Mathf.Sqrt(2 * (displacementY - Height) / Gravity));//the start velocity on x and z axis can be calculated using the Kinematic Equations
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * Gravity * Height);//same thing for velocity on y axis

        return velocityXZ + velocityY;//the result of the start velocity on 3 axises
    }

    private void GetMousePosition()
    {
        if (Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject == null)
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);//shot a ray from screen mouse position
            if (plane.Raycast(ray, out distance))//if that ray hit the virtual plane store the distance between the camera and the hit postion on the plane
            {
                targetPosition = ray.GetPoint(distance);//get the point in world position where the ray and virtual plane were collided and asssgine it to the target position
                TargetSign.gameObject.SetActive(true);
                TargetSign.position = new Vector3(targetPosition.x, TargetSign.position.y, targetPosition.z);
                Cannon.LookAt(targetPosition);
                Cannon.eulerAngles = new Vector3(Cannon.eulerAngles.x, CannonBase.eulerAngles.y, initialCannonEuralAngle.z);//aplly constraints on the cannon rotaion 
                CannonBase.LookAt(targetPosition);
                CannonBase.eulerAngles = new Vector3(initialCannonBaseEuralAngle.x, CannonBase.eulerAngles.y, initialCannonBaseEuralAngle.z);//aplly constraints on the cannon base rotaion 
            }
        }
    }

    private void LaunchProjectile()
    {
        if (projectilesNumber <= 0)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject == null && targetPosition != Vector3.positiveInfinity)
        {
            Rigidbody objectToThrowRig = GetProjectile();
            objectToThrowRig.useGravity = true;
            objectToThrowRig.velocity = CalculateLaunchVelocity(objectToThrowRig.transform);
            targetPosition = Vector3.positiveInfinity;//reset the position variable
            TargetSign.gameObject.SetActive(false);
            projectilesNumber--;
            GamePropertiesModifyier.UpdateProjectilesNumber(false,false, -1);
        }
    }

    private Rigidbody GetProjectile()
    {
        if (Projectiles.Count > 0)
        {
            return ResetUsedProjectile(Projectiles.Dequeue());
        }
        else
        {
            Rigidbody projectile = Instantiate(ProjectilePrefab, ShootingFromPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            projectile.transform.parent = ProjectilesParent;
            return projectile;
        }
    }

    private Rigidbody ResetUsedProjectile(Rigidbody usedProjectile)
    {
        usedProjectile.transform.position = ShootingFromPoint.position;
        usedProjectile.useGravity = false;
        usedProjectile.gameObject.SetActive(true);
        return usedProjectile;
    }

    public void UpdateProjectilesNumber(bool hasToReset, int projectilesNumber = 1)
    {
        if (hasToReset)
        {
            this.projectilesNumber = InitialProjectilesNumber;
        }
        else
        {
            this.projectilesNumber = projectilesNumber;
        }
    }
}