using UnityEngine;

public class KillVolume : MonoBehaviour
{
    [SerializeField]
    Transform respawn_pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "Float")
        {
            Debug.LogError($"Kill volume \"{name}\" killed {other.gameObject.name}");
            var player_character = other.gameObject.transform;
            player_character.GetComponent<CharacterController>().enabled = false;
            player_character.transform.position = respawn_pos.position;
            player_character.GetComponent <CharacterController>().enabled = true;   
        }
    }
}
