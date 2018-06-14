using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class EndDisplay : NetworkBehaviour
{

    public GameObject[] players;
    public Button exitToLobby;
    public Text resultsText;
    public Text victoryText;
    public float time; //Grabs time
    public bool isGamePlaying; //Boolean for if playtime is active

    // Use this for initialization
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        time = players[0].GetComponent<GameTimer>().timer;
        isGamePlaying = players[0].GetComponent<GameTimer>().roundHasStarted;
        exitToLobby.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        time = players[0].GetComponent<GameTimer>().timer;
        isGamePlaying = players[0].GetComponent<GameTimer>().roundHasStarted;
    }

    void OnGUI()
    {
        if (!isLocalPlayer)
            return;

        if (time <= 0 && isGamePlaying)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            Array.Sort(players, delegate (GameObject x, GameObject y) { return x.GetComponent<Control>().GetTimeWasIt().CompareTo(y.GetComponent<Control>().GetTimeWasIt()); });

            //In here will be where the canvas will pop up and display the result screen
            exitToLobby.gameObject.SetActive(true);

            resultsText.text = "Final Scores\nPlayer Id / Total Time Spent Cursed\n";

            foreach (GameObject player in players)
            {
                if (player.GetComponent<Control>().GetId() == GetComponent<Control>().GetId())
                    resultsText.text += ("(YOU) Player " + player.GetComponent<Control>().GetId() + " / " + System.Math.Round(player.GetComponent<Control>().GetTimeWasIt(), 2) + " seconds\n");
                else
                    resultsText.text += ("Player " + player.GetComponent<Control>().GetId() + " / " + System.Math.Round(player.GetComponent<Control>().GetTimeWasIt(), 2) + " seconds\n");
            }
            
            if (GetComponent<Control>().GetId() == players[0].GetComponent<Control>().GetId())
                victoryText.text = "You Win!";
            else
                victoryText.text = "You Lose...";
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
