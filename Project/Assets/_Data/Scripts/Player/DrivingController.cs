using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DrivingController : MonoBehaviour
{
    struct Movement
    {
        public float turningValue;
        public float movingValue;
    }

    [Header("Main Components")]
    [SerializeField] Rigidbody rigidBody;

    [Header("Movement variables")]
    [SerializeField] float acceleration = 20f;
    [SerializeField] float breakMultiplier = 3.0f;
    [SerializeField] float speed = 0f;
    [SerializeField] float maxSpeed = 35f;
    [SerializeField] float rotateSpeed = 5.0f;
    [SerializeField] Movement movement;
    private float previousMovementValue = 0;
    private bool bounced = false;
    [SerializeField] bool is_moving => (movement.movingValue != 0);

    [Header("Ground Checking Variables")]
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] bool isGrounded;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float wheelRadius = 0.5f;

    [Header("Drifting Variables")]
    [SerializeField] GameObject body;
    [SerializeField] float maxRotation = 30;
    [SerializeField] Animation driftAnimation;
    [SerializeField] bool manualDriftAnim = true;
    [SerializeField] float manualAnimationSpeed = 1f;
    [SerializeField] bool drifting = false;
    [SerializeField] float driftMultiplier = 2f;
    [SerializeField] GameObject trailPrefab;
    [SerializeField] GameObject leftTrailStart;
    [SerializeField] GameObject rightTrailStart;
    [SerializeField] DriftingEffectsController driftingEffects;
    GameObject currentTrailLeft;
    GameObject currentTrailRight;
    [SerializeField]GameObject driftTrailsContainer;

    [Header("Boost Variables")]
    [SerializeField] float boostMultiplier = 2f;
    [SerializeField] float boostTimer = 0f;
    [SerializeField] float boostDuration = 2f;
    [SerializeField] bool boostReady = false;
    [SerializeField] float maxBoostSpeed = 20f;
    [SerializeField] int boostTier = 0;
    [SerializeField] bool TiersEnabled;
    [SerializeField] float Tier1Multiplier;
    [SerializeField] float Tier2Multiplier;
    [SerializeField] float Tier3Multiplier;
    [SerializeField] GameObject boostParticlesBL;
    [SerializeField] GameObject boostParticlesBR;
    [SerializeField] float boostTierTimeIncrement = 0.5f;

    float sign = 1f;

    [Header("Lift Variables")]
    [SerializeField] private Transform lift;
    [SerializeField] private float liftSpeed = 1.0f;
    [SerializeField] private float minLiftPosition = 2.4f;
    [SerializeField] private float maxLiftPosition = 9.5f;

    [Header("UI")]
    [SerializeField] private HudManager hudManager;

    [Header("Other References")]
    [SerializeField] private Transform steeringWheel;
    [SerializeField] private SkinnedMeshRenderer playerMesh; // This data type so we can change the skin to match player getting in after alpha

    [Space(10)]
    [SerializeField] float bouncingForceMultiplier = 5f;
    [SerializeField] ForceMode bouncingForceMode = ForceMode.Acceleration;
    [Range(1,2)]
    [SerializeField] float bounceDecay = 2f;
    Vector3 addedForce = Vector3.zero;
    [SerializeField, Min(0f)] float collisionVelocityForCrateDamage = 10f;
    [SerializeField] List<string> ignoreBounceMask;

    [Space(10)]
    [SerializeField] private Transform lookAtTransform;
    [SerializeField] private Transform cameraForwardPos;
    [SerializeField] private Transform cameraReversePos;
    Vector3 rootForward, rootReverse;
    Vector3 lookAtPosition;
    Vector3 cameraReverseOrigin;
    Vector3 cameraForwardOrigin;
    float maxCameraReverseDist;
    float maxCameraForwardDist;
	
	[Header("Camera Shake")]
	[SerializeField] ForkliftCameraShake cameraShake;
	[SerializeField] float shakeDuration = 0.2f;
	[SerializeField] float shakeMagnitude = 0.05f;

    [Header("Camera Boost")]
    [SerializeField] float fovChangeMultiplier = 1.2f;

    [SerializeField] GameObject playerCamera = null;

    private Rigidbody rb;

    private AudioEnabler audio_enabler;

    private Gamepad playerGamepad;

    public Transform CameraForwardTransform => cameraForwardPos;
    public Transform CameraReverseTransform => cameraReversePos;

    FloatPickup floatPickup;
    bool lifting = false;
    bool selfIsLifted = false;


    public void setPlayerGamepad(Gamepad gamepad)
    {
        playerGamepad = gamepad;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audio_enabler = GetComponent<AudioEnabler>();
        floatPickup = GetComponent<FloatPickup>();

        // Camera-transform variables initialisation 
        UpdateCameraTransformPositions();
        rootForward = cameraForwardPos.localPosition;
        rootReverse = cameraReversePos.localPosition;
        maxCameraReverseDist = Vector3.Magnitude(lookAtPosition - cameraReverseOrigin);
        maxCameraForwardDist = Vector3.Magnitude(lookAtPosition - cameraForwardOrigin);

        playerCamera.transform.parent.transform.parent = null; // Get camera shake root
        maxSpeed = 9.0f; //why is maxSpeed being set here?

        driftTrailsContainer.transform.parent = null;
    }

    private void FixedUpdate()
    {
        groundCheck();  
        updateMove();
        updateRotate();

        handleLift();
        repositionCameraTransforms();

        transform.SetPositionAndRotation(transform.position, new Quaternion(0, transform.rotation.y, 0, transform.rotation.w));

        if (TiersEnabled)
        {
            maxBoostSpeed = 30f;
            TieredDriftBoost();

        }
        else
        {
            maxBoostSpeed = 20f;
            DriftBoost();
        }
    }

