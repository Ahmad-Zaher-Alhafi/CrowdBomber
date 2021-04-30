using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    [SerializeField] private int borderStageNumber;
    [SerializeField] private Transform rightBorder;
    [SerializeField] private Transform leftBorder;
    [SerializeField] private Transform upBorder;
    [SerializeField] private Transform downBorder;

    public int BorderStageNumber => borderStageNumber;
    public Transform RightBorder => rightBorder;
    public Transform LeftBorder => leftBorder;
    public Transform UpBorder => upBorder;
    public Transform DownBorder => downBorder;

    public bool ChechIfInsideBorder(Vector3 point)
    {
        if (point.x < rightBorder.position.x && point.x > leftBorder.position.x && point.z < upBorder.position.z && point.z > downBorder.position.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateActivationState(bool isActive)
    {
        rightBorder.gameObject.SetActive(isActive);
        leftBorder.gameObject.SetActive(isActive);
        upBorder.gameObject.SetActive(isActive);
        downBorder.gameObject.SetActive(isActive);
    }

    public Vector3 GetRandomPointInsideBorders()
    {
        return new Vector3(Random.Range(leftBorder.position.x, rightBorder.position.x), 0, Random.Range(downBorder.position.z, upBorder.position.z));
    }
}