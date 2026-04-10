using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Interactions;
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
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float wheelRadius = 0.5f;

    [Header("Drifting Variables")]
    [SerializeField] GameObject body;
    [SerializeField] float maxRotation = 30;
    [SerializeField] Animator DriftBody;
    [SerializeField] bool manualDriftAnim = true;
    [SerializeField] float manualAnimationSpeed = 1f;
    [SerializeField] bool drifting = false;
    [SerializeField] float driftSpeed = 100f;
    [SerializeField] GameObject trailPrefab;
    [SerializeField] GameObject leftTrailStart;
    [SerializeField] GameObject rightTrailStart;
    [SerializeField] DriftingEffectsController driftingEffects;
    GameObject currentTrailLeft;
    GameObject currentTrailRight; 
    [SerializeField]GameObject driftTrailsContainer;
    bool manuallyStoppedDrift = false;

    [Header("Boost Variables")]
    [SerializeField] float boostMultiplier = 2f;
    [SerializeField] float boostTimer = 0f;
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
    [SerializeField] GameObject speedLinesImage;
    [SerializeField] Vector3 crateBoostMultipliers = new(2f,2.5f,3f);
    [SerializeField] Slider boostBar;

    float sign = 1f;

    [Header("Lift Variables")]
    [SerializeField] private Transform lift;
    [SerializeField] private float liftSpeed = 1.0f;
    [SerializeField] private float minLiftPosition = 5f;
    [SerializeField] private float maxLiftPosition = 10f;
	public UnityEvent<bool> onLift = new UnityEvent<bool>();

    [Header("UI")]
    [SerializeField] private HudManager hudManager;
    [SerializeField] private GameObject dropAllUI;
    [SerializeField] private Slider dropAllSlider;
    [SerializeField] private GameOverPanel gameOverMenu;
	[SerializeField] private GameObject breakFreePrompt;

    [Header("Other References")]
    [SerializeField] private Transform steeringWheel;
    [SerializeField] private SkinnedMeshRenderer playerMesh;
    // This data type so we can change the skin to match player getting in after alpha

    [Header("Audio Variables")]
    [SerializeField] AudioSource runningSound;
    [SerializeField] float runningMaxPitch;
    [SerializeField] float audioSpeedRatio;

    [Header("Bouce variables")]
    [Header("Bounce variables")]
    [SerializeField] float bouncingForceMultiplier = 5f;
    [Range(1,2)]
    [SerializeField] float bounceDecay = 2f;
    Vector3 addedForce = Vector3.zero;
    [SerializeField, Min(0f)] float collisionVelocityForCrateDamage = 10f;
    [SerializeField] List<string> ignoreBounceMask;

    [Header("Camera Transform")]
    [SerializeField] private Transform lookAtTransform;
    [SerializeField] private Transform cameraForwardPos;
    [SerializeField] private Transform cameraReversePos;
    [SerializeField] private List<string> cameraRayCastMask = new List<string>();
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
    [SerializeField] private GameObject castRay;

    private Rigidbody rb;

    private AudioEnabler audio_enabler;

    private Gamepad playerGamepad;

    private bool holdingInteract = false;
    private float interactHoldTime = 0f;

    private bool lookingUp = false;
    private bool lookingBack = false;

    public Transform CameraForwardTransform => cameraForwardPos;
    public Transform CameraReverseTransform => cameraReversePos;

    bool lifting = false;
    [SerializeField] bool selfIsLifted = false;
    CratePickUp lifterPickup;

    [Header("Wheel Animations")]
    [SerializeField] Animator frontwheel;
    [SerializeField] Animator backwheel;
    [SerializeField] Animator frontwheel2;
    [SerializeField] Animator backwheel2;

    [SerializeField] Animator stunanim;

    public void setPlayerGamepad(Gamepad gamepad)
    {
        playerGamepad = gamepad;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audio_enabler = GetComponent<AudioEnabler>();

        // Camera-transform variables initialisation 
        UpdateCameraTransformPositions();
        rootForward = cameraForwardPos.localPosition;
        rootReverse = cameraReversePos.localPosition;
        maxCameraReverseDist = Vector3.Magnitude(lookAtPosition - cameraReverseOrigin);
        maxCameraForwardDist = Vector3.Magnitude(lookAtPosition - cameraForwardOrigin);

        playerCamera.transform.parent.transform.parent = null; // Get camera shake root
        maxSpeed = 9.0f; //why is maxSpeed being set here?

        driftTrailsContainer.transform.parent = null;

        speedLinesImage.SetActive(false);

        dropAllUI.SetActive(false);
    }

    private void FixedUpdate()
    {
        if(transform.parent == null)
        {
            selfIsLifted = false;
			breakFreePrompt.SetActive(false);
			
            rigidBody.isKinematic = false;
            GetComponent<BoxCollider>().enabled = true;
        }

        rb.angularVelocity = Vector3.zero;

        groundCheck();  
        updateMove();
        updateRotate();

        handleLift();
        repositionCameraTransforms();

        transform.SetPositionAndRotation(transform.position, new Quaternion(0, transform.rotation.y, 0, transform.rotation.w));

        if (holdingInteract)
        {
            interactHoldTime += Time.deltaTime / 0.4f; //default max hold time
            dropAllSlider.value = interactHoldTime;
        }

        if (selfIsLifted) return;
        //the sections after here shoudln't currently be able to run while the forklift is lifted

        frontwheel.SetFloat("Speed", speed);
        backwheel.SetFloat("Speed", speed);
        frontwheel2.SetFloat("Speed", speed);
        backwheel2.SetFloat("Speed", speed);
        frontwheel.SetFloat("wheeldir", movement.turningValue);
        backwheel.SetFloat("wheeldir", movement.turningValue);
        frontwheel2.SetFloat("wheeldir", movement.turningValue);
        backwheel2.SetFloat("wheeldir", movement.turningValue);
        
        if (speed > 10f && bounced == true)
        {
            stunanim.SetBool("Stun", true);   
        }
        else
        {
            stunanim.SetBool("Stun", false);
        }    

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
        
        //Audio changes pitch depending on the speed of the forklift.
        audioSpeedRatio = speed;
        runningSound.pitch = Mathf.Lerp(speed / 2, runningMaxPitch, Time.deltaTime * audioSpeedRatio);
    }

