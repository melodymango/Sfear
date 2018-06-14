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
}
