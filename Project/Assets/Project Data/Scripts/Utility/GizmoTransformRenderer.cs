using System.Collections.Generic;
using UnityEngine;

public class GizmoTransformRenderer : MonoBehaviour
{
    [SerializeField] Vector3 scale = new Vector3(1,1,1);
    [SerializeField] Vector3 offset = new Vector3(0, 0, 0);
    [SerializeField] Mesh gizmo_mesh;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawMesh(
            gizmo_mesh,
            transform.position + offset,
            Quaternion.identity,
            scale);
        
    }
}