#region Updating functions
    public void groundCheck()
    {
        RaycastHit hit; 
        float rayLength = groundDistance + wheelRadius;
        isGrounded = Physics.Raycast(groundCheckTransform.position, -groundCheckTransform.up, out hit, rayLength, groundMask);
        Debug.DrawRay(groundCheckTransform.position, -groundCheckTransform.up * rayLength, isGrounded ? Color.green : Color.red);
    }

    private void updateMove()
    {
        if (Mathf.Abs(speed) <= maxSpeed) playerCamera.GetComponent<CameraController>().resetFOV();

        if (!isGrounded || selfIsLifted) return;

        //if triggers held
        if (is_moving)
        {
            //if current speed is maxxed out and the player is attempting to move in that direction
            if (Mathf.Abs(speed) >= maxSpeed && sign == Mathf.Sign(speed))
            {
                speed -= acceleration * Time.deltaTime * sign * ((Mathf.Sign(speed) != sign) ? breakMultiplier : 1);
            }
            else
            {
                speed += acceleration * Time.deltaTime * sign * ((Mathf.Sign(speed) != sign) ? breakMultiplier : 1);
            }
        }
        //if triggers not held
        else
        {
           //if speed is around 0 then stop
           if (Mathf.Abs(speed) <= acceleration * Time.deltaTime * breakMultiplier)
           {
               speed = 0;    
               movement.movingValue = 0;
           }
           //otherwise decelerate
           else
           {
               speed -= acceleration * Time.deltaTime * Mathf.Sign(speed) * breakMultiplier;
           }
        }

        if (addedForce.magnitude < 0.1)
        {
            rigidBody.linearVelocity = (transform.forward * speed) + new Vector3(0, rigidBody.linearVelocity.y, 0);
        }

        if (addedForce.magnitude > 0.1) addedForce *= 1f / bounceDecay;
        else if(bounced)
        {
            bounced = false;
            addedForce = new Vector3();
            movement.movingValue = previousMovementValue;
        }

        //audio handling
        if (sign == -1 && is_moving)
        {
            audio_enabler.Enable("reverse");
        }
        else
        {
            audio_enabler.Disable("reverse");
        }

        if (sign == 1 && is_moving)
        {
            audio_enabler.Enable("driving");
        }
    }
    
    private void updateRotate()
    {
        //don't do rotations if the forklift isn't moving
        if ((speed == 0 && !bounced) || selfIsLifted) return;

        //do the actual forklift rotation so it turns
        transform.Rotate(0, sign * movement.turningValue * rotateSpeed * (drifting ? driftMultiplier : 1.0f) * Time.deltaTime, 0);

        //transform the angle of the forklift from what unity uses to a value that can be used with the maximum rotation value
        float bodyAngle = Mathf.Ceil(body.transform.localEulerAngles.y - 360f * Mathf.Floor(body.transform.localEulerAngles.y / 180f))%360;

        if(movement.turningValue == 0)
        {
            if(Mathf.Abs(bodyAngle) >= 0.1f)
            {
                body.transform.RotateAround(
                 body.transform.position + body.transform.forward * body.transform.localScale.z / 2f,
                 Vector3.up,
                 sign * -Mathf.Sign(bodyAngle) * manualAnimationSpeed);
            }
            else
            {
                body.transform.localRotation = new();
                body.transform.localPosition = new();
            }
        }

        //if the forklift isn't drifting, make sure it is looking forward
        if(!drifting || movement.movingValue == -1 || movement.turningValue == 0)
        {
            body.transform.localRotation = new();
            body.transform.localPosition = new();
            return;
        }


        //-----If player is drifting-----//

        //If the body is fully rotated, then return
        if (Mathf.Abs(bodyAngle) >= maxRotation && Mathf.Sign(bodyAngle) == MathF.Sign(movement.turningValue)) 
        {
            return;
        }

        //If there is no animation added for the drift, then do a basic, manual animation
        if(manualDriftAnim)
        {
            body.transform.RotateAround(
                body.transform.position + body.transform.forward * body.transform.localScale.z / 2f,
                Vector3.up,
                sign * movement.turningValue * manualAnimationSpeed * Time.deltaTime);
        }
        else
        {
            if(driftAnimation != null)
            {
                driftAnimation.Play();
            }

            //matbe not needed, need animation first if we are using one
            body.transform.RotateAround(
                body.transform.position + body.transform.forward * body.transform.localScale.z / 2f,
                Vector3.up,
                sign * movement.turningValue * maxRotation);
        }
    }

    private void handleLift()
    {
        float y = lift.localPosition.y;

        if (lifting)
        {
            y += liftSpeed * Time.deltaTime;
            y = Mathf.Clamp(y, minLiftPosition, maxLiftPosition);

            lift.localPosition = new Vector3(lift.localPosition.x, y, lift.localPosition.z);
        }
        else
        {
            y -= liftSpeed * Time.deltaTime;
            y = Mathf.Clamp(y, minLiftPosition, maxLiftPosition);

            lift.localPosition = new Vector3(lift.localPosition.x, y, lift.localPosition.z);
        }
    }

    // Repositions the transforms of cameras based on if they would collide with eachother
    void repositionCameraTransforms()
    {
        UpdateCameraTransformPositions();
        RaycastHit hit;
        Vector3 direction;

        // Get layer mask we need
        LayerMask mask = ~LayerMask.GetMask("Ignore Raycast", "UI", "Crates");

        // Forward cam transform
        direction = cameraForwardOrigin - lookAtPosition;
        if (Physics.Raycast(lookAtPosition, direction, out hit, maxCameraForwardDist, mask))
        {
            cameraForwardPos.position = hit.point;
        }
        else
        {
            cameraForwardPos.localPosition = rootForward;
        }

        // Reverse cam transform
        direction = cameraReverseOrigin - lookAtPosition;
        if (Physics.Raycast(lookAtPosition, direction, out hit, maxCameraReverseDist, mask))
        {
            cameraReversePos.position = hit.point;
        }
        else
        {
            cameraReversePos.localPosition = rootReverse;
        }
    }

    // Updates positions based on the camera transforms
    // Mainly doing this to avoid duplicating this code
    private void UpdateCameraTransformPositions()
    {
        lookAtPosition = lookAtTransform.position;
        cameraReverseOrigin = cameraReversePos.position;
        cameraForwardOrigin = cameraForwardPos.position;
    }
    public void reset()
    {
        movement.movingValue = 0;
        movement.turningValue = 0;
        drifting = false;
    }

