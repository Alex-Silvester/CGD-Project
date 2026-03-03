using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followingObject;
    [SerializeField] Transform lookingAtObject;

    [Range(0f, 1f)]
    [SerializeField] float lerpValue = 0.01f;

    [SerializeField] Camera cameraObject;

    [SerializeField] float fovLerpValue = 0.1f;

    public float startingFov 
    {
        get;
        private set;
    }

    public float fov
    {
        get { return cameraObject.fieldOfView; }
        set 
        {
            cameraObject.fieldOfView = value; 
        }  
    }
    public void resetFOV()
    {
        fov = Mathf.Lerp(fov, startingFov, fovLerpValue);
    }

    private void Awake()
    {
        startingFov = fov;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            followingObject.position,
            lerpValue * (1 + Vector3.Distance(transform.position, followingObject.position)));

        float distDifference =
            Vector3.Distance(transform.position, lookingAtObject.position) -
            Vector3.Distance(followingObject.position, lookingAtObject.position);

        Vector3 direction = (lookingAtObject.position - transform.position).normalized;
        direction.y = 0;

        transform.localPosition += direction * distDifference;
        transform.LookAt(lookingAtObject.position);
    }

    public void setFollowing(Transform pos)
    {
        followingObject = pos;
    }
}
