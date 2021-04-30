using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanves : MonoBehaviour
{
    [SerializeField] private Slider stagesSlider;
    [SerializeField] private TextMeshProUGUI stagesNumTxt;
    [SerializeField] private TextMeshProUGUI moneyTxt;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    [SerializeField] private Image soundButtonImg;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private TextMeshProUGUI healthPropertieCostTxt;
    [SerializeField] private TextMeshProUGUI humansNumPropertieCostTxt;
    [SerializeField] private TextMeshProUGUI projectilesNumPropertieCostTxt;
    [SerializeField] private TextMeshProUGUI runAfterSpeedPropertieCostTxt;
    [SerializeField] private GameObject propertiesPanel;
    [SerializeField] private Image LoseWinPanel;
    [SerializeField] private TextMeshProUGUI LoseWinTxt;
    [SerializeField] private TextMeshProUGUI LevelNameTxt;
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private PropertiesManager propertiesManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        EventsManager.onLevelStart += ResetForNewLevel;

        stagesSlider.maxValue = gameManager.MaxStagesNumInLevel;
        stagesSlider.minValue = 1;
        //UpdateStageSlider(gameManager.StageNumber);

        UpdateProperiteCostTxt(Constants.PropertiesTypes.RunAfterSpeed);
        UpdateProperiteCostTxt(Constants.PropertiesTypes.HumansNum);
        UpdateProperiteCostTxt(Constants.PropertiesTypes.Health);
        UpdateProperiteCostTxt(Constants.PropertiesTypes.ProjectilesNum);
    }

    public void AddRunAfterSpeed()
    {
        if (propertiesManager.TryUpdateRunAfterPropertie())
        {
            UpdateProperiteCostTxt(Constants.PropertiesTypes.RunAfterSpeed);
        } 
    }

    public void AddHuman()
    {
        if (propertiesManager.TryUpdateHumansNumPropertie())
        {
            UpdateProperiteCostTxt(Constants.PropertiesTypes.HumansNum);
        }
    }

    public void AddHealth()
    {
        if (propertiesManager.TryUpdateHealthPropertie())
        {
            UpdateProperiteCostTxt(Constants.PropertiesTypes.Health);
        }
    }

    public void AddProjectile()
    {
        if (propertiesManager.TryUpdateProjectilesNumPropertie())
        {
            UpdateProperiteCostTxt(Constants.PropertiesTypes.ProjectilesNum);
        }
    }

    public void UpdateStageSlider(int StageNum)
    {
        stagesSlider.value = StageNum;
        stagesNumTxt.text = "Stage " + StageNum + "/" + gameManager.MaxStagesNumInLevel;
    }

    public void UpdateMoneyTxt(float newMoneyAmount)
    {
        moneyTxt.text = "$" + newMoneyAmount.ToString();
    }

    public void UpdateProperiteCostTxt(Constants.PropertiesTypes propertieType)
    {
        switch (propertieType)
        {
            case Constants.PropertiesTypes.RunAfterSpeed:
                runAfterSpeedPropertieCostTxt.text = "$" + propertiesManager.GetPropertieCost(propertieType).ToString();
                break;
            case Constants.PropertiesTypes.HumansNum:
                humansNumPropertieCostTxt.text = "$" + propertiesManager.GetPropertieCost(propertieType).ToString();
                break;
            case Constants.PropertiesTypes.Health:
                healthPropertieCostTxt.text = "$" + propertiesManager.GetPropertieCost(propertieType).ToString();
                break;
            case Constants.PropertiesTypes.ProjectilesNum:
                projectilesNumPropertieCostTxt.text = "$" + propertiesManager.GetPropertieCost(propertieType).ToString();
                break;
            default:
                break;
        }
    }

    public void TurnOnOffSounds()
    {
        if (soundButtonImg.sprite == soundOnSprite)
        {
            audioManager.UpdateAudioMuteState(true);
            soundButtonImg.sprite = soundOffSprite;
        }
        else
        {
            audioManager.UpdateAudioMuteState(false);
            soundButtonImg.sprite = soundOnSprite;
        }
    }

    public void ShowHideSettingsPanel()
    {
        if (settingsPanel.activeInHierarchy)
        {
            settingsPanel.SetActive(false);
        }
        else
        {
            settingsPanel.SetActive(true);
        }
    }

    public void UpdatePropertiesPanelActivationState(bool isActive)
    {
        propertiesPanel.SetActive(isActive);
    }

    private void ResetForNewLevel()
    {
        runAfterSpeedPropertieCostTxt.text = "$" + propertiesManager.GetPropertieInitialCost(Constants.PropertiesTypes.RunAfterSpeed).ToString();
        humansNumPropertieCostTxt.text = "$" + propertiesManager.GetPropertieInitialCost(Constants.PropertiesTypes.HumansNum).ToString();
        healthPropertieCostTxt.text = "$" + propertiesManager.GetPropertieInitialCost(Constants.PropertiesTypes.Health).ToString();
        projectilesNumPropertieCostTxt.text = "$" + propertiesManager.GetPropertieInitialCost(Constants.PropertiesTypes.ProjectilesNum).ToString();
    }

    public void UpdateLevelNameTxt(string levelName)
    {
        LevelNameTxt.text = levelName;
    }

    public void UpdateLoseWinPanel(bool isActive, bool haswon, bool hasWonStage)
    {
        if (!isActive)
        {
            LoseWinPanel.gameObject.SetActive(isActive);
            return;
        }

        LoseWinPanel.gameObject.SetActive(isActive);

        if (haswon)
        {
            if (hasWonStage)
            {
                LoseWinTxt.text = "Congrats!\n\nStage Was Completed";
            }
            else
            {
                LoseWinTxt.text = "Congrats!\n\nLevel Was Completed";
            }
        }
        else
        {
            LoseWinTxt.text = "You Lose!\n\nTry Again";
        }
    }

    private void OnDestroy()
    {
        EventsManager.onLevelStart -= ResetForNewLevel;
    }
}