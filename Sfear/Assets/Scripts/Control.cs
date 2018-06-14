using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Control : NetworkBehaviour {
	private GameObject ball;			//Reference to the Sfear
	private Vector3 relativePosition;	//Position of this player relative to the center of the Sfear
	private float radius;				//Radius of ball PLUS half height of capsule
	private float hspeed, vspeed;       //Horizontal and vertical speed (relative to camera plane)
    private float directionFacing = -90;
	private int tapCount = 0;           //How many times the user has clicked (this is to keep track of double-clicking)
    [SyncVar]
    private float doubleTapTimer = 0;   //Keeps track of how long between clicks
    [SyncVar]
    private float invisibleTimer = 0;   //Keeps track of how long the player has been invisible
    [SyncVar]
    private float invisibilityCooldownTimer = 0;
    [SyncVar]
    private float timeWasIt = 0f;       //Keeps track of the total duration the player was "it"
    [SyncVar]
    private int id = -1;

    //Public variables
    public float acceleration = 0.0005f;	    //How fast the player object responds to swiping
	public float maxSpeed = 0.015f;		        //Fastest possible move speed
	public float slowDown = 0.99f;		        //Multiplied by movement speed every step to make smooth movement
    public float  invisibilityDuration = 3.0f;  //How long the player can set themselves as invisible
    public float invisibilityCooldown = 6.0f;   //How long until the player can set themselves as invisible

    public Text invisibilityTimerText;

    [SyncVar]
    public bool canBeTagged = true;
    [SyncVar]
    public bool canMove = false;
    [SyncVar]
    public bool isInvisible = false;
    [SyncVar]
    public bool canBeInvisible = true;
    [SyncVar]
    public bool isIt = false;
	private bool red = false;
	private float invulnCooldown = 0;
    public Material defaultMaterial;
    public Material taggedItMaterial;
	public Material invulnMaterial;
	public GameObject playerModel;
	
	private int TEST;

    void Start () {
		TEST = 0;
		invisibleTimer = invisibilityDuration;
        invisibilityCooldownTimer = invisibilityCooldown;

        radius = 0.80f;
		
		//Get a reference to the ball
		ball = GameObject.FindWithTag("Ball"); 
		
		//Find relative position to sphere's center
		relativePosition = transform.position - ball.transform.position;
		
		Camera.main.transform.position = transform.position + relativePosition;
		Camera.main.transform.LookAt(ball.transform, Camera.main.transform.up);
	}
	
	void Update () {
		if(!red && isIt){
			playerModel.GetComponent<MeshRenderer>().material = taggedItMaterial;
			red = true;
		}
		else if(red && !isIt){
			playerModel.GetComponent<MeshRenderer>().material = invulnMaterial;
			Invoke("ChangeToWhite", invulnCooldown);
			red = false;
		}

        if (!isLocalPlayer){
			return;
		}

        //Set pre-round text, notifying player if they're it or not
        if (!GetComponent<GameTimer>().roundHasStarted) {
            if (isIt) {
                invisibilityTimerText.text = "You're it!";
            }
            else {
                invisibilityTimerText.text = "RUN!!!";
            }
        }

        if (canMove) {
            //Used to keep old settings while making movement framerate-independent
            //float dT = Time.deltaTime*60;

            //Slow down gradually
            hspeed *= slowDown/**d*T*/;
            vspeed *= slowDown/**dT*/;

            //If user's finger is held down, calculate move speed.
            if (Input.GetAxis("Tap") == 1) {
                //add mouse movement to this player's speed
                hspeed -= Input.GetAxis("Mouse X") * acceleration/**dT*/;
                vspeed -= Input.GetAxis("Mouse Y") * acceleration/*dT*/;

                //Clamp to max speed
                hspeed = Mathf.Clamp(hspeed, -maxSpeed, maxSpeed);
                vspeed = Mathf.Clamp(vspeed, -maxSpeed, maxSpeed);
            }

            //Move
            if (Move(hspeed, vspeed)) {
                //Find relative position to sphere's center
                relativePosition = transform.position - ball.transform.position;

                //Move camera to be above self, relative to the ball
                Camera.main.transform.position = transform.position + relativePosition;

                //Look down towards sphere
                Camera.main.transform.LookAt(ball.transform, Camera.main.transform.up);

                //Rotate player model accordingly
                transform.rotation = Camera.main.transform.rotation;
                transform.Rotate(-90, 0, 0);
				transform.Rotate(0, -Mathf.Rad2Deg*directionFacing+90, 0);
				TEST++;
            }

            
            if (isIt) {
                //test
                invisibilityTimerText.text = "You're it!";

                //Add on to the total time the player is it
                timeWasIt += Time.deltaTime;

                //Double-tapping for invisibility!
                if (Input.GetMouseButtonDown(0)) {
                    tapCount++;
                }
                if (tapCount > 0) {
                    doubleTapTimer += Time.deltaTime;
                }
                if (tapCount >= 2 && canBeInvisible) {
                    CmdSetInvisibile(true);
                    Debug.Log("Double-tapped.");
                    doubleTapTimer = 0.0f;
                    tapCount = 0;
                    //start cooldown timer
                    invisibilityCooldownTimer -= Time.deltaTime;
                }
                if (doubleTapTimer > 0.3f) {
                    doubleTapTimer = 0f;
                    tapCount = 0;
                }

                //If invisible, check to see if the player's invisibility duration is over
                if (isInvisible) {
                    invisibleTimer -= Time.deltaTime;
                    invisibilityTimerText.text = "Invisible for: " + Mathf.Ceil(invisibleTimer);

                    if (invisibleTimer < 0) {
                        CmdSetInvisibile(false);
                        invisibleTimer = invisibilityDuration;
                    }
                }
                //Display invisibility cooldown timer
                if (!canBeInvisible) {
                    invisibilityCooldownTimer -= Time.deltaTime;
                    if (!isInvisible) {
                        invisibilityTimerText.text = "Able to be invisible in: " + Mathf.Ceil(invisibilityCooldownTimer);
                    }
                }
                else {
                    if (!isInvisible) {
                        invisibilityTimerText.text = "Able to be invisible in: 0";
                    }
                }
                //check to see if player can become invisible again
                if (invisibilityCooldownTimer < 0) {
                    canBeInvisible = true;
                    invisibilityCooldownTimer = invisibilityCooldown;
                }
            }
            else
            {
                CmdSetInvisibile(false);
                invisibilityTimerText.text = "RUN!!!";
            }
        }
	}
	
	bool Move(float h, float v){
        float increment = 0.175f;
		
		//Save initial x & y position
        float initialX = transform.position.x;
        float initialY = transform.position.y;
        float initialZ = transform.position.z;
		
		Vector3 resetVector = new Vector3(initialX, initialY, initialZ);

        //Move in a tangent line (Because camera plane is tangent to sphere)
        transform.position += Camera.main.transform.right*h;
		transform.position += Camera.main.transform.up*v;

        //Get all colliders the player is in contact with, IGNORING TRIGGERS
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.04f, ~0, QueryTriggerInteraction.Ignore);
        if (hitColliders.Length >= 1){
			//Debug.Log("Number of Colliders hit: " + hitColliders.Length);

            //reset position to initial position
            transform.position = resetVector;
			
			//Get attempted move angle and speed
			float moveDir = Mathf.Atan2(v,h);
			float moveSpeed = new Vector2(h,v).magnitude;
			
			//Check for nearest free spot by checking angles outwards, up to 80 degrees to the side
			for(int i = 1; i < 9; i++){
				//Used to prevent people from speed boosting off of walls
				float friction = 1.0f/i;
				
				//Check in positive direction
				float newAngle = increment*i + moveDir;
				float newH = Mathf.Cos(newAngle)*moveSpeed;
				float newV = Mathf.Sin(newAngle)*moveSpeed;
				transform.position += Camera.main.transform.right*newH;
				transform.position += Camera.main.transform.up*newV;
				hitColliders = Physics.OverlapSphere(transform.position, 0.04f, ~0, QueryTriggerInteraction.Ignore);
				if (hitColliders.Length >= 1){
					transform.position = resetVector;
				}
				else{
					transform.position = transform.position.normalized * radius;
					hspeed = newH*friction;
					vspeed = newV*friction;					
					directionFacing = newAngle;
					return true;
				}
				
				//Check in negative direction
				newAngle = -increment*i + moveDir;
				newH = Mathf.Cos(newAngle)*moveSpeed;
				newV = Mathf.Sin(newAngle)*moveSpeed;
				transform.position += Camera.main.transform.right*newH;
				transform.position += Camera.main.transform.up*newV;
				hitColliders = Physics.OverlapSphere(transform.position, 0.04f, ~0, QueryTriggerInteraction.Ignore);
				if (hitColliders.Length >= 1){
					transform.position = resetVector;
				}
				else{
					transform.position = transform.position.normalized * radius;
					hspeed = newH*friction;
					vspeed = newV*friction;
					directionFacing = newAngle;
					return true;
				}
			}
			
			return false;
			/*
            //check to see if we can move at least in the x direction
            transform.position += Camera.main.transform.right * h;
            hitColliders = Physics.OverlapSphere(transform.position, 0.04f, ~0, QueryTriggerInteraction.Ignore);
            if (hitColliders.Length >= 1){
                transform.position = new Vector3(initialX, initialY, initialZ);

            }

            //check to see if we can move at least in the y direction
            transform.position += Camera.main.transform.up * v;
            hitColliders = Physics.OverlapSphere(transform.position, 0.04f, ~0, QueryTriggerInteraction.Ignore);
            if (hitColliders.Length >= 1){
                transform.position = new Vector3(initialX, initialY, initialZ);
            }
			*/
        }
        else{
            //Snap back to sphere's surface
            transform.position = transform.position.normalized * radius;
			directionFacing = Mathf.Atan2(v,h);
            return true;
        }
	}

    public bool CanBeTagged() {
        return canBeTagged;
    }
	
	public void ChangeToWhite(){
		if(!red && !isIt){
			playerModel.GetComponent<MeshRenderer>().material = defaultMaterial;
		}
	}

    [ClientRpc]
    public void RpcInvulnerable()
    {
        Debug.Log("Can't touch this");
        canBeTagged = false;
        //GetComponent<MeshRenderer>().material.color = Color.cyan;
        isIt = false;
    }

    [ClientRpc]
    public void RpcCanBeTagged(float cooldown)
    {
        Invoke("SetCanBeTagged", cooldown);
    }

    [ClientRpc]
    public void RpcChangeToIt()
    {
        //Material taggedItMaterial = Resources.Load("Materials/Materials/TaggedIt.mat", typeof(Material)) as Material;
        //GetComponent<MeshRenderer>().material = taggedItMaterial;
        //GetComponent<MeshRenderer>().material.color = Color.red;
        isIt = true;
    }
	
	[ClientRpc]
	public void RpcSetCooldown(float cooldown){
		invulnCooldown = cooldown;
	}

    //Sets the player as invisible
    [Command]
    void CmdSetInvisibile(bool b)
    {
        RpcSetInvisibile(b);
    }

    [ClientRpc]
    void RpcSetInvisibile(bool b)
    {
        isInvisible = b;
        //Debug.Log("Player is invisible: " + isInvisible);
        playerModel.GetComponent<MeshRenderer>().enabled = !b;
        canBeInvisible = false;
    }

    void SetCanBeTagged()
    {
        Debug.Log("Free Game");
        canBeTagged = true;
       //GetComponent<MeshRenderer>().material.color = Color.white;
    }

    public void SetCanMove(bool b)
    {
        canMove = b;
    }

    [ClientRpc]
    public void RpcSetCanMove(bool b)
    {
        canMove = b;
    }
    
    //Set player Id
    public void SetId(int i)
    {
        id = i;
    }

    //Get player id
    public int GetId()
    {
        return id;
    }

    //Get total time player was it
    public float GetTimeWasIt()
    {
        return timeWasIt;
    }
}
