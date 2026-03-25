using com.cyborgAssets.inspectorButtonPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
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

    private bool activated = false;

    [SerializeField] int playersUnderneath = 0;

    [Header("Audio Variables")]
    [SerializeField] AudioSource runningSound;
    [SerializeField] float audioSpeedRatio;
    [SerializeField] float runningMaxPitch;
    private AudioEnabler audio_enabler;

    float timeWaited = 0f;

    [SerializeField] Vector3 startPosition;
    Vector3 endPosition;

    float currentMovementTime = 0f;
    bool movingBack = false;

    Vector3 activationPostion = Vector3.zero;
    float direction = 0;

    void Start()
    {
        activated = false;

        endPosition = transform.localPosition + new Vector3(0,distance,0);

        direction = Mathf.Sign(distance);

        activationPostion = startPosition;
        audio_enabler = GetComponent<AudioEnabler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playersUnderneath > 0 && !activated)
        {
            activate();
        }

        //if activated and not near the end position
        if (activated && Vector3.Distance(transform.localPosition, endPosition) > threshold)
        {
            float dist = Vector3.Distance(activationPostion, endPosition) * direction;

            currentMovementTime += speed * Time.deltaTime;
            transform.localPosition = activationPostion + new Vector3(0,easingCurve.Evaluate(currentMovementTime)*dist,0);
        }
        //if activated and near the end position
        else if (activated && Vector3.Distance(transform.localPosition, endPosition) <= threshold)
        {
            currentMovementTime = 0f;
            audio_enabler.Disable("Activated");
            timeWaited += Time.deltaTime;
            if (timeWaited >= waitTime)
            {
                activated = false;
                audio_enabler.Disable("Activated");
                timeWaited = 0f;

                activationPostion = transform.localPosition;
            }
        }
        //if not activated and not near the end position
        else if (Vector3.Distance(transform.localPosition, startPosition) > threshold)
        {
            float dist = Vector3.Distance(activationPostion, startPosition) * direction;

            currentMovementTime += speed * Time.deltaTime;
            transform.localPosition = activationPostion - new Vector3(0, easingCurve.Evaluate(currentMovementTime) * dist, 0);

            movingBack = true;
            audio_enabler.Enable("Activated");
        }
        else
        {
            movingBack = false;
            audio_enabler.Disable("Activated");
            currentMovementTime = 0f;
        }

        //Audio changes pitch depending on the speed of the forklift.
        audioSpeedRatio = speed;
        runningSound.pitch = Mathf.Lerp(speed * 5, runningMaxPitch, Time.deltaTime * (audioSpeedRatio * 3));
    }

    [ProButton]
    public void activate()
    {
        activated = true;

        currentMovementTime = 0f;

        activationPostion = transform.localPosition;

        audio_enabler.Enable("Activated");
    }

#if UNITY_EDITOR
    [ProButton]
    public void test()
    {
        Vector3 end = transform.localPosition + new Vector3(0, distance, 0);

        activated = !activated;
        transform.localPosition = activated ? end :startPosition;
    }
#endif

    private void OnTriggerEnter()
    {
        Debug.Log("Enter");
        if(moveBackOnCollide && movingBack)
        {
            Debug.Log("Added");
            playersUnderneath++;
        }
    }

    private void OnTriggerExit()
    {
        if(moveBackOnCollide && playersUnderneath > 0)
            playersUnderneath--;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.localPosition + startPosition + new Vector3(0, distance, 0), new Vector3(1f,1f,1f));
    }
}
