using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField]
    Vector3 direction = Vector3.zero;

    [SerializeField]
    float speed = 1f;

    [SerializeField]
    bool moveLocal = false;

    private void Awake()
    {
        direction.Normalize();
        if (moveLocal)
        {
            direction = transform.rotation * direction;
        }
    }

    private void Update() => transform.position += speed * Time.deltaTime * direction;
}
