using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ParticleSystem[] particleSystems;
    public float SecondsToDeactivate;

    private ProjectileThrower projectileThrower;
    private Rigidbody rig;
    private WaitForSeconds delay;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        projectileThrower = FindObjectOfType<ProjectileThrower>();
        rig = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        delay = new WaitForSeconds(SecondsToDeactivate);
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
            StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait()
    {
        yield return delay;
        projectileThrower.NumOfActiveProjectilesInScene--;
        EventsManager.OnProjectileDeactivate();
        gameObject.SetActive(false);
        meshRenderer.enabled = true;
        projectileThrower.Projectiles.Enqueue(rig);
    }
}
