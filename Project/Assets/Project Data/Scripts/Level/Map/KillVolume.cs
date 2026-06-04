using UnityEngine;

public class KillVolume : MonoBehaviour
{
    [SerializeField]
    Transform respawn_pos;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            var player_character = other.gameObject.transform;
            player_character.transform.position = respawn_pos.position;
        }
    }
}
