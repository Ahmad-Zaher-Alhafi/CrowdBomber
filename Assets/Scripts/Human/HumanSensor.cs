using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSensor : MonoBehaviour
{
    [SerializeField] private Human human;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.HumanTag) && other.gameObject != human.gameObject)
        {
            Human human = other.GetComponent<Human>();
            if (!human.IsPoisoned())
            {
                human.OrderToRunFrom(this.human);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.HumanTag) && other.gameObject != human.gameObject)
        {
            Human human = other.GetComponent<Human>();
            if (!human.IsPoisoned())
            {
                human.RemoveHumanThatRunAwayFrom(this.human);
            }
        }
    }

    public void ResetForNextStage()
    {
        gameObject.SetActive(false);
    }
}
