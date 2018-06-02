/* Script attached to the object that represents "it".
 * The "it" object is passed to the player who is "it".
 * Assigns "it" status to a random player at the beginning of a round.
 * Also is in charge of transferring itself when a player tags another player*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssignIt : NetworkBehaviour {

    private GameObject[] players;
    private GameObject itObject;
    private GameObject playerIt;

    // Use this for initialization
    void Start()
    {
        itObject = GameObject.FindGameObjectWithTag("TaggedIt");
        Invoke("AssignItToPlayer", 1f);
    }

    // Update is called once per frame
    void Update () {
		
	}

    //Assign "It" status to a random player 
    void AssignItToPlayer()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Number of players: " + players.Length);

        int i = Random.Range(0, players.Length);

        itObject.transform.SetParent(players[i].transform, false);
        playerIt = players[i];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other != playerIt.gameObject)
        {
            playerIt = null;
            itObject.transform.parent = null;
            itObject.transform.SetParent(other.gameObject.transform, false);
            playerIt = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other != playerIt.gameObject)
        {
            //Debug.Log("Not colliding with another player.");
        }
    }
}
