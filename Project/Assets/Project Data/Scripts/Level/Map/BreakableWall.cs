using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class BreakableWall : MonoBehaviour
{
    public Collider physicalCollider;

    public Collider triggerCollider;

    public GameObject intactWall;

    public GameObject fracturedWall;

    public List<Transform> fractureSpawnPoints = new();

    private void Awake()
    {
        fractureSpawnPoints.AddRange(fracturedWall.GetComponentsInChildren<Transform>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<DrivingController>(out var drivingController))
            {
                var canBreakWall = drivingController.TryBreakBreakableWall();
                if (!canBreakWall)
                {
                    return;
                }
                else
                {
                    physicalCollider.enabled = false;
                    triggerCollider.enabled = false;
                    intactWall.SetActive(false);
                    fracturedWall.SetActive(true);
                    foreach (var child in fracturedWall.GetComponentsInChildren<Rigidbody>())
                    {
                        child.isKinematic = false;
                        child.AddExplosionForce(1000f, transform.position, 5f);
                        Physics.IgnoreCollision(child.GetComponent<Collider>(), other);
                    }
                }
            }

        }
    }

    public void ResetWall()
    {
        physicalCollider.enabled = true;
        triggerCollider.enabled = true;
        intactWall.SetActive(true);
        fracturedWall.SetActive(false);
            foreach (var child in fracturedWall.GetComponentsInChildren<Rigidbody>())
            {
                child.isKinematic = true;
                child.transform.position = fractureSpawnPoints[fractureSpawnPoints.IndexOf(child.transform)].position;
                child.transform.rotation = fractureSpawnPoints[fractureSpawnPoints.IndexOf(child.transform)].rotation;
        }
    }
}
