using UnityEngine;
using UnityEngine.InputSystem;

public class DirectorController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    Vector3 direction = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 deltaX = transform.right * direction.x;
        Vector3 deltaY = Vector3.up * direction.y;
        Vector3 deltaZ = transform.forward * direction.z;


        transform.position += (deltaX + deltaY + deltaZ).normalized * speed * Time.deltaTime;
    }

    public void OnMove(InputValue input)
    {
        direction = input.Get<Vector3>();
    }

    public void OnLook(InputValue input)
    {
        Vector2 rotation = input.Get<Vector2>();

        transform.RotateAround(transform.position, Vector3.up, rotation.x);
        transform.RotateAround(transform.position, transform.right, rotation.y);
    }
}
