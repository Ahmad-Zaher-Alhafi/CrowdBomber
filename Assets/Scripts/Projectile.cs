using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private float timeToDeactivate;

    private ProjectilesManager projectilesManager;
    private ProjectileThrower projectileThrower;
    private Rigidbody rig;
    private MeshRenderer meshRenderer;
    private int numOfHumansThatBeingHit;
    private GameObject humanThatBeingHit;

    private void Start()
    {
        projectilesManager = FindObjectOfType<ProjectilesManager>();
        projectileThrower = FindObjectOfType<ProjectileThrower>();
        rig = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Constants.GroundLayerNumber)
        {
            meshRenderer.enabled = false;
            rig.useGravity = false;
            rig.velocity = Vector3.zero;

            foreach (ParticleSystem particle in particleSystems)
            {
                particle.Play();
            }

            Invoke("Deactivate", timeToDeactivate);
        }
        else if (other.CompareTag(Constants.HumanTag))
        {
            Human human = other.GetComponent<Human>();

            if (other.gameObject != humanThatBeingHit && !human.IsPoisoned())
            {
                numOfHumansThatBeingHit++;
                humanThatBeingHit = other.gameObject;

                if (numOfHumansThatBeingHit >= 2)
                {
                    human.OrderToShowSpecialText(numOfHumansThatBeingHit);
                }
            }
        }
    }

    private void Deactivate()
    {
        numOfHumansThatBeingHit = 0;
        projectilesManager.UpdateActiveProjectilesNumberInScene(-1);
        EventsManager.OnProjectileDeactivate();
        projectileThrower.EnqueueProjectile(rig);
        meshRenderer.enabled = true;
        gameObject.SetActive(false);
    }
}
