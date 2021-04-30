using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSign : MonoBehaviour
{
    [SerializeField] private ProjectileThrower projectileThrower;
    [SerializeField] private InputHandler inputHandler;

    public void SetSignPosition(Vector3 position)
    {
        transform.position = new Vector3(position.x, transform.position.y, position.z);
    }

    public void UpdateSignActivation(bool hastoActivate)
    {
        if (hastoActivate)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
