using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple timer for tracking elapsed time and calling functions when a timer expires.
/// </summary>
public class Timer : MonoBehaviour
{
    /* Not using InvokeRepeating since:
     * I don't know if its better to call it than calculating the time.
     * You can set this component to inactive to cease the timer's calculations.
     * Don't feel like figuring out a smart way to call CancelInvoke thats not annoying.
    */

    [Range(0f, 100f), Tooltip("How long the timer will continue.")]
    public float duration = 1f;

    [Tooltip("Make the timer automatically restart once it passes its duration.")]
    public bool repeat = false;

    [Tooltip("Make the timer immediately begin on Start")]
    public bool autoStart = true;

    [Tooltip("Events to run when the timer expires.")]
    public UnityEvent timeout;


    [HideInInspector]
    public bool paused;
    float currentTime;

    public float ElapsedTime => currentTime;

    private void Awake()
    {
        Initialise();
    }

    private void Start()
    {
        if (autoStart) paused = false;
    }

    // Increments current time each frame
    private void Update()
    {
        if (paused) return;
        
        currentTime += Time.deltaTime;

        // Is this timer expired
        if (currentTime > duration)
        {
            // Restart timer if it should repeat
            if (repeat)
            {
                Restart();

            }
            else
            {
                paused = true;
            }
            timeout.Invoke();
        }
        
    }

    // Initialiser
    void Initialise()
    {
        currentTime = 0f;
        paused = true;
    }

    /// <summary>
    /// Resets the timer's internally tracked time.
    /// </summary>
    public void Restart()
    {
        currentTime = 0f;
    }

    /// <summary>
    /// Fast forwards the interally tracked time to a new time. Can immediately force a timeout.
    /// </summary>
    /// <param name="newTime"> New time to fast forward to. </param>
    public void ForwardTo(float newTime = math.INFINITY)
    {
        currentTime = newTime;
    }

    private void OnDestroy()
    {
        timeout.RemoveAllListeners();
    }
}
