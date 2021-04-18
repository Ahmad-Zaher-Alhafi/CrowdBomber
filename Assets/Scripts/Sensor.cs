using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public Human Human;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.HumanTag) && other.gameObject != Human.gameObject)
        {
            Human human = other.GetComponent<Human>();
            if (!human.IsPoisened)
            {
                //print("in");
                human.OrderToRunFrom(Human);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.HumanTag) && other.gameObject != Human.gameObject)
        {
            Human human = other.GetComponent<Human>();
            if (!human.IsPoisened)
            {
                //print("Out");
                human.OrderToStopRunnigAway(Human);
            }
        }
    }
}
