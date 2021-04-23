using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public List<Human> Humans;
    [HideInInspector] public List<Human> DeadZombies;
    [HideInInspector] public List<Human> Zombies;
    [HideInInspector] public bool IsStartingNextStage;
    public int LevelNumber;
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
    public Level[] Levels;
    public TextMeshProUGUI LevelNameTxt;
    public GameObject BottomPowerUpsPanel;
    public Image LoseWinPanel;
    public TextMeshProUGUI LoseWinTxt;
    public ProjectileThrower ProjectileThrower;

    private Transform mainCamera;
    private int currentStageNumber;
    private bool hasToUpdateCameraPos;
    private Vector3 cameraPosToGoTo;
    private bool isItFirstTimeLaunch;//if the player launched the game for the first time after closing it
    private WaitForSeconds delay;
    private int currentLevelNumber;
    private int oldStageNumber;
    private AudioManager audioManager;


    [System.Serializable]
    public class Level
    {
        public GameObject LevelObject;
        public string LevelName;
    }

    private void Awake()
    {
        EventsManager.onZombieDeath += CheckLoseWinState;
        EventsManager.onProjectileDeactivate += CheckLoseWinState;

        audioManager = FindObjectOfType<AudioManager>();
        mainCamera = Camera.main.transform;
        isItFirstTimeLaunch = true;
        delay = new WaitForSeconds(SecondstoStartNextStage);
        oldStageNumber = 0;
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)) && EventSystem.current.currentSelectedGameObject == null)
        {
            BottomPowerUpsPanel.SetActive(false);
        }

        if (hasToUpdateCameraPos)
        {
            if (Vector3.Distance(mainCamera.position, cameraPosToGoTo) > .1f)
            {
                mainCamera.position = Vector3.Lerp(mainCamera.position, cameraPosToGoTo, CameraSmoothSpeed * Time.deltaTime);
            }
            else
            {
                ProjectileThrower.gameObject.SetActive(true);
                hasToUpdateCameraPos = false;
                if (currentStageNumber != oldStageNumber)
                {
                    oldStageNumber = currentStageNumber;
                    NavMeshSurface.BuildNavMesh();//build the navmesh after we changed the stage borders
                }
                BottomPowerUpsPanel.SetActive(true);

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


    public void ActivateLevel(int LevelNumber)
    {
        foreach (Level level in Levels)
        {
            level.LevelObject.SetActive(false);
        }

        currentLevelNumber = LevelNumber;
        Levels[LevelNumber - 1].LevelObject.SetActive(true);
        LevelNameTxt.text = Levels[LevelNumber - 1].LevelName;
    }

    /// <summary>
    /// active the humans again after they being disabled in the previous stage
    /// </summary>
    private void ResetHumans()
    {
        while (Humans.Count < gamePropertiesModifyier.CurrentNumOfHumansInLevel)
        {
            if (DeadZombies.Count > 0)
            {
                Humans.Add(DeadZombies[0]);
                DeadZombies[0].Reset();
                DeadZombies[0].transform.position = gamePropertiesModifyier.GetRandomCreatPoint();
                DeadZombies[0].gameObject.SetActive(true);
                DeadZombies.Remove(DeadZombies[0]);
            }
            else
            {
                return;
            }
        }
    }

    /// <summary>
    /// Change the camera position on y and z axis according to the stage number
    /// </summary>
    public void SetCameraStagePos(int stageNumber)
    {
        ProjectileThrower.gameObject.SetActive(false);
        currentStageNumber = stageNumber;

        if (stageNumber == 1)
        {
            cameraPosToGoTo = new Vector3(mainCamera.position.x, CameraStage1Pos.y, CameraStage1Pos.z);
            SetBordersActivation(true, false, false);
        }
        else if (stageNumber == 2)
        {
            cameraPosToGoTo = new Vector3(mainCamera.position.x, CameraStage2Pos.y, CameraStage2Pos.z);
            SetBordersActivation(false, true, false);
        }
        else if (stageNumber == 3)
        {
            cameraPosToGoTo = new Vector3(mainCamera.position.x, CameraStage3Pos.y, CameraStage3Pos.z);
            SetBordersActivation(false, false, true);
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

    public IEnumerator StartNextStage(bool hasWon)
    {
        IsStartingNextStage = true;
        yield return delay;

        if (hasWon)
        {
            currentStageNumber++;
        }
        IsStartingNextStage = false;
        LoseWinPanel.gameObject.SetActive(false);
        
        if (currentStageNumber > MaxStagesNumInLevel)
        {
            Levels[currentLevelNumber - 1].LevelObject.SetActive(false);
            currentLevelNumber++;

            if (currentLevelNumber > Levels.Length)
            {
                currentLevelNumber = 1;
            }

            Levels[currentLevelNumber - 1].LevelObject.SetActive(true);
            LevelNameTxt.text = Levels[currentLevelNumber - 1].LevelName;

            currentStageNumber = 1;
            gamePropertiesModifyier.PrepareNextStage(true, currentStageNumber, currentLevelNumber);
        }
        else
        {
            gamePropertiesModifyier.PrepareNextStage(false, currentStageNumber, currentLevelNumber);
        }

        MainCanves.UpdateStageSlider(currentStageNumber);
    }

    public void CheckLoseWinState()
    {
        if (Humans.Count == 0 && Zombies.Count == 0)
        {
            Win();
        }
        else if (gamePropertiesModifyier.ProjectilesNumber <= 0 && Zombies.Count == 0 && ProjectileThrower.NumOfActiveProjectilesInScene == 0)
        {
            Lose();
        }
    }

    public void Win()
    {
        if (currentStageNumber == MaxStagesNumInLevel)
        {
            StartCoroutine(audioManager.SetMainMusicVolume(.5f,1.5f));
            audioManager.PlayLevelWinSound();
            LoseWinTxt.text = "Congrats!\n\nStage Was Completed";
        }
        else
        {
            StartCoroutine(audioManager.SetMainMusicVolume(.5f, 1.5f));
            audioManager.PlayStageWinSound();
            LoseWinTxt.text = "Congrats!\n\nLevel Was Completed";
        }
        
        LoseWinPanel.gameObject.SetActive(true);
        LoseWinTxt.gameObject.SetActive(true);
        StartCoroutine(StartNextStage(true));
    }

    public void Lose()
    {
        LoseWinTxt.text = "You Lose!\n\nTry Again";
        LoseWinPanel.gameObject.SetActive(true);
        LoseWinTxt.gameObject.SetActive(true);
        StartCoroutine(StartNextStage(false));
    }

    private void OnDestroy()
    {
        EventsManager.onZombieDeath -= CheckLoseWinState;
        EventsManager.onProjectileDeactivate -= CheckLoseWinState;
    }
}
