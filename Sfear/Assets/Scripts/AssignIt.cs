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
    private Material defaultMaterial;

    //Public variables
    public Material taggedItMaterial;
    public float cooldown;

    // Use this for initialization
    void Start()
    {
        defaultMaterial = new Material(Shader.Find("Sprites/Default"));
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

        foreach (GameObject p in players)
        {
            if (p == playerIt)
			{
                p.GetComponent<Control>().RpcSetCanMove(false);
				p.GetComponent<Control>().RpcSetCooldown(cooldown);
			}
            else
            {
                p.GetComponent<Control>().RpcSetCanMove(true);
                p.GetComponent<Control>().RpcInvulnerable();
                p.GetComponent<Control>().RpcCanBeTagged(cooldown);
				p.GetComponent<Control>().RpcSetCooldown(cooldown);
            }
        }

        playerIt.GetComponent<Control>().RpcChangeToIt();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.gameObject != playerIt.gameObject && other.GetComponent<Control>().CanBeTagged())
        {
            //playerIt.GetComponent<MeshRenderer>().material = defaultMaterial;
            playerIt.GetComponent<Control>().RpcInvulnerable();
            playerIt.GetComponent<Control>().RpcCanBeTagged(cooldown);
            playerIt = null;
            itObject.transform.parent = null;
            itObject.transform.SetParent(other.gameObject.transform, false);
            playerIt = other.gameObject;
            //playerIt.GetComponent<MeshRenderer>().material = taggedItMaterial;
            playerIt.GetComponent<Control>().RpcChangeToIt();
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
