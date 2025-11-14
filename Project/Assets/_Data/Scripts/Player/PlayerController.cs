using StarterAssets;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static Unity.Burst.Intrinsics.X86;
using static UnityEditor.PlayerSettings;

// This script is for our game's custom features
// The other FirstPersonController is for first person movement code
public class PlayerController : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField][Range(1, 4)] private int playerNumber = 1;

	Gamepad playerGamepad;

    //[Header("UI")]
    //[SerializeField] private HudManager hudManager;

    // Enter / exiting vehicles
    private List<IDriveable> driveablesInRange = new List<IDriveable>();

	[Space(20)]
	[Header("Player Model")]
	[SerializeField] GameObject model;

	private ForkliftController current_forklift = null;

	[SerializeField] GameObject camera;
	[SerializeField] Transform player_camera_root;
	[SerializeField] float cameraYOffset = 1.622f;

	[SerializeField] InputActionReference lifting_action;
	[SerializeField] InputActionReference dropping_action;

	private float enter_vehicle_start_height;
	private bool lift_enabled = false;




	public void setPlayerGamepad(Gamepad pad)
	{
		playerGamepad = pad;
	}

	public bool driving { get; private set; }

    private void Start()
	{
        driving = false;
    }

    private void OnEnable()
	{
		// Reset
		driveablesInRange = new List<IDriveable>();
	}

    private void OnDisable()
    {

    }

    private void Update()
    {

        if (playerGamepad.buttonEast.wasPressedThisFrame) 
		{
			if (!lift_enabled)
			{
				Lift();
                lift_enabled = true;

            }
			else
			{
				Drop();
				lift_enabled = false;

            }
			print(lift_enabled);
		}

        /*
        if(playerGamepad.rightShoulder.IsPressed())
		{
			Lift();
		}
		else if(playerGamepad.leftShoulder.IsPressed())
		{
			Drop();
		}
		else
		{
			cancelLift();
		}
		*/
    }

    public void OnInteract()
    {
		if(!driving)
		{ 
			EnterVehicle();

			if(current_forklift!=null)
			{ 
				camera.transform.LookAt(current_forklift.getLookAtTransform()); 
			}
        }
		else
		{
			GetComponent<CharacterController>().enabled = false;
			Debug.Log("hi");
			current_forklift.interact();
			model.SetActive(true);
			driving = false;

			Transform exit_transform = current_forklift.getExitTransform();

			transform.position = exit_transform.position;
			transform.eulerAngles = new Vector3(0,exit_transform.rotation.y,0);

			camera.transform.position = player_camera_root.position;
			camera.transform.rotation = player_camera_root.rotation;
			camera.transform.parent = gameObject.transform;
			camera.transform.GetChild(0).transform.localPosition = new Vector3(0f, cameraYOffset, 0f);

            GetComponent<CharacterController>().enabled = true;

            current_forklift = null;
		}
    }

	public void Lift()
	{
		if(driving)
		{
			current_forklift.Lift();
		}
	}

	public void Drop()
	{
        if (driving)
		{
			current_forklift.Drop();
		}
	}

	private void cancelLift()
	{
        if (driving)
		{
			current_forklift.cancelLift();
		}
	}

    private void EnterVehicle()
	{
        if (TryEnterVehicleInRange())
        {
			enter_vehicle_start_height = camera.transform.position.y;
            model.SetActive(false);
			driving = true;
        }
    }

	public void drive(StarterAssetsInputs input)
	{
		current_forklift.move(input);
	}

	public void cameraDrive(float rotation_velocity)
	{
		camera.transform.position = new Vector3(camera.transform.position.x, enter_vehicle_start_height, camera.transform.position.z);
		Debug.Log(camera.transform.position);
		// camera.transform.RotateAround(current_forklift.transform.position, Vector3.up, rotation_velocity);
		camera.transform.LookAt(current_forklift.getLookAtTransform());
    }

    private bool TryEnterVehicleInRange()
	{
		if (driveablesInRange.Count > 0)
		{
			if (driveablesInRange[0] == null)
			{
				Debug.LogError("Closest vehicle is null");
				return false;
			}

			if(!driveablesInRange[0].TryEnterVehicle(this))
			{
				Debug.LogWarning("Failed to enter vehicle");
				return false;
			}

			// Camera setup
			current_forklift = (ForkliftController)driveablesInRange[0];
            camera.transform.parent = current_forklift.transform;
			SetCameraPosition(false);
			//Transform forklift_camera_root = current_forklift.getCameraRoot();
            //camera.transform.SetPositionAndRotation(forklift_camera_root.transform.position, forklift_camera_root.transform.rotation);
            //camera.transform.LookAt(current_forklift.getLookAtTransform());
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public void AddVehicleInRange(IDriveable driveable)
	{
		// Don't add twice
		if (!driveablesInRange.Contains(driveable))
		{
			driveablesInRange.Add(driveable);
			
			//hudManager.SetVehiclePromptStatus(playerNumber, true);
		}
		else
		{
			Debug.LogWarning("Tried to add a driveable that is already in driveablesInRange");
		}
	}
	
	public void RemoveVehicleInRange(IDriveable driveable)
	{
		if (driveablesInRange.Contains(driveable))
		{
			driveablesInRange.Remove(driveable);
			
			if (driveablesInRange.Count <= 0)
			{
				//hudManager.SetVehiclePromptStatus(playerNumber, false);
			}
		}
		else
		{
			Debug.LogWarning("Tried to remove driveable that isn't in driveablesInRange.");
		}
	}

	// Sets the position of the camera to either forward of reverse
	public void SetCameraPosition(bool toReverse)
	{
        Transform atForward = current_forklift.getCameraRoot();
		Transform atReverse = current_forklift.CameraReversePosition;

		/// TODO:
		/// - Needs to lerp or tween between the two positions
		/// - It may be desirable to instead set these positions based on to opposite locations of a circle
		/// - That way the translation will be a rotation around this circle (Transform.RotateAround)
		/// - How to do animation... idk :P
		if (toReverse)
		{
            camera.transform.SetPositionAndRotation(atReverse.transform.position, atReverse.transform.rotation);
        }
		else
		{
            camera.transform.SetPositionAndRotation(atForward.transform.position, atForward.transform.rotation);
		}
 
		camera.transform.GetChild(0).transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        camera.transform.LookAt(current_forklift.getLookAtTransform());
    }
	
	public Gamepad GetPlayerGamepad()
	{
		return playerGamepad;
	}

    /*private bool VehicleCheck()
	{
		// Is there a vehicle in front of the player?
			
		// Source - https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Physics.SphereCast.html
		RaycastHit hit;
		Vector3 p1 = transform.position + controller.center;

		// Cast a sphere wrapping character controller vehicleEnterDistance meters forward
		// to see if it is about to hit anything.
		if (Physics.SphereCast(p1, controller.height / 2, transform.forward, out hit, vehicleEnterDistance))
		{
			IDriveable drivable = hit.transform.GetComponent<IDriveable>();
				
			if (drivable != null)
			{
				Debug.Log("Driveable found: " + hit.transform.name);
				return true;
			}
			else
			{
				Debug.Log(hit.transform.name + " isn't driveable");
				return false;
			}
		}
		else
		{
			Debug.Log("Nothing there");
			return false;
		}
	}*/
}