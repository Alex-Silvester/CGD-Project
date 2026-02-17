using UnityEngine;
using UnityEngine.Events;

public class ButtonDetector : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] UnityEvent pressEvent;
    [SerializeField] UnityEvent releaseEvent;

    [Header("Button data")]
    [SerializeField] GameObject button;
    [SerializeField] Transform pressedTransform;
    [SerializeField] Transform releasedTransform;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            pressEvent?.Invoke();
            button.transform.position = pressedTransform.position;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            releaseEvent?.Invoke();
            button.transform.position = releasedTransform.position;
        }
    }
}
