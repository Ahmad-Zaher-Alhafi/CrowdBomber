using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    //Read this article for better understanding about how to use rayscast to get the mouse position in the world https://gamedevbeginner.com/how-to-convert-the-mouse-position-to-world-space-in-unity-2d-3d/#screen_to_world_3d

    [SerializeField] private MainCanves mainCanves;
    [SerializeField] private TargetSign targetSign;
    [SerializeField] private Tower tower;
    [SerializeField] private ProjectileThrower projectileThrower;

    private Plane plane;
    private Ray ray;
    private Camera mainCamera;
    private float distance;
    private Vector3 inputPosition;


    // Start is called before the first frame update
    void Start()
    {
        plane = new Plane(Vector3.up, 0);//creat a virtual infinent plane
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)) && EventSystem.current.currentSelectedGameObject == null)
        {
            mainCanves.UpdatePropertiesPanelActivationState(false);
        }

        if (projectileThrower.ProjectilesNumber <= 0)
        {
            return;
        }

        if (Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject == null)
        {
            targetSign.UpdateSignActivation(true);
            targetSign.SetSignPosition(GetInputPosition());
            tower.LookTowards(GetInputPosition());
            GetInputPosition();
        }
        else if(Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject == null)
        {
            targetSign.UpdateSignActivation(false);
            projectileThrower.OrderToLaunchProjectile(GetInputPosition());
        }
    }

    public Vector3 GetInputPosition()
    {
        //shot a ray from screen mouse position
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        //if that ray hit the virtual plane store the distance between the camera and the hit postion on the plane
        if (plane.Raycast(ray, out distance))
        {
            //get the point in world position where the ray and virtual plane were collided and asssgine it to the target position
            inputPosition = ray.GetPoint(distance);
            return inputPosition;
        }

        return Vector3.zero;
    }
}
