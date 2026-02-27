using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.Interpolators;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float distance = 1f;

    [SerializeField] float waitTime = 10f;

    [SerializeField] float threshold = 0.1f;

    [SerializeField] AnimationCurve easingCurve;

    bool activated = false;

    float timeWaited = 0f;

    Vector3 startPosition;
    Vector3 endPosition;

    float currentMovementTime = 0f;

    void Start()
    {
        startPosition = transform.position;
        endPosition = transform.position + new Vector3(0,distance,0);
    }

    // Update is called once per frame
    void Update()
    {
        //if activated and not near the end position
        if (activated && Vector3.Distance(transform.position, endPosition) > threshold)
        {
            currentMovementTime += speed * Time.deltaTime;
            transform.position = startPosition + new Vector3(0,easingCurve.Evaluate(currentMovementTime)*distance,0);
        }
        //if activated and near the end position
        else if (activated)
        {
            currentMovementTime = 0f;

            timeWaited += Time.deltaTime;
            if (timeWaited >= waitTime)
            {
                activated = false;
                timeWaited = 0f;
            }
        }
        //if not activated and not near the end position
        else if (Vector3.Distance(transform.position, startPosition) > threshold)
        {
            currentMovementTime += speed * Time.deltaTime;
            transform.position = endPosition - new Vector3(0, easingCurve.Evaluate(currentMovementTime)*distance, 0);
        }
        else
        {
            currentMovementTime = 0f;
        }
    }

    public void activate()
    {
        activated = true;
    }
}
