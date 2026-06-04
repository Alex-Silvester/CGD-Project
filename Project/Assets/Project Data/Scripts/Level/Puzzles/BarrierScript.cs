using System.Collections;
using System.Linq;
using UnityEngine;

public class BarrierScript : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float openTime;
    float timeToClose;
    bool active = false;

    [SerializeField] BoxCollider _collider;

    private void Update()
    {
        if (!active) return;

        if(Time.time >= timeToClose && active)
        {
            StartCoroutine(Deactivate());
        }
    }

    public void Activate()
    {
        timeToClose = Time.time + openTime;
        active = true;

        _collider.enabled = false;

        animator.SetBool("active", true);
    }

    private IEnumerator Deactivate()
    {
        active = false;
        animator.SetBool("active", false);
        yield return new WaitForSeconds(1f);
        _collider.enabled = true;
    }
}
