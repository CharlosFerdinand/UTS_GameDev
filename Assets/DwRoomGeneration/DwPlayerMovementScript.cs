using System.Collections.Generic;
using UnityEngine;

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
    private bool isJumping;
    private bool isGrounded = true;


    [Header("InputKey")]
    private KeyCode RightKey = KeyCode.D;
    private KeyCode LeftKey = KeyCode.A;
    private KeyCode ForwardKey = KeyCode.W;
    private KeyCode BackwardKey = KeyCode.S;
    private KeyCode SprintKey = KeyCode.LeftShift;
    private KeyCode JumpKey = KeyCode.Space;


    [Header("Movement")]
    [SerializeField] private float airSpeed = 6f;
    [SerializeField] private float walkSpeed = 12f;
    [SerializeField] private float sprintSpeed = 18f;
    private PlayerState playerState; //PlayerState.Jumping, PlayerState.Sprinting, PlayerState.Falling
    private float baseSpeed; //after applying state
    private float speed; //total speed (if in the future we want to add item)
    private Vector3 moveDirection = Vector3.zero; //current character velocity gained by movement()
    private Rigidbody rb;


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
        applyState();
    }

    private void LateUpdate() //is basically the collider physics frame (50fps)
    {
        movement();
    }




    //Functions ==============================================================

    //this function is for checking input
    private void updateMovementInput() //Checks for input - - - - - - - - - -
    { //called once per Update()
        headAxisX();
        headAxisY();
        horizontalKey();
        verticalKey();
        getGround();
    }

    //function that checks and apply player state
    private void applyState()
    { //called once per Update()
        //check state
        stateCheck();

        //apply
        if (playerState == PlayerState.Falling)
        {
            baseSpeed = airSpeed; //speed when player is on air
        }
        else if (playerState == PlayerState.Walking)
        {
            baseSpeed = walkSpeed;
        }
        else if (playerState == PlayerState.Sprinting)
        {
            baseSpeed = sprintSpeed;
        }
    }

    //function for translating input into movement. will be called on physics frame
    private void movement() //moves and rotates the character - - - - - - - -
    {
        //apply speed
        speed = baseSpeed; //baseSpeed was affected by playerState


        //movement
        moveDirection.z = vertical;
        moveDirection.x = horizontal;
        moveDirection.y = 0;
        rb.MovePosition(
            Time.deltaTime * speed * (
            moveDirection.x * this.transform.right +
            moveDirection.y * this.transform.up +
            moveDirection.z * this.transform.forward
            )
            + this.transform.position
            );

        //rotation
        this.transform.Rotate(Vector3.up * mouseX * Time.deltaTime); //rotate on y axis
        cameraLatRotation();
    }




    //set the camera latitude according to it's current latitude
    private void cameraLatRotation()
    {
        cameraX -= mouseY * Time.deltaTime;
        cameraX = Mathf.Clamp(cameraX, -88f, 88f); //ensures the camera does not over rotate
        mainCamera.transform.localRotation = Quaternion.Euler(Vector3.right * cameraX); //rotate on x axis
    }


    //ground Checking
    private void getGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, -Vector3.up);
        if (Physics.Raycast(ray, out hit, 1.05f, layer))
        {
            if (Vector3.Distance(this.transform.position,hit.point) > groundFootingRange)
            {
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
        }
    }




    //Keys ===================================================================

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
        if (Input.GetKeyDown(RightKey))
        {
            horizontal = 1;
        }
        else if (Input.GetKeyDown(LeftKey))
        {
            horizontal = -1;
        }
        else if (Input.GetKeyUp(RightKey) || Input.GetKeyUp(LeftKey))
        {
            horizontal = 0;
        }
    }


    //update vertical key for movement
    private void verticalKey()
    {
        if (Input.GetKeyDown(ForwardKey))
        {
            vertical = 1;
        }
        else if (Input.GetKeyDown(BackwardKey))
        {
            vertical = -1;
        }
        else if (Input.GetKeyUp(ForwardKey) || Input.GetKeyUp(BackwardKey))
        {
            vertical = 0;
        }
    }




    //State Check ============================================================
    
    //check state
    private void stateCheck()
    {
        //when player is touching ground
        if (isGrounded)
        {
            if (Input.GetKey(SprintKey))
            {
                playerState = PlayerState.Sprinting;
            }
            else
            {
                playerState = PlayerState.Walking;
            }
        }
        else if (!isGrounded) //when player is not touching the ground
        {
            playerState = PlayerState.Falling;
        }
    }
}
