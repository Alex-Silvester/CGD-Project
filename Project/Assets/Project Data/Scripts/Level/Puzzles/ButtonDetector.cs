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

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.tag == "Player")
    //    {
    //        pressEvent?.Invoke();
    //        button.transform.position = pressedTransform.position;
    //        GetComponent<AudioEnabler>().Enable("Button");
    //    }
    //}
    //
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        releaseEvent?.Invoke();
    //        button.transform.position = releasedTransform.position;
    //    }
    //}

    private void OnTriggerEnter(Collider collider)
    { 
        if (collider.gameObject.tag == "Player")
        {
            pressEvent?.Invoke();
            button.transform.position = pressedTransform.position;
            GetComponent<AudioEnabler>().Enable("Button");
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            releaseEvent?.Invoke();
            button.transform.position = releasedTransform.position;
        }
    }

    private void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            pressEvent?.Invoke();
            button.transform.position = pressedTransform.position;
            GetComponent<AudioEnabler>().Enable("Button");
        }
    }

    private void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            releaseEvent?.Invoke();
            button.transform.position = releasedTransform.position;
        }
    }
}
