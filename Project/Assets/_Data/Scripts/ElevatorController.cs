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

    [SerializeField] float threshold = 0.01f;

    [SerializeField] AnimationCurve easingCurve;
    [SerializeField] bool moveBackOnCollide = false;

    bool activated = false;

    [SerializeField] int playersUnderneath = 0;

    float timeWaited = 0f;

    Vector3 startPosition;
    Vector3 endPosition;

    float currentMovementTime = 0f;
    bool movingBack = false;

    Vector3 activationPostion = Vector3.zero;
    float direction = 0;

    void Start()
    {
        startPosition = transform.position;
        endPosition = transform.position + new Vector3(0,distance,0);

        direction = Mathf.Sign(distance);

        activationPostion = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(playersUnderneath > 0 && !activated)
        {
            activate();
        }

        //if activated and not near the end position
        if (activated && Vector3.Distance(transform.position, endPosition) > threshold)
        {
            float dist = Vector3.Distance(activationPostion, endPosition) * direction;

            currentMovementTime += speed * Time.deltaTime;
            transform.position = activationPostion + new Vector3(0,easingCurve.Evaluate(currentMovementTime)*dist,0);
        }
        //if activated and near the end position
        else if (activated && Vector3.Distance(transform.position, endPosition) <= threshold)
        {
            currentMovementTime = 0f;

            timeWaited += Time.deltaTime;
            if (timeWaited >= waitTime)
            {
                activated = false;
                timeWaited = 0f;

                activationPostion = transform.position;
            }
        }
        //if not activated and not near the end position
        else if (Vector3.Distance(transform.position, startPosition) > threshold)
        {
            float dist = Vector3.Distance(activationPostion, startPosition) * direction;

            currentMovementTime += speed * Time.deltaTime;
            transform.position = activationPostion - new Vector3(0, easingCurve.Evaluate(currentMovementTime) * dist, 0);

            movingBack = true;
        }
        else
        {
            movingBack = false;
            currentMovementTime = 0f;
        }
    }

    public void activate()
    {
        activated = true;

        currentMovementTime = 0f;

        activationPostion = transform.position;
    }

    private void OnTriggerEnter()
    {
        if(moveBackOnCollide && movingBack)
        {
            playersUnderneath++;
        }
    }

    private void OnTriggerExit()
    {
        if(moveBackOnCollide)
            playersUnderneath--;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position + new Vector3(0, distance, 0), new Vector3(0.5f,0.5f,0.5f));
    }
}