#endregion

#region Input Functions
    public void OnMove(InputValue value)
    {
        if (!isGrounded) return;

        movement.movingValue = value.Get<Vector2>().y;

        if (movement.movingValue != 0)
        {
            sign = Mathf.Sign(movement.movingValue);
        }

        previousMovementValue = movement.movingValue;
    }

    public void OnTurn(InputValue value)
    {   
        float prevTurnValue = movement.turningValue;
        movement.turningValue = value.Get<Vector2>().x;

        if (movement.turningValue != prevTurnValue) 
        {
            boostTimer = 0; 
        }
    }

    public void OnDrift()
    {
        drifting = !drifting;

        if(drifting)
        {
            currentTrailLeft = Instantiate(trailPrefab);
            currentTrailLeft.transform.parent = leftTrailStart.transform;
            currentTrailLeft.transform.position = leftTrailStart.transform.position;

            currentTrailRight = Instantiate(trailPrefab);
            currentTrailRight.transform.parent = rightTrailStart.transform;
            currentTrailRight.transform.position = rightTrailStart.transform.position;
        }
        else
        {
            currentTrailRight.transform.parent = driftTrailsContainer.transform;
            currentTrailLeft.transform.parent =  driftTrailsContainer.transform;
        }
    }

    public void OnLift()
    {
        lifting = !lifting;
    }

    public void OnLook(InputValue value)
    {
        Vector2 direction = value.Get<Vector2>();
        direction = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));

        if(direction.y == 1)
        {
            playerCamera.GetComponent<CameraController>().setFollowing(cameraReversePos);
        }
        else
        {
            playerCamera.GetComponent<CameraController>().setFollowing(cameraForwardPos);
        }
    }

    public void OnInteract()
    {
        floatPickup.PickUpSelectedForklift();

        floatPickup.PickUpSelected();
    }

    public void DriftBoost()
    {
        if (drifting)
        {
           boostTimer += Time.deltaTime;

           if (boostTimer >= 1f)
           {
               boostReady = true;
           }
        }
        else if (!drifting && boostReady)
        {
            speed = speed * boostMultiplier;
            boostReady = false;

            if(speed >= maxBoostSpeed)
            {
                speed = maxBoostSpeed;
            }
        }
        else if (!drifting && !boostReady)
        {
            boostTimer = 0f;
        }
    }

    public void TieredDriftBoost()
    {
        /*
        ParticleSystem ps = boostParticlesBR.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule ma = ps.main;

        ParticleSystem ps1 = boostParticlesBL.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule ma1 = ps1.main;
        if(!drifting && boostParticlesBL.activeSelf)
        {
            boostParticlesBL.SetActive(false);
            boostParticlesBR.SetActive(false);
        }
        */

        CameraController controller = playerCamera.GetComponent<CameraController>();

        if (drifting)
        {
            boostTimer += Time.deltaTime;
            if (boostTimer <= boostTierTimeIncrement)
            {
                boostTier = 0;
                boostReady = false;
            }
            else if (boostTimer <= 2 * boostTierTimeIncrement)
            {
                boostTier = 1;
                boostReady = true;
                /*
                boostParticlesBL.SetActive(true);
                boostParticlesBR.SetActive(true);

                ma.startColor = Color.yellow;
                ma1.startColor = Color.yellow;
                 */
            }
            else if (boostTimer <= 3 * boostTierTimeIncrement)
            {
                boostTier = 2;
                boostReady = true;

                /*
                boostParticlesBL.SetActive(true);
                boostParticlesBR.SetActive(true);

                ma.startColor = Color.red;
                ma1.startColor = Color.red;
                 */
            }
            else if (boostTimer < 4 * boostTierTimeIncrement)
            {
                boostTier = 3;
                boostReady = true;

                /*
                boostParticlesBL.SetActive(true);
                boostParticlesBR.SetActive(true);

                ma.startColor = Color.blue;
                ma1.startColor = Color.blue;
                */
            }

            // Set drifting effects to current tier and play them
            driftingEffects.SetEffectTier(boostTier);
            driftingEffects.Play();
        }
        else if (!drifting && boostReady)
        {
            controller.fov = controller.fov * fovChangeMultiplier;

            switch (boostTier)
            {
                case 0:
                    boostMultiplier = 0f;
                    break;
                case 1:
                    boostMultiplier = Tier1Multiplier;
                    break;
                case 2:
                    boostMultiplier = Tier2Multiplier;
                    break;
                case 3:
                    boostMultiplier = Tier3Multiplier;
                    break;
            }

            speed = speed * boostMultiplier;
            boostReady = false;

            if (speed >= maxBoostSpeed)
            {
                speed = maxBoostSpeed;
            }
        }
        else if (!drifting && !boostReady)
        {
            boostTimer = 0f;
            boostTier = 0;
            driftingEffects.Stop();
        }

    }

    public void togglePlayerLifted()
    {
        selfIsLifted = !selfIsLifted;
    }

    #endregion

    public void OnDrawGizmos()
    {
        RaycastHit hit;
        float rayLength = groundDistance + wheelRadius;
        if (Physics.Raycast(groundCheckTransform.position, -groundCheckTransform.up, out hit, rayLength, groundMask))
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawRay(groundCheckTransform.position, -groundCheckTransform.up * rayLength);
    }

    private void OnCollisionEnter(Collision collision)
    {
		// Shake camera when colliding with crates
		if (collision.transform.CompareTag("Float"))
		{
			cameraShake.Shake(shakeDuration, shakeMagnitude * speed);
		}
		
        if (ignoreBounceMask.Contains(collision.gameObject.tag)) return;

        bounced = true;

        Vector3 forceDirection = Vector3.zero;

        Vector3 forwardDir = -transform.forward * sign;
        Vector3 normalDir = collision.impulse.normalized;

        forceDirection = normalDir;

        //reflect the direction around the impulse of the collision
        //float k = 2 * (forwardDir.x * normalDir.z + forwardDir.z * normalDir.x);
        //forceDirection = new Vector3(forwardDir.x-k*normalDir.z, 0,forwardDir.z-k*normalDir.x).normalized;

        addedForce = forceDirection * bouncingForceMultiplier * rigidBody.mass;

        movement.movingValue = 0;

        rigidBody.AddForce(addedForce);
		
		// Camera shake
		cameraShake.Shake(shakeDuration, shakeMagnitude * speed);

        TryDropOnCollision(collision);
    }

    // Drops the forklift's held object based on a collision
    private void TryDropOnCollision(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= collisionVelocityForCrateDamage)
        {
            floatPickup.TryDropSelectedObject();
        }
    }

}
