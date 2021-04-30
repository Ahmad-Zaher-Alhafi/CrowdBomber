using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private Transform cannon;
    [SerializeField] private InputHandler inputHandler;

    private Vector3 initialCannonEuralAngle;

    // Start is called before the first frame update
    void Start()
    {
        initialCannonEuralAngle = cannon.eulerAngles;
    }

    public void LookTowards(Vector3 pointToLookAt)
    {
        cannon.LookAt(pointToLookAt);
        //aplly constraints on the cannon rotaion 
        cannon.eulerAngles = new Vector3(initialCannonEuralAngle.x, cannon.eulerAngles.y, initialCannonEuralAngle.z);

    } 

    public void UpdateActivationState(bool isItActive)
    {
        gameObject.SetActive(isItActive);
    }
}
