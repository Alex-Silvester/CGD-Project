using System.Collections.Generic;
using UnityEngine;

public class AddObjectsAsChildren : MonoBehaviour
{
    [SerializeField] List<string> tagMask = new() { "Don't have elevator parent" };

    private void OnTriggerEnter(Collider other)
    {
        if (tagMask.Contains(other.gameObject.tag)) return;

        GameObject obj = other.gameObject;

        if (obj.transform.parent != null) return;

        obj.transform.parent = transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (tagMask.Contains(other.gameObject.tag)) return;

        GameObject obj = other.gameObject;

        if (obj.transform.parent != transform) return;

        obj.transform.parent = null;
    }
}
