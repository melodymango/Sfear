using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EndDisplay : NetworkBehaviour {

    public GameObject[] players;
    public Button exitToLobby;
    public float time; //Grabs time
    public bool isGamePlaying; //Boolean for if playtime is active
    public bool gameCreated; //And extra bool because GameTimer.roundHasStarted doesn't go back to false due to spaghetti code

    // Use this for initialization
    void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");

        time = players[0].GetComponent<GameTimer>().timer;
        isGamePlaying = players[0].GetComponent<GameTimer>().roundHasStarted;
        exitToLobby.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        time = players[0].GetComponent<GameTimer>().timer;
        isGamePlaying = players[0].GetComponent<GameTimer>().roundHasStarted;
    }

    void OnGUI()
    {
        if (time <= 0 && isGamePlaying && !gameCreated)
        {
            //In here will be where the canvas will pop up and display the result screen
            exitToLobby.gameObject.SetActive(true);
        }
    }

    public void ExitGame()
    {
        if (isServer)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
}
