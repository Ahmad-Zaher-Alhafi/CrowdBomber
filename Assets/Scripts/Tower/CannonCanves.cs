using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CannonCanves : MonoBehaviour
{
    [SerializeField] private PropertiesManager propertiesManager;
    [SerializeField] private TextMeshProUGUI projectilesNumTxt;

    private void Start()
    {
        UpdateProjectilesNumTxt((int)propertiesManager.GetPropertieValue(Constants.PropertiesTypes.ProjectilesNum));
    }

    public void UpdateProjectilesNumTxt(int projecilesNum)
    {
        projectilesNumTxt.text = projecilesNum.ToString();
    }
}