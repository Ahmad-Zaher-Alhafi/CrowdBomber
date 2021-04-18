using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionSensor : MonoBehaviour
{
    public bool IsBlocked;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.WallTag))
        {
            IsBlocked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.WallTag))
        {
            IsBlocked = false;
        }
    }
}
