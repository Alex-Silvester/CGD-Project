using UnityEngine;
using UnityEngine.Events;

public class EnvironmentImpact : MonoBehaviour
{
    [SerializeField]
    float impactThreshold = 0.5f;

    [SerializeField]
    UnityEvent onImpactThresholdMet;

    //This script is nothing but a simple audio enabler with collisions. Suitable for anything that has no interactivtity but is a part of the environment e.g. Cones, metal signs, etc... - Callum.S
    void OnCollisionEnter(Collision collision)
    {
        Vector3 velocity = collision.relativeVelocity;
        if (velocity.x > impactThreshold || velocity.y > impactThreshold || velocity.z > impactThreshold)
        {
            onImpactThresholdMet.Invoke();
        }
    }
}
