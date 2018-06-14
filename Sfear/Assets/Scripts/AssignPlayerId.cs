using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssignPlayerId : NetworkBehaviour {

    public GameObject[] players;

    // Use this for initialization
    void Start () {
        Invoke("AssignPlayerIds", 1f);
    }
	
	// Update is called once per frame
	void Update () {
        /*
        if (players[0].GetComponent<GameTimer>().timer <= 0 && players[0].GetComponent<GameTimer>().roundHasStarted)
        {
            foreach (GameObject player in players)
            {
                player.GetComponent<EndDisplay>().RpcDisplayFinalScore(CalculateFinalScore());
            }
        }*/
		
	}

    void AssignPlayerIds()
    {
        int id = 1;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<Control>().SetId(id);
            id++;
        }

        foreach (GameObject player in players)
        {
            Debug.Log(player.GetComponent<Control>().GetId() + " has joined the game.");
        }

    }

    public string CalculateFinalScore()
    {
        string result = "";
        result += "Final Scores\nPlayer Id / Total Time Spent Cursed\n";
        foreach (GameObject player in players)
        {
            result += ("Player " + player.GetComponent<Control>().GetId() + " / " + System.Math.Round(player.GetComponent<Control>().GetTimeWasIt(), 2) + " seconds\n");
        }

        return result;
    }
}
