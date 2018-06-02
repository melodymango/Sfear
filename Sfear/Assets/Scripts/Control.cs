using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Control : NetworkBehaviour {
	private GameObject ball;			//Reference to the Sfear
	private Vector3 relativePosition;	//Position of this player relative to the center of the Sfear
	private float radius;				//Radius of ball PLUS half height of capsule
	private float hspeed, vspeed;		//Horizontal and vertical speed (relative to camera plane)
	
	//Public variables
	public float acceleration = 0.0005f;	//How fast the player object responds to swiping
	public float maxSpeed = 0.015f;		//Fastest possible move speed
	public float slowDown = 0.99f;		//Multiplied by movement speed every step to make smooth movement
	
	void Start () {
		radius = 0.55f;
		
		//Get a reference to the ball
		ball = GameObject.FindWithTag("Ball"); 
		
		//Find relative position to sphere's center
		relativePosition = transform.position - ball.transform.position;
		
		Camera.main.transform.position = transform.position + relativePosition;
		Camera.main.transform.LookAt(ball.transform, Camera.main.transform.up);
	}
	
	void Update () {
	
		if(!isLocalPlayer){
			return;
		}

		//Used to keep old settings while making movement framerate-independent
		//float dT = Time.deltaTime*60;
        		
		//Slow down gradually
		hspeed *= slowDown/**d*T*/;
		vspeed *= slowDown/**dT*/;
		
		//If user's finger is held down, calculate move speed.
		if(Input.GetAxis("Tap") == 1){
			//add mouse movement to this player's speed
			hspeed -= Input.GetAxis("Mouse X")*acceleration/**dT*/;
			vspeed -= Input.GetAxis("Mouse Y")*acceleration/*dT*/;
			
			//Clamp to max speed
			hspeed = Mathf.Clamp(hspeed, -maxSpeed, maxSpeed);
			vspeed = Mathf.Clamp(vspeed, -maxSpeed, maxSpeed);
		}
		
		//Move
		Move(hspeed,vspeed);
		
		//Find relative position to sphere's center
		relativePosition = transform.position - ball.transform.position;
		
		//Move camera to be above self, relative to the ball
		Camera.main.transform.position = transform.position + relativePosition;

		//Look down towards sphere
		Camera.main.transform.LookAt(ball.transform, Camera.main.transform.up);
		
		//Rotate player model accordingly
		transform.rotation = Camera.main.transform.rotation;
		transform.Rotate(90,0,0);
	}
	
	void Move(float h, float v){
		//Move in a tangent line (Because camera plane is tangent to sphere)
		transform.position += Camera.main.transform.right*h;
		transform.position += Camera.main.transform.up*v;
		
		//Snap back to sphere's surface
		transform.position = transform.position.normalized*radius;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Colliding with another player.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Not colliding with another player.");
        }
    }
}
