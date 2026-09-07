using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    //Assigns Unity player bindings to corresponding actions
    PlayerInput playerInput;
    InputAction inputActionMove;
    InputAction inputActionJump;
    InputAction inputActionDash;

    //Initialises variables

    //Basic Movement
    private Rigidbody RigidBody;  //RigidBody physics component to refer to
    private Vector3 moveDirection;
    public float playerVelocity;  //Creates parameter to pass through movement to change player velocity
    private float playerHeight;   //Player height
    public float jumpPower;       //Changes how high player jumps
    

    //Dashing
    public float dashSpeed;
    public float dashDuration;
    private float startTime;
    private float dashCooldown;

    bool OnGround() { return Physics.Raycast(transform.position, Vector3.down, playerHeight); }  //Shoots ray downwards to check if player is on ground, returns True/False
    
    void Start()
    {
        //Assigns Unity components to variables
        RigidBody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerHeight = GetComponent<CharacterController>().height;

        //Assigns Unity Input bindings to variables
        inputActionMove = playerInput.actions.FindAction("Move");
        inputActionJump = playerInput.actions.FindAction("Jump");
        inputActionDash = playerInput.actions.FindAction("Dash");
    }

    private void FixedUpdate() //Physics framerate tied calculations
    {
        //Adds more gravity to player for better feel
        RigidBody.AddForce(Physics.gravity * RigidBody.mass);
    }

    void Update() //Calls functions
    {
        //Directional Movement
        playerMovement();


        //Performs coroutine if dash button pressed and if time of dashCooldown has passed
        dashCooldown = dashCooldown - Time.deltaTime;

        if (inputActionDash.WasPressedThisFrame())
        {
            if(dashCooldown<=0)
            {
                StartCoroutine(Dash());
            }
            
        }



        //Calls Jump function if jump button pressed and on ground
        if (inputActionJump.WasPressedThisFrame() && OnGround())
        {
            Jump();
        }
        
        //Outputs current horizontal player speed to console, rounded to two decimal places
        Vector3 currentVelocity = new Vector3(RigidBody.linearVelocity.x, 0, RigidBody.linearVelocity.z);
        float velocity = (float)System.Math.Round(currentVelocity.magnitude, 2);
        Debug.Log("Speed: " + velocity);
    }

    void playerMovement()
    {
        //Reads 2D vector of movement inputs and converts to 3D vector to allow movement through 3D space
        Vector2 playerPos = inputActionMove.ReadValue<Vector2>();

        //Breaks camera into 3D vectors of X and Z axis corresponding to player movement. Y axis = 0 so horizontal movement allowed only
        Vector3 camX = Camera.main.transform.forward;
        Vector3 camZ = Camera.main.transform.right;
        camX.y = 0;
        camZ.y = 0;

        //Makes player move in direction camera is facing
        moveDirection = (camX.normalized * playerPos.y) + (camZ.normalized * playerPos.x); //Normalized to limit magnitude to prevent uncontrollable speeds
        RigidBody.linearVelocity += moveDirection * playerVelocity * Time.deltaTime; //Movement updates every frame
    } 
    void Jump()
    {
        //Adds force to the RigidBody physics component of character to simulate jump
        RigidBody.AddForce(Vector3.up  * jumpPower, ForceMode.Impulse);
    }


    //Dash Coroutine
    IEnumerator Dash()
    {

        //Allows dash in direction camera is facing and input direction
        Vector2 inputDirection = inputActionMove.ReadValue<Vector2>();
        Vector3 dashDirection = (Camera.main.transform.forward * inputDirection.y) + (Camera.main.transform.right * inputDirection.x);
        

        startTime = Time.time; //Gets total elapsed time since scene started

        while(Time.time < startTime + dashDuration) //Records time when dash button is pressed, repeats until the current time is the time dash is pressed + duration of dash
        {
            RigidBody.linearVelocity = (dashDirection * dashSpeed);
            dashCooldown = 1f; //Dash cooldown in seconds

            yield return null; //Repeats every frame
        }
    }

}
