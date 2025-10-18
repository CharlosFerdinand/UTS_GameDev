using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//i wanna add momentum when jumping, ill have to change the way speed work later


public class DwPlayerMovementScript : MonoBehaviour
{
    public enum PlayerState
    {
        Walking,
        Sprinting,
        Falling
    }

    [Header("Inputs (play fullscreen for precise sensitivity)")]
    [SerializeField] private float sensitivity = 2f; //mouse sensitivity degree per update frame
    private int horizontal; //right/left
    private int vertical; //forward/backward
    private float mouseX; //mouse x move
    private float mouseY; //mouse y move
    private bool isGrounded = false;
    private bool onRigidGround = false;
    private Vector3 groundVelocity;
    private DwPlayerHpScript hpScript;


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
    
    //stats
    private float playerHeight;
    private float centerToFeet; //usually half of player's height
    private float baseSpeed; //after applying state
    private float speed; //total speed (if in the future we want to add item)

    //action
    private PlayerState playerState; //PlayerState.Jumping, PlayerState.Sprinting, PlayerState.Falling
    private Vector3 moveDirection = Vector3.zero; //movement Direction in the world.
    private Rigidbody rb;
    

    [Header("Movement:Jump")]
    [SerializeField] private float jumpStrength = 13f; //upward speed/second
    private bool isJumping = false;

    [Header("Gravity")]
    [SerializeField] private float playerGravity = 9.81f;
    [SerializeField] private float gravityAdditionalMultiplier = 1f;


    [Header("Camera")]
    private GameObject mainCamera;
    private float cameraX = 0; //latitude way of change (like when you nod your head), store camera.rotation.x (aka rotation on x axis or kinda like the magnetic flow of wire)


    [Header("Slope Raycast")]
    [SerializeField] private LayerMask layer; //at what layer the line is drawn (Just like picture editing tools layering)
    private float maxSlopeDegree = 36.87f; //max acceptable slope
    private RaycastHit slopeHit;

    [Header("Collision Raycast")] //to make sure that you dont stick to platform like a spider, has yet to be implemented
    private RaycastHit directionHit;


    [Header("ReadOnly Rb Velocity")]
    public float xVelocity;
    public float yVelocity;
    public float zVelocity;
    public float debugXDirection;
    public float debugYDirection;
    public float debugZDirection;
    public float debugSpeed;

    [Header("UI")]
    public TMP_Text uiDebugText;




    //Game Lifecycle =======================================================================================================================================
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = this.transform.GetChild(0).gameObject; //get first child, must be main camera
        rb = this.GetComponent<Rigidbody>();
        hpScript = this.GetComponent<DwPlayerHpScript>();
        speed = baseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        updateMovementInput();
        updateCharacter();
        handleState();
        rotatePlayer();
    }

    private void FixedUpdate() //is basically the collider physics frame (50fps)
    {
        readStats(); //read stats such as speed velocity, etc. the record are info from previous physics frame (aka FixedUpdate)
        writeStats();
        //movement is only applied when player is still alive.
        if (hpScript.isAlive)
        {
            movement();
        }
        isGrounded = false;
        onRigidGround = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "groundTag")
        {
            if (collision.gameObject.GetComponent<Rigidbody>() != null && Physics.Raycast(this.transform.position,Vector3.down, centerToFeet+feetToGround))
            {
                groundVelocity = collision.gameObject.GetComponent<Rigidbody>().linearVelocity / 2;
                onRigidGround = true;
            }
        }
    }

    //ground checking
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "groundTag")
        {
            isGrounded = true;
        }
    }




    //Functions ============================================================================================================================

    //this function is for checking input
    private void updateMovementInput() //Checks for input - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    { //called once per Update()
        headAxisX();
        headAxisY();
        horizontalKey();
        verticalKey();
        jumpKey();
        slopeGroundCheck();
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
    private void movement() //moves the character - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    {
        //apply speed
        speed = baseSpeed; //baseSpeed was affected by playerState
        speedLimit(); //this function make sure that character does not move more than the expected speed.
        moveDirection = getGroundVelocity(); //get the velocity of ground

        //apply drag
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        { //when on air, you dont take drag
            rb.linearDamping = 0f;
        }

        //gravity
        if (isGrounded) //when player is on ground, dont apply gravity, instead apply current velocity
        {
            moveDirection.y = 0;
        }
        else //when player is mid air, apply gravity
        {
            moveDirection.y = -playerGravity * gravityAdditionalMultiplier;
        }

        //add horizontal movement
        Vector3 horizontalMove = (transform.forward * vertical + transform.right * horizontal).normalized;
        if (onSlope())
        {//if player is on top of an acceptable slope (relevant degree an distance), apply slope movement
            horizontalMove = getSlopeNormalizedMove(horizontalMove) * speed;
        }
        else //else, apply horizontal move normally
        {
            horizontalMove = horizontalMove * speed;
        }

        //apply the horizontal movement
        moveDirection += horizontalMove;
        
        jumpCheck();

        //apply movement
        rb.AddForce(moveDirection * 10 * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }//moves the character - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -


    //rotates the character (called in update to give more responsiveness)
    private void rotatePlayer() //rotates the character - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    {
        //rotation
        this.transform.Rotate(Vector3.up * mouseX); //rotate on y axis
        cameraX -= mouseY;
        cameraX = Mathf.Clamp(cameraX, -89f, 89f); //ensures the camera does not over rotate
        mainCamera.transform.localRotation = Quaternion.Euler(Vector3.right * cameraX); //rotate on x axis
    }//rotates the character - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -




    //function for setting velocity of the ground
    private Vector3 getGroundVelocity()
    {
        if (onRigidGround)
        {
            return groundVelocity;
        }
        return Vector3.zero;
    }

    //function for taking dmg
    //private void 

    //is grounded if slopeHit degree correlate with it's distance
    private void slopeGroundCheck()
    {
        if (onSlope())
        {
            float dist = slopeHit.point.y - centerToFeet; //distance from feet to slope
            //get other degree of slope
            float otherDegree = 90f - Vector3.Angle(Vector3.up, slopeHit.normal);
            //calculate max range for distance
            float maxRadius = (playerHeight / 4f);
            float radius = deg2X(otherDegree,maxRadius);
            float distanceExtra = maxRadius / Mathf.Tan(otherDegree * Mathf.Deg2Rad);
            float maxRange = distanceExtra - maxRadius + Mathf.Sqrt(maxRadius * maxRadius - radius * radius);
            if (dist <= maxRange+0.14f && !isGrounded) //0.14f is error margin, 0.13f is the minimum for non slope to give grounded
            {
                isGrounded = true;
            }
        }
    }

    //project the movement direction according to a plane based on hit.normal as upward direction of the plane. return normalized direction
    private Vector3 getSlopeNormalizedMove(Vector3 moveDir)
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }

    //slope check
    private bool onSlope()
    {
        if (
            Physics.Raycast(this.transform.position, Vector3.down,
            out slopeHit,
            centerToFeet + getSlopeDistance() + 0.02f, 
            layer
            )
            ) //0.02f as offset
        {
            return Vector3.Angle(Vector3.up, slopeHit.normal) < maxSlopeDegree && slopeHit.collider.gameObject.tag != "groundTag";
        }
        return false;
    }

    //y' = -x / (r^2 - x^2)^(1/2)
    private float getSlopeDistance() //function for calculating distance
    {
        float maxRadius = playerHeight / 4;
        float radius = maxRadius * 0.6f; //radius of feet.
        float gradient = radius / Mathf.Sqrt(maxRadius * maxRadius - radius * radius); //this is the turunan of xkuadrat + ykuadrat equals to rkuadrat. but gradient will always be positive instead
        float slopeDegree = Mathf.Atan(gradient) * Mathf.Rad2Deg; //should be roughly 36.87 degree if rounded up, i still have my paper when i was calculating it manually at home
        float otherDegree = 90 - slopeDegree; //to get the degree of the other corner of my imaginary triangle
        float distanceExtra = radius / Mathf.Tan(otherDegree * Mathf.Deg2Rad); //this is the distance with extra y1
        float distance = distanceExtra - maxRadius + Mathf.Sqrt(maxRadius * maxRadius - radius * radius); //basically dikurangin maxRadius aka r lalu tambah y(radius) aka (r^2 - x^2)^(1/2)
        return distance;
    }

    private float deg2X(float degree, float maxRadius)
    {
        return maxRadius * Mathf.Cos(degree * Mathf.Deg2Rad);
    }

    //jumping
    private void jumpCheck() //add jump force to rigidbody
    {
        //process input
        if (isJumping && isGrounded)
        {
            rb.AddForce(jumpStrength * Vector3.up, ForceMode.Impulse);
        }
    }

    //speed limiter
    private void speedLimit()
    {
        Vector3 flatVelocity = rb.linearVelocity.x * Vector3.right + rb.linearVelocity.z * Vector3.forward;
        if (flatVelocity.magnitude > speed)
        {
            float y = rb.linearVelocity.y;
            Vector3 newVelocity = flatVelocity.normalized * speed;
            newVelocity += Vector3.up * y;
            rb.linearVelocity = newVelocity;
        }
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


    //reading stats ======================================================================================================================================
    private void readStats()
    {
        xVelocity = rb.linearVelocity.x;
        yVelocity = rb.linearVelocity.y;
        zVelocity = rb.linearVelocity.z;
        debugXDirection = moveDirection.x;
        debugYDirection = moveDirection.y;
        debugZDirection = moveDirection.z;
        debugSpeed = moveDirection.magnitude;
    }

    //update the TMP_text
    private void writeStats()
    {
        uiDebugText.text = "Debug\n" +
            "isGrounded: " + isGrounded + "\n" +
            "Speed: " + debugSpeed + "\n\n" +
            "X Velocity: " + xVelocity + "\n" +
            "Y Velocity: " + yVelocity + "\n" +
            "Z Velocity: " + zVelocity + "\n\n" +
            "X Direction: " + debugXDirection + "\n" +
            "Y Direction: " + debugYDirection + "\n" +
            "Z Direction: " + debugZDirection;
    }
}
