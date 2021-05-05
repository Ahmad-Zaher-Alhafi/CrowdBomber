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
    [Header("Check it if you want to delete the saved data")]
    [SerializeField] private bool hasToDeleteSaveFiles;
    [SerializeField] private Tower tower;
    [SerializeField] private ProjectilesManager projectilesManager;
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private HumansManager humansManager;
    [SerializeField] private int maxStagesNumInLevel;
    [SerializeField] private MainCanves mainCanves;
    [SerializeField] private float timeToStartNextStage;
    [SerializeField] private Level[] levels;
    [SerializeField] private ProjectileThrower projectileThrower;
    [SerializeField] private BordersManger bordersManager;
    [SerializeField] private int levelNumber = 1;
    [SerializeField] private int stageNumber = 1;
    [SerializeField] private DataSaver dataSaver;

    public int StageNumber
    {
        get => stageNumber;
        private set => stageNumber = value;
    }
    public int MaxStagesNumInLevel => maxStagesNumInLevel;

    private WaitForSeconds delayBeforeStartStage;
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
        if (hasToDeleteSaveFiles)
        {
            PlayerPrefs.DeleteAll();
        }

        EventsManager.onHumanDeath += CheckLoseWinState;
        EventsManager.onProjectileDeactivate += CheckLoseWinState;

        if (PlayerPrefsManager.CheckForDataName(Constants.DataNames.LevelNumber.ToString()))
        {
            levelNumber = (int)PlayerPrefsManager.LoadFloat(Constants.DataNames.LevelNumber.ToString());
        }

        if (PlayerPrefsManager.CheckForDataName(Constants.DataNames.StageNumber.ToString()))
        {
            StageNumber = (int)PlayerPrefsManager.LoadFloat(Constants.DataNames.StageNumber.ToString());
        }
        
        audioManager = FindObjectOfType<AudioManager>();

        bordersManager.SetBordersActivation(StageNumber);
        
        delayBeforeStartStage = new WaitForSeconds(timeToStartNextStage);
        oldStageNumber = 0;
    }

    private void Start()
    {
        ActivateLevel(levelNumber);
        StartCoroutine(StartNextStage(false, 0));
    }

    /// <summary>
    /// this function will be called by the camera when it finishes updating it's position
    /// </summary>
    public void OnCameraPositionUpdated()
    {
        tower.UpdateActivationState(true);

        //build the navmesh after we changed the stage borders if the stage changed
        if (StageNumber != oldStageNumber)
        {
            oldStageNumber = StageNumber;
            navMeshSurface.BuildNavMesh();
        }

        mainCanves.UpdatePropertiesPanelActivationState(true);
        humansManager.CreateNeededHumans();
    }

    public void ActivateLevel(int levelNumber)
    {
        foreach (Level level in levels)
        {
            level.LevelObject.SetActive(false);
        }

        levels[levelNumber - 1].LevelObject.SetActive(true);
        mainCanves.UpdateLevelNameTxt(levels[levelNumber - 1].LevelName);
    }

    public IEnumerator StartNextStage(bool hasWon, float timeToStartNextStage)
    {
        tower.UpdateActivationState(false);

        yield return new WaitForSeconds(timeToStartNextStage);

        if (hasWon)
        {
            StageNumber++;

            if (StageNumber > MaxStagesNumInLevel)
            {
                StartNextLevel();
                StageNumber = 1;
            }

            dataSaver.AddDataToBeSaved(Constants.DataNames.StageNumber, StageNumber);
        }

        mainCanves.UpdateLoseWinPanel(false, false, false);
        EventsManager.OnStageStart(StageNumber);
        mainCanves.UpdateStageSlider(StageNumber);
    }

    public void CheckLoseWinState()
    {
        if (humansManager.GetListCount(Constants.ListsNames.Humans) == 0 && humansManager.GetListCount(Constants.ListsNames.PosionedHumans) == 0)
        {
            Win();
        }
        else if (projectileThrower.ProjectilesNumber <= 0 && humansManager.GetListCount(Constants.ListsNames.PosionedHumans) == 0 && projectilesManager.NumOfActiveProjectilesInScene == 0)
        {
            Lose();
        }
    }

    public void Win()
    {
        if (StageNumber == MaxStagesNumInLevel)
        {
            audioManager.OrderToSetMainMusicVolume(.5f,1.5f);
            audioManager.PlayLevelWinSound();
            mainCanves.UpdateLoseWinPanel(true, true, false);
        }
        else
        {
            audioManager.OrderToSetMainMusicVolume(.5f, 1.5f);
            audioManager.PlayStageWinSound();
            mainCanves.UpdateLoseWinPanel(true, true, true);
        }
        
        StartCoroutine(StartNextStage(true, timeToStartNextStage));
    }

    public void Lose()
    {
        mainCanves.UpdateLoseWinPanel(true, false, false);

        StartCoroutine(StartNextStage(false, timeToStartNextStage));
    }

    public void StartNextLevel()
    {
        levels[levelNumber - 1].LevelObject.SetActive(false);
        levelNumber++;

        if (levelNumber > levels.Length)
        {
            levelNumber = 1;
        }

        levels[levelNumber - 1].LevelObject.SetActive(true);
        mainCanves.UpdateLevelNameTxt(levels[levelNumber - 1].LevelName);
        dataSaver.AddDataToBeSaved(Constants.DataNames.LevelNumber, levelNumber);
        EventsManager.OnLevelStart();
    }

    private void OnDestroy()
    {
        EventsManager.onHumanDeath -= CheckLoseWinState;
        EventsManager.onProjectileDeactivate -= CheckLoseWinState;
    }
}