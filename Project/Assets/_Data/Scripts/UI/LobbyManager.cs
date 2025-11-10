using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] List<Image> player_indicator;
    [SerializeField] Camera camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.SetActive(false);
            camera.gameObject.SetActive(false);
        }
    }

    public void playerJoined(int playerNum)
    {
        Debug.Log($"player {playerNum + 1} joined");

        player_indicator[playerNum].GetComponent<Image>().color = Color.green;
    }
}
