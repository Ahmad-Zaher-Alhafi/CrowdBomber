using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public List<Human> Humans;
    [HideInInspector] public List<Human> DeadZombies;
    [HideInInspector] public List<Human> Zombies;
    public int StageNumber;
    public Transform RightBorderStage1;
    public Transform LeftBorderStage1;
    public Transform UpBorderStage1;
    public Transform DownBorderStage1;
    public Transform RightBorderStage2;
    public Transform LeftBorderStage2;
    public Transform UpBorderStage2;
    public Transform DownBorderStage2;
    public Transform RightBorderStage3;
    public Transform LeftBorderStage3;
    public Transform UpBorderStage3;
    public Transform DownBorderStage3;
    public GamePropertiesModifyier gamePropertiesModifyier;
    public Vector3 CameraStage1Pos;
    public Vector3 CameraStage2Pos;
    public Vector3 CameraStage3Pos;
    public NavMeshSurface NavMeshSurface;
    public float CameraSmoothSpeed;
    public int MaxStagesNumInLevel;
    public MainCanves MainCanves;
    public float SecondstoStartNextStage;
    public GameObject[] Levels;

    private Transform mainCamera;
    private int currentStageNumber;
    private bool hasToUpdateCameraPos;
    private Vector3 cameraPosToGoTo;
    private bool isItFirstTimeLaunch;//if the player launched the game for the first time after closing it
    private WaitForSeconds delay;


    private void Awake()
    {
        EventsManager.onZombieDeath += CheckLoseWinState;
        EventsManager.onProjectileDeactivate += CheckLoseWinState;

        mainCamera = Camera.main.transform;
        isItFirstTimeLaunch = true;
        delay = new WaitForSeconds(SecondstoStartNextStage);
        //SetCameraStagePos(StageNumber);
    }

    private void Update()
    {
        if (hasToUpdateCameraPos)
        {
            if (Vector3.Distance(mainCamera.position, cameraPosToGoTo) > .1f)
            {
                mainCamera.position = Vector3.Lerp(mainCamera.position, cameraPosToGoTo, CameraSmoothSpeed * Time.deltaTime);
            }
            else
            {
                hasToUpdateCameraPos = false;
                NavMeshSurface.BuildNavMesh();

                if (isItFirstTimeLaunch)
                {
                    gamePropertiesModifyier.CreateNeededHumans();
                    isItFirstTimeLaunch = false;
                }
                else
                {
                    ResetHumans();
                }
            }
        }
    }


    /// <summary>
    /// active the humans again after they being disabled in the previous stage
    /// </summary>
    private void ResetHumans()
    {
        while (Humans.Count < gamePropertiesModifyier.CurrentNumOfHumansInLevel)
        {
            Humans.Add(DeadZombies[0]);
            DeadZombies[0].Reset(false);
            DeadZombies[0].transform.position = gamePropertiesModifyier.GetRandomCreatPoint();
            DeadZombies[0].gameObject.SetActive(true);
            DeadZombies.Remove(DeadZombies[0]);
        }
        //DeadZombies.Clear();
    }

    /// <summary>
    /// Change the camera position on y and z axis according to the stage number
    /// </summary>
    public void SetCameraStagePos(int stageNumber)
    {
        if (stageNumber == 1)
        {
            cameraPosToGoTo = new Vector3(mainCamera.position.x, CameraStage1Pos.y, CameraStage1Pos.z);
            SetBordersActivation(true, false, false);
            currentStageNumber = 1;
        }
        else if (stageNumber == 2)
        {
            cameraPosToGoTo = new Vector3(mainCamera.position.x, CameraStage2Pos.y, CameraStage2Pos.z);
            SetBordersActivation(false, true, false);
            currentStageNumber = 2;
        }
        else if (stageNumber == 3)
        {
            cameraPosToGoTo = new Vector3(mainCamera.position.x, CameraStage3Pos.y, CameraStage3Pos.z);
            SetBordersActivation(false, false, true);
            currentStageNumber = 3;
        }

        hasToUpdateCameraPos = true;
    }

    /// <summary>
    /// To actiavate or deactivate the borders according to the current stage
    /// </summary>
    /// <param name="stage1">true to activate first stage borders else false</param>
    /// <param name="stage2">true to activate second stage borders else false</param>
    /// <param name="stage3">true to activate third stage borders else false</param>
    private void SetBordersActivation(bool stage1, bool stage2, bool stage3)
    {
        RightBorderStage1.gameObject.SetActive(stage1);
        LeftBorderStage1.gameObject.SetActive(stage1);
        UpBorderStage1.gameObject.SetActive(stage1);
        DownBorderStage1.gameObject.SetActive(stage1);
        RightBorderStage2.gameObject.SetActive(stage2);
        LeftBorderStage2.gameObject.SetActive(stage2);
        UpBorderStage2.gameObject.SetActive(stage2);
        DownBorderStage2.gameObject.SetActive(stage2);
        RightBorderStage3.gameObject.SetActive(stage3);
        LeftBorderStage3.gameObject.SetActive(stage3);
        UpBorderStage3.gameObject.SetActive(stage3);
        DownBorderStage3.gameObject.SetActive(stage3);
    }

    private IEnumerator StartStage(bool hasWon)
    {
        yield return delay;

        if (hasWon)
        {
            currentStageNumber++;
        }
        
        if (currentStageNumber > MaxStagesNumInLevel)
        {
            currentStageNumber = 1;
            gamePropertiesModifyier.PrepareNextStage(true, currentStageNumber);
        }
        else
        {
            gamePropertiesModifyier.PrepareNextStage(false, currentStageNumber);
        }

        MainCanves.UpdateStageSlider(currentStageNumber);
    }

    public void CheckLoseWinState()
    {
        if (Humans.Count == 0 && Zombies.Count == 0)
        {
            Win();
        }
        else if (gamePropertiesModifyier.ProjectilesNumber <= 0 && Zombies.Count == 0)
        {
            Lose();
        }
    }

    public void Win()
    {
        print("You Win!");
        StartCoroutine(StartStage(true));
    }

    public void Lose()
    {
        print("You Lose!");
        StartCoroutine(StartStage(false));
    }

    private void OnDestroy()
    {
        EventsManager.onZombieDeath -= CheckLoseWinState;
        EventsManager.onProjectileDeactivate -= CheckLoseWinState;
    }
}