#region Updating functions
    public void groundCheck()
    {
        RaycastHit hit; 
        float rayLength = groundDistance + wheelRadius;
        isGrounded = Physics.Raycast(groundCheckTransform.position, -groundCheckTransform.up, out hit, rayLength);
        Debug.DrawRay(groundCheckTransform.position, -groundCheckTransform.up * rayLength, isGrounded ? Color.green : Color.red);
    }

    private void updateMove()
    {
        if (Mathf.Abs(speed) <= maxSpeed || selfIsLifted)
        {
            playerCamera.GetComponent<CameraController>().resetFOV();
            speedLinesImage.SetActive(false);
        }

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
                speedLinesImage.SetActive(false);
                speed += acceleration * Time.deltaTime * sign * ((Mathf.Sign(speed) != sign) ? breakMultiplier : 1);
            }
        }
        //if triggers not held
        else
        {
           speedLinesImage.SetActive(false);

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
        transform.Rotate(0, movement.turningValue * (drifting ? driftSpeed * sign : rotateSpeed * Mathf.Sign(speed)) * Time.deltaTime, 0);

        //transform the angle of the forklift from what unity uses to a value that can be used with the maximum rotation value
        float bodyAngle = Mathf.Ceil(body.transform.localEulerAngles.y - 360f * Mathf.Floor(body.transform.localEulerAngles.y / 180f))%360;

        if(movement.turningValue == 0)
        {
            if(Mathf.Abs(bodyAngle) >= 0.1f)
            {
                body.transform.RotateAround(
                 body.transform.position + body.transform.forward * body.transform.localScale.z / 2f,
                 Vector3.up,
                 -Mathf.Sign(bodyAngle) * manualAnimationSpeed);
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
            DriftBody?.SetFloat("DriftDirection", 0);
            return;
        }


        //-----If player is drifting-----//

        //If the body is fully rotated, then return
        if (Mathf.Abs(bodyAngle) >= maxRotation && Mathf.Sign(bodyAngle) == MathF.Sign(movement.turningValue)) 
        {
            return;
        }


        //rotate the body of the forklift over time
        body.transform.RotateAround(
            body.transform.position + body.transform.forward * body.transform.localScale.z / 2f,
            Vector3.up,
            movement.turningValue * manualAnimationSpeed * Time.deltaTime);

        //If there is an animation added for the drift
        if (!manualDriftAnim)
        {
            DriftBody?.SetFloat("DriftDirection", movement.turningValue);
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
        LayerMask mask = ~LayerMask.GetMask(cameraRayCastMask.ToArray());

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

    public void OnDrift(InputValue value)
    {
        var pressVal = value.Get<float>();

        // you can't drift while lifted
        if (selfIsLifted) return;

        if(manuallyStoppedDrift)
        {
            manuallyStoppedDrift = false;
        }
        else
        {
            drifting = !drifting & (pressVal == 1);
        }



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
		
		onLift?.Invoke(lifting);
    }

    public void OnLookBack(InputValue value)
    {
        int val = Mathf.CeilToInt(value.Get<float>());
        lookingBack = (val == 1);

        playerCamera.GetComponent<CameraController>().setFollowing(lookingBack ? cameraReversePos : cameraForwardPos);
    }

    public void OnInteract()
    {
        castRay.GetComponent<CratePickUp>().PickUpSelected();
        castRay.GetComponent<CratePickUp>().PickUpSelectedForklift();
    }

    public void OnDrop()
    {
        holdingInteract = true;
        dropAllUI.SetActive(true);

        castRay.GetComponent<CratePickUp>().DropHeld();
    }

    public void OnReleaseDrop()
    {
        holdingInteract = false;
        dropAllUI.SetActive(false);
        interactHoldTime = 0f;
    }

    public void OnDropHold()
    {
        CratePickUp cratePickup = castRay.GetComponent<CratePickUp>();

        for (int i = 0; i <= cratePickup.heldObjectsCount; i++)
        {
            cratePickup.DropHeld();    
        }

        dropAllUI.SetActive(false);
        interactHoldTime = 0f;
        holdingInteract = false;
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
        // This should be cached in a variable not called here
        CameraController controller = playerCamera.GetComponent<CameraController>();

        if (drifting)
        {
            // Set boosting tier 
            boostTimer += Time.deltaTime;
            boostTier = Mathf.Clamp(Mathf.FloorToInt(boostTimer / boostTierTimeIncrement), 0, 3);
            boostReady = boostTier > 0;

            // Set drifting effects to current tier and play them
            driftingEffects.SetEffectTier(boostTier);
            driftingEffects.Emit(true);
            driftingEffects.Play();

            boostBar.gameObject.SetActive(true);
            boostBar.value = Mathf.Min(boostTimer / (3 * boostTierTimeIncrement), 3);

            /* This sucks btw
            if (boostTimer <= boostTierTimeIncrement)
            {
                boostTier = 0;
                boostReady = false;
            }
            else if (boostTimer <= 2 * boostTierTimeIncrement)
            {
                boostTier = 1;
                boostReady = true;
                
                boostParticlesBL.SetActive(true);
                boostParticlesBR.SetActive(true);

                ma.startColor = Color.yellow;
                ma1.startColor = Color.yellow;
                 
            }
            else if (boostTimer <= 3 * boostTierTimeIncrement)
            {
                boostTier = 2;
                boostReady = true;

                
                boostParticlesBL.SetActive(true);
                boostParticlesBR.SetActive(true);

                ma.startColor = Color.red;
                ma1.startColor = Color.red;
                 
            }
            else if (boostTimer < 4 * boostTierTimeIncrement)
            {
                boostTier = 3;
                boostReady = true;

                
                boostParticlesBL.SetActive(true);
                boostParticlesBR.SetActive(true);

                ma.startColor = Color.blue;
                ma1.startColor = Color.blue;
                
            }
             */
        }
        else if (!drifting && boostReady)
        {
            controller.fov = controller.startingFov * fovChangeMultiplier;

            speedLinesImage.SetActive(true);

            switch (boostTier)
            {
                case 0:
                    boostMultiplier = 0f;
                    break;
                case 1:
                    boostMultiplier = Tier1Multiplier;
                    multiplyCrateScore(crateBoostMultipliers.x);
                    break;
                case 2:
                    boostMultiplier = Tier2Multiplier;
                    multiplyCrateScore(crateBoostMultipliers.y);
                    break;
                case 3:
                    boostMultiplier = Tier3Multiplier;
                    multiplyCrateScore(crateBoostMultipliers.z);
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
            driftingEffects.Emit(false);
            driftingEffects.Stop();
            boostTimer = 0f;
            boostTier = 0;

            boostBar.value = 0f;
            boostBar.gameObject.SetActive(false);
        }
    }

    public void togglePlayerLifted(bool lifted, CratePickUp cratePickUp = null)
    {
        selfIsLifted = !selfIsLifted;
		breakFreePrompt.SetActive(selfIsLifted);
		
        lifterPickup = cratePickUp;

        if(selfIsLifted)
        {
            stopAllAnimations();
        }
    }

    public void OnDisconnectFromPickup()
    {
        lifterPickup?.DropHeld();
    }

    #endregion

    public void OnDrawGizmos()
    {
        RaycastHit hit;
        float rayLength = groundDistance + wheelRadius;
        if (Physics.Raycast(groundCheckTransform.position, -groundCheckTransform.up, out hit, rayLength))
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
        if (ignoreBounceMask.Contains(collision.gameObject.tag)) return;

		// Shake camera when colliding with crates
		if (collision.transform.CompareTag("Float"))
		{
			cameraShake.Shake(shakeDuration, shakeMagnitude * speed);
        }
        //if collision is not with a crate then stop drifting
        else if(drifting)
        {
            manuallyStopDrifting();
        }
        
        
        bounced = true;

        Vector3 forceDirection = Vector3.zero;

        Vector3 forwardDir = -transform.forward * sign;
        Vector3 normalDir = collision.impulse.normalized;

        forceDirection = normalDir;

        addedForce =
            forceDirection * bouncingForceMultiplier * rigidBody.mass * 
            -Mathf.Sign(Vector3.Dot(normalDir, (collision.gameObject.transform.position - transform.position).normalized));

        

        movement.movingValue = 0;

        rigidBody.AddForce(addedForce);
		
		// Camera shake
		cameraShake.Shake(shakeDuration, shakeMagnitude * speed);

        TryDropOnCollision(collision);
        
        //Audio impact for when the forklift bounces from a wall, it plays a sound
        if (bounced == true)
        {
            audio_enabler.Enable("impact");
            //print("IMPACT FORKLIFT");
        }
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Float")
            TryDropOnCollision(collision);
    }

    // Drops the forklift's held object based on a collision
    private void TryDropOnCollision(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= collisionVelocityForCrateDamage)
        {
            castRay.GetComponent<CratePickUp>().DropHeld();
        }
    }

    private void multiplyCrateScore(float multiplier)
    {
        castRay.GetComponent<CratePickUp>().multiplyCrateScore(multiplier);
    }

    private void manuallyStopDrifting()
    {
        manuallyStoppedDrift = true;

        drifting = false;
        boostReady = false;
        driftingEffects.Emit(false);
        driftingEffects.Stop();
        boostTimer = 0f;
        boostTier = 0;

        currentTrailRight.transform.parent = driftTrailsContainer.transform;
        currentTrailLeft.transform.parent = driftTrailsContainer.transform;
    }

    private void stopAllAnimations()
    {
        DriftBody.SetFloat("DriftDirection", 0);

        frontwheel.SetFloat("Speed", 0);
        backwheel.SetFloat("Speed", 0);
        frontwheel2.SetFloat("Speed", 0);
        backwheel2.SetFloat("Speed", 0);

        frontwheel.SetFloat("wheeldir", 0);
        backwheel.SetFloat("wheeldir", 0);
        frontwheel2.SetFloat("wheeldir", 0);
        backwheel2.SetFloat("wheeldir", 0);

        stunanim.SetBool("Stun", false);
    }
}
