using System.Collections.Generic;
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
    [SerializeField] private float airDegrade = 0.2f; //[range of 0-1]the rate at which horizontal speed degrade by it's length when exceeding airSpeed. took a long time to build the code for it.
    private PlayerState playerState; //PlayerState.Jumping, PlayerState.Sprinting, PlayerState.Falling
    private float baseSpeed; //after applying state
    private float speed; //total speed (if in the future we want to add item)
    public Vector3 moveDirection = Vector3.zero; //current character velocity gained by movement()
    private Rigidbody rb;
    

    [Header("Movement:Jump")]
    [SerializeField] private float jumpDuration = 0.15f; //duration of the jump (the longer you hold the jump key, the higher the jump)
    [SerializeField] private float jumpStrength = 13f; //upward speed/second
    [SerializeField] private float jumpMaxCooldown = 0.5f; //wait time until jump is ready
    private float jumpTimer = 0f; //while it's 0, no jump
    private float jumpCooldown = 0f; //0 or lower means it's ready to jump
    private bool isJumping = false;
    private bool jumped = false;

    [Header("Gravity")]
    [SerializeField] private float playerGravity = 9.81f; //this is m/s^2
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private float multiplierDuration = 0.5f;
    private float multiplierTimer = 0f;
    //player gravity will continuously reduce moveDirection


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
        jumpKey();
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

        //gravity
        if (isGrounded) //when player is on ground
        {
            moveDirection.y = 0;
        }
        else //when player is mid air
        {//apply velocity reduction per second (reducing m/s on each second)
            moveDirection.y -= playerGravity * Time.deltaTime * spedUpFall();
        }

        //normalize input
        float normalH = normalizeVelocityAxis(horizontal, vertical);
        float normalV = normalizeVelocityAxis(vertical, horizontal);

        //horizontal velocity on ground
        if (isGrounded)
        {
            moveDirection.x = normalH * speed;
            moveDirection.z = normalV * speed;
        }
        else //horizontal velocity on air
        {
            if (
                Mathf.Sqrt(
                    moveDirection.x * moveDirection.x + moveDirection.z * moveDirection.z
                    ) > airSpeed
                )
            {//if length is longer than airSpeed, degrade length of horizontal vector by 0.3
                float normalX = normalizeVelocityAxis(moveDirection.x, moveDirection.z);
                float normalZ = normalizeVelocityAxis(moveDirection.z, moveDirection.x);
                float lengthXZ = Mathf.Sqrt(moveDirection.x * moveDirection.x + moveDirection.z * moveDirection.z);
                moveDirection.x -= Time.deltaTime * normalX * airDegrade; //speed degradation when on air.
                moveDirection.z -= Time.deltaTime * normalZ * airDegrade; //meaning that some speed are kept.

                //add air strafing when going upward
                if (jumped)
                {
                    moveDirection.x += 2 * normalH * speed * Time.deltaTime;
                    moveDirection.z += 2 * normalV * speed * Time.deltaTime;
                }
            }
            else
            {//strafe like falling
                moveDirection.x = normalH * speed;
                moveDirection.z = normalV * speed;
            }
        }
        jump();

        //apply accelaration
        rb.MovePosition(
            Time.deltaTime * (
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




    //speed up fall after reaching peak of height (trick for a satisfying gravity effect learned from the dev of isadora edge in youtube short)
    private float spedUpFall()
    {
        float multiplier = 1f;
        if (jumped)
        {
            multiplierTimer = multiplierDuration;
        }
        else if(multiplierTimer > 0f)
        {
            multiplierTimer -= Time.deltaTime;
            multiplier = gravityMultiplier;
        }
        return multiplier;
    }

    //normalize velocity on certain axis
    private float normalizeVelocityAxis(float main, float other)
    {
        float result = 0f;
        //using x^2 + y^2 = length as basis for normalization
        float sum = Mathf.Sqrt((main*main) + (other*other));
        if (sum == 0)
        {
            result = 0f;
        }
        else
        {
            result = main / sum;
        }
        return result;
    }

    //jumping
    private void jump() //add jump force to rigidbody
    {
        float value = 0;
        //timer
        if (jumpCooldown > 0f)
        {
            jumpCooldown -= Time.deltaTime;
        }
        if(jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }

        //process input
        if (isJumping && isGrounded)
        {
            jumped = true;
        }
        else if (jumped && !isJumping) //if they stop jumping, jumped stop
        {
            jumped = false;
        }

        //when jump happened
        if (jumped)
        {
            //initial, set cooldown and duration
            if (isGrounded && jumpTimer <= 0f && jumpCooldown <= 0f)
            {
                jumpTimer = jumpDuration;
                jumpCooldown = jumpMaxCooldown;
                value += jumpStrength;
            }
            else if(jumpTimer > 0f) //in jump duration
            {
                value += jumpStrength;
            }
            else
            {
                jumped = false;
            }
        }
        rb.AddForce(Vector3.up * value, ForceMode.Impulse);
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
