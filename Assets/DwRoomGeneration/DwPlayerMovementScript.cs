using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//i wanna add momentum when jumping, ill have to change the way speed work later


public class DwPlayerMovementScript : MonoBehaviour
{
    public enum PlayerState
    {
        Walking,
        Sprinting,
        Falling
    }

    [Header("Inputs")]
    [SerializeField] private float sensitivity = 225; //mouse sensitivity degree per second
    private int horizontal; //right/left
    private int vertical; //forward/backward
    private float mouseX; //mouse x move
    private float mouseY; //mouse y move
    public bool isGrounded = true;


    [Header("InputKey")]
    private KeyCode RightKey = KeyCode.D;
    private KeyCode LeftKey = KeyCode.A;
    private KeyCode ForwardKey = KeyCode.W;
    private KeyCode BackwardKey = KeyCode.S;
    private KeyCode SprintKey = KeyCode.LeftShift;
    private KeyCode JumpKey = KeyCode.Space;


    [Header("PlayerCharacter")]
    [SerializeField] private float walkSpeed = 12f;
    [SerializeField] private float sprintSpeed = 18f;
    [SerializeField] private float feetToGround; //offset for ground checking
    [SerializeField] private float groundDrag; //for rigidbody
    public float playerHeight;
    public float centerToFeet; //usually half of player's height
    private float baseSpeed; //after applying state
    private float speed; //total speed (if in the future we want to add item)
    public PlayerState playerState; //PlayerState.Jumping, PlayerState.Sprinting, PlayerState.Falling
    public Vector3 moveDirection = Vector3.zero; //movement Direction in the world.
    private Rigidbody rb;
    

    [Header("Movement:Jump")]
    [SerializeField] private float jumpStrength = 13f; //upward speed/second
    private bool isJumping = false;

    [Header("Gravity")]
    [SerializeField] private float playerGravity = 9.81f;
    [SerializeField] private float gravityMultiplier = 1f;


    [Header("Camera")]
    private GameObject mainCamera;
    private float cameraX = 0; //latitude way of change (like when you nod your head), store camera.rotation.x (aka rotation on x axis or kinda like the magnetic flow of wire)


    [Header("Raycast")]
    [SerializeField] private LayerMask layer; //at what layer the line is drawn (Just like picture editing tools layering)
    private float groundFootingRange = 0.52f; //the highest point on body when collision still counts




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = this.transform.GetChild(0).gameObject; //get first child, must be main camera
        rb = this.GetComponent<Rigidbody>();
        speed = baseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        updateMovementInput();
        updateCharacter();
        handleState();
    }

    private void FixedUpdate() //is basically the collider physics frame (50fps)
    {
        movement();
    }




    //Functions ==============================================================

    //this function is for checking input
    private void updateMovementInput() //Checks for input - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    { //called once per Update()
        headAxisX();
        headAxisY();
        horizontalKey();
        verticalKey();
        jumpKey();
        getGround();
    } //Checks for input - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

    //function that checks and apply player state
    private void handleState()
    { //called once per Update()
        if (isGrounded)
        {
            if (Input.GetKey(SprintKey))
            {
                playerState = PlayerState.Sprinting;
                baseSpeed = sprintSpeed;
            }
            else
            {
                playerState = PlayerState.Walking;
                baseSpeed = walkSpeed;
            }
        }
        else if (!isGrounded) //when player is not touching the ground
        {
            playerState = PlayerState.Falling;
        }
    }

    //function for translating input into movement. will be called on physics frame
    private void movement() //moves and rotates the character - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    {
        //apply speed
        speed = baseSpeed; //baseSpeed was affected by playerState
        moveDirection = Vector3.zero;

        //apply drag
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        { //when on air, you dont take drag
            rb.linearDamping = 0.5f;
        }

        //gravity
        if (isGrounded) //when player is on ground, dont apply gravity, instead apply current velocity
        {
            moveDirection.y = 0;
        }
        else //when player is mid air, apply gravity
        {
            //moveDirection.y = -playerGravity * gravityMultiplier;
        }

        //normalize input
        moveDirection += (transform.forward*vertical + transform.right * horizontal).normalized * speed;
        
        jumpCheck();

        //apply movement
        rb.AddForce(moveDirection * 10 * Time.fixedDeltaTime, ForceMode.VelocityChange);

        //rotation
        this.transform.Rotate(Vector3.up * mouseX * Time.deltaTime); //rotate on y axis
        cameraX -= mouseY * Time.deltaTime;
        cameraX = Mathf.Clamp(cameraX, -89f, 89f); //ensures the camera does not over rotate
        mainCamera.transform.localRotation = Quaternion.Euler(Vector3.right * cameraX); //rotate on x axis
    }//moves and rotates the character - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -




    //jumping
    private void jumpCheck() //add jump force to rigidbody
    {
        //process input
        if (isJumping && isGrounded)
        {
            rb.AddForce(jumpStrength * Vector3.up, ForceMode.Impulse);
        }
    }

    //ground Checking
    private void getGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, -Vector3.up);
        if (Physics.Raycast(ray, out hit, centerToFeet+feetToGround, layer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    //roof Checking
    private bool getRoofed()
    {
        bool isRoofed = false;
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, Vector3.up);
        if (Physics.Raycast(ray, out hit, 1.05f, layer))
        {
            if (Vector3.Distance(this.transform.position, hit.point) > groundFootingRange)
            {
                isRoofed = true;
            }
        }
        return isRoofed;
    }

    //gets the height of character based on renderer and update the "center to feet" distance
    private void updateCharacter()
    {
        playerHeight = this.gameObject.GetComponent<Renderer>().bounds.size.y; //get player height
        centerToFeet = playerHeight / 2;
    }




    //Keys ===================================================================

    //update jump key, affects isJumping (as in intention wise)
    private void jumpKey()
    {
        if (Input.GetKeyDown(JumpKey))
        {
            isJumping = true;
        }
        if (Input.GetKeyUp(JumpKey))
        {
            isJumping = false;
        }
    }

    //update vertical rotation (lat, minecraft uses lat as well aka latitude. hopefully i will learn to make a minecraft mod by 2026)
    private void headAxisY()
    {
        mouseY = Input.GetAxis("Mouse Y") * sensitivity;
    }

    //update horizontal rotation
    private void headAxisX()
    {
        mouseX = Input.GetAxis("Mouse X") * sensitivity;
    }
    

    //update horizontal key for movement
    private void horizontalKey()
    {
        horizontal = 0;
        if (Input.GetKey(RightKey))
        {
            horizontal += 1;
        }
        if (Input.GetKey(LeftKey))
        {
            horizontal += -1;
        }
    }


    //update vertical key for movement
    private void verticalKey()
    {
        vertical = 0;
        if (Input.GetKey(ForwardKey))
        {
            vertical += 1;
        }
        if (Input.GetKey(BackwardKey))
        {
            vertical += -1;
        }
    }
}
