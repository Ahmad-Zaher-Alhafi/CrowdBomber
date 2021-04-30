using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float CameraSmoothSpeed;
    public Vector3 CameraStage1Pos;
    public Vector3 CameraStage2Pos;
    public Vector3 CameraStage3Pos;
    private bool hasToUpdateCameraPos;
    private Vector3 _cameraPosToGoTo;

    // Start is called before the first frame update
    void Start()
    {
        EventsManager.onStageStart += OrderToUpdateCameraPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasToUpdateCameraPos)
        {
            UpdateCameraPos(_cameraPosToGoTo);
        }
    }

    public void OrderToUpdateCameraPos(int stageNumber)
    {
        Vector3 cameraPosToGoTo = transform.position;

        if (stageNumber == 1)
        {
            cameraPosToGoTo = new Vector3(transform.position.x, CameraStage1Pos.y, CameraStage1Pos.z);
        }
        else if (stageNumber == 2)
        {
            cameraPosToGoTo = new Vector3(transform.position.x, CameraStage2Pos.y, CameraStage2Pos.z);
        }
        else if (stageNumber == 3)
        {
            cameraPosToGoTo = new Vector3(transform.position.x, CameraStage3Pos.y, CameraStage3Pos.z);
        }

        hasToUpdateCameraPos = true;
        _cameraPosToGoTo = cameraPosToGoTo;
    }

    public void UpdateCameraPos(Vector3 cameraPosToGoTo)
    {
        if (Vector3.Distance(transform.position, cameraPosToGoTo) > .1f)
        {
            transform.position = Vector3.Lerp(transform.position, cameraPosToGoTo, CameraSmoothSpeed * Time.deltaTime);
        }
        else
        {
            hasToUpdateCameraPos = false;
            gameManager.OnCameraPositionUpdated();
        }
    }

    private void OnDestroy()
    {
        EventsManager.onStageStart -= OrderToUpdateCameraPos;
    }
}
