﻿/* Taken from https://answers.unity.com/questions/1179602/implementing-server-side-code-with-unet.html and modified.
 This script handles the round timers for the game. First, there is a 5 second pre-round countdown where players cannot move.
 Afterwards, the round actually begins, and players are free to move.*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameTimer : NetworkBehaviour
{
    private const float roundTime = 60.0f; //How long each round of the game should be
    private const float countDownTime = 6.0f; //The pre-round countdown timer
    [SyncVar]
    public float gameTime; //The length of a game, in seconds.
    [SyncVar]
    public float timer; //How long the game has been running.
    [SyncVar]
    public bool masterTimer = false; //Is this the master timer?
    [SyncVar]
    public bool roundHasStarted; //Whether the game has started or not

    public Text countdownTimerText;

    GameTimer serverTimer;

    void Start()
    {
        roundHasStarted = false;
        timer = countDownTime;
        if (isServer)
        { // For the host to do: use the timer and control the time.
            if (isLocalPlayer)
            {
                serverTimer = this;
                masterTimer = true;
            }
        }
        else if (isLocalPlayer)
        { //For all the boring old clients to do: get the host's timer.
            GameTimer[] timers = FindObjectsOfType<GameTimer>();
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i].masterTimer)
                {
                    serverTimer = timers[i];
                }
            }
        }

        GetComponent<Control>().SetCanMove(false);
    }

    void Update()
    {
        //Debug.Log("Round has started: " + roundHasStarted + "for player " + GetComponent<PlayerResources>().GetId());
        if (!roundHasStarted)
        {
            if (timer < 0)
            {
                Debug.Log("Round is starting.");
                StartCoroutine(RoundStartPause(3f));
                timer = roundTime;
                roundHasStarted = true;
                GetComponent<Control>().SetCanMove(true);
            }
        }

        if (masterTimer)
        { //Only the MASTER timer controls the time
            timer -= Time.deltaTime;
            //Debug.Log(timer);
            if(roundHasStarted && timer<= 0)
            {
                timer = 0;
            }
        }

        if (isLocalPlayer)
        { //EVERYBODY updates their own time accordingly.
            if (serverTimer)
            {
                gameTime = serverTimer.gameTime;
                timer = serverTimer.timer;
                countdownTimerText.text = Mathf.Floor(timer).ToString();
            }
            else
            { //Maybe we don't have it yet?
                GameTimer[] timers = FindObjectsOfType<GameTimer>();
                for (int i = 0; i < timers.Length; i++)
                {
                    if (timers[i].masterTimer)
                    {
                        serverTimer = timers[i];
                    }
                }
            }
            if(roundHasStarted && timer<=0)
            {
                GetComponent<Control>().SetCanMove(false);
            }
        }
    }

    IEnumerator RoundStartPause(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}