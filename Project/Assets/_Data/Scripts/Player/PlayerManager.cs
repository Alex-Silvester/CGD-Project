using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Space(20)]
    [Header("Player Spawn Points")]
    [SerializeField] Transform player_1_transform;
    [SerializeField] Transform player_2_transform;
    [SerializeField] Transform player_3_transform;
    [SerializeField] Transform player_4_transform;

    [Space(20)]
    [Header("Forklift Spawning")]
    [SerializeField] GameObject forklift_prefab;
    [SerializeField] Vector2 spawn_offest;
    [SerializeField] UnityEvent<int, Transform> playerJoined;

    List<Transform> player_positions = new List<Transform>();
    int player_count = 0;
    [Space(20)]
    [Header("Player Debug Mode")]
#if UNITY_EDITOR
    [SerializeField] bool debug_mode_on = false;
#else
    bool debug_mode_on = false;
#endif
    [SerializeField] GameObject player_prefab;
	[SerializeField] [Range(1, 4)] int debugPlayerCount = 4;

    private int players = 0;
    [SerializeField] private Camera blankCamera;

    List<GameObject> inputs = new();
    [SerializeField] InputActionReference player_join_action;

    // [SerializeField] MinimapPanel minimap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_positions.Add(player_1_transform);
        player_positions.Add(player_2_transform);
        player_positions.Add(player_3_transform);
        player_positions.Add(player_4_transform);

        PlayerInputManager input_manager = GetComponent<PlayerInputManager>();

		// Debug (for testing)
        if (debug_mode_on)
        {
            input_manager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenJoinActionIsTriggered;
            input_manager.playerPrefab = player_prefab;

            for (int i = 0; i < debugPlayerCount; i++)
            {
                PlayerInput player = PlayerInput.Instantiate(player_prefab, i, splitScreenIndex: i);

                player.SwitchCurrentControlScheme(Gamepad.all[Mathf.Min(player_count, Gamepad.all.Count-1)]);
                player.gameObject.GetComponent<DrivingController>().setPlayerGamepad(Gamepad.all[Mathf.Min(player_count, Gamepad.all.Count - 1)]);
            }

            // minimap.RepositionPanel(input_manager.maxPlayerCount);
        }
		// Standard (build)
		else
		{
			print(LobbyMenuManager.currentPlayers.Count);
			
			// LobbyMenuManager.currentPlayers = Gamepads in order they pressed join button on lobby screen
			for (int i = 0; i < LobbyMenuManager.currentPlayers.Count; i++)
			{
                PlayerInput player = PlayerInput.Instantiate(player_prefab, i, splitScreenIndex: i);

                player.SwitchCurrentControlScheme(LobbyMenuManager.currentPlayers[i]);
                player.gameObject.GetComponent<DrivingController>().setPlayerGamepad(LobbyMenuManager.currentPlayers[i]);
			}
		}

        blankCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
    }

    public void OnPlayerJoined(PlayerInput player)
    {		
        blankCamera.enabled = false;

        inputs.Add(player.gameObject);
        player_count++;

        if (debug_mode_on)
        {
            player.gameObject.transform.position = player_positions[player.playerIndex].position;
            player.gameObject.transform.rotation = player_positions[player.playerIndex].rotation;

            playerJoined.Invoke(player_count, player.gameObject.transform);

            return;
        }

        players++;
        
        if (players == 3)
        {
            blankCamera.enabled = true;
        }

        InputDevice playerDevice = player.devices[0]; // the only device used for player is controller at index 0
		
		if (debug_mode_on)
		{
			Gamepad playerGamepad = (Gamepad)InputSystem.GetDeviceById(playerDevice.deviceId); // cast the device as a gamepad using the associated id.

			player.SwitchCurrentControlScheme(playerGamepad);

			player.gameObject.GetComponent<DrivingController>().setPlayerGamepad(playerGamepad);
		}

        player.gameObject.transform.position = player_positions[player.playerIndex].position;
        player.gameObject.transform.rotation = player_positions[player.playerIndex].rotation;
        
        playerJoined.Invoke(player_count, player.gameObject.transform);
    }
}