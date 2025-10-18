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
    private KeyCode PauseKey = KeyCode.Escape; //to pause


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
    [SerializeField] private float jumpCooldown = 0.3f; //cooldown of jump
    private float jumpTimer = 0f; //timer
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
    [SerializeField] private TMP_Text uiDebugText;
    [SerializeField] private GameObject uiPauseScreen; //looking at how i name the variable, i think i need a rule for naming, such as preffix, main, suffix.




    //Game Lifecycle =======================================================================================================================================
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiPauseScreen.SetActive(false); //just in case that in editor, uiPauseScreen was set to active, deactivate it.
        mainCamera = this.transform.GetChild(0).gameObject; //get first child, must be main camera
        rb = this.GetComponent<Rigidbody>();
        hpScript = this.GetComponent<DwPlayerHpScript>();
        speed = baseSpeed;
        Time.timeScale = 1f; //ensures that start of the game time always move.
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
        if (hpScript.isAlive && Time.timeScale > 0f) //only run if player is alive and time is moving
        {
            movement();
        }
        isGrounded = false;
        onRigidGround = false;
    }

    //taking in velocity
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
        pauseKey();
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

        //if is not grounded but y velocity is almost 0 yet moveDirection indicates gravity appliance, then forcefully drag them downward to prevent wall sticking
        wallStickFix();
    }//moves the character - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -


    //rotates the character (called in update to give more responsiveness)
    private void rotatePlayer() //rotates the character - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    {
        if (Time.timeScale > 0) //rotate only when not paused
        {
            //rotation
            this.transform.Rotate(Vector3.up * mouseX); //rotate on y axis
            cameraX -= mouseY;
            cameraX = Mathf.Clamp(cameraX, -89f, 89f); //ensures the camera does not over rotate
            mainCamera.transform.localRotation = Quaternion.Euler(Vector3.right * cameraX); //rotate on x axis
        }
    }//rotates the character - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -




    //function to fix wall stick bug if it happened, must test: [using only y as a mean of detecting bug instead of using magnitude]
    private void wallStickFix()
    {
        /*
        //my solution number 2.
        if (
            (Vector3.right * moveDirection.x + Vector3.forward * moveDirection.z).magnitude > 0.1f &&
            rb.linearVelocity.magnitude < 0.1f &&
            !isGrounded
            )
        { //if moveDirection(x and z). magnitude is more than 0, yet velocity.magnitude is almost zero, and is not grounded. then forcefully drag it downward with transform.Translate() instead of rigidBody.
            this.transform.Translate(Vector3.up * moveDirection.y * Time.fixedDeltaTime);
        }
        //result: it kinda works, but using Time.fixedDeltaTime on transform.Translate might have caused some tearing in movement (not smooth), some test is needed in order to look further as to why this happen.
        //for now im gonna test my first solution
        //after testing first solution with Mathf.Abs(), tearing happened, even after changing it to Time.deltaTime. meaning that it might be because im running transform.Translate() in FixedUpdate().
        //now, im planning to instead manipulate velocity.y directly through linearVelocity.y
        */
        /*
        //my solution number 1 (my first solution)
        if (
            moveDirection.y > 0.1f &&
            rb.linearVelocity.y < 0.1f &&
            !isGrounded
            ) //if gravity is supposed to be applied, 
        {
            this.transform.Translate(Vector3.up * moveDirection.y * Time.fixedDeltaTime); //to see if it is the fixedDeltaTime (since fixedDeltaTime meant for rb)
        }
        */
        //i just realized it but, i forgot to use Mathf.Abs().
        /*
        if (
            Mathf.Abs(moveDirection.y) > 0.1f &&
            Mathf.Abs(rb.linearVelocity.y) < 0.1f &&
            !isGrounded
            ) //if gravity is supposed to be applied, 
        {
            this.transform.Translate(Vector3.up * moveDirection.y * Time.deltaTime); //to see if it is the fixedDeltaTime (since fixedDeltaTime meant for rb)
        }
        */
        //adding y moveDirection to y velocity is not effective, since it will slow down from time to time, resulting in downward movement by 1 step per second in a burst of speed
        /*
        if (
            Mathf.Abs(moveDirection.y) > 0.1f &&
            Mathf.Abs(rb.linearVelocity.y) < 0.1f &&
            !isGrounded
            ) //if gravity is supposed to be applied, 
        {
            rb.linearVelocity = moveDirection.y * Vector3.up + Vector3.right * rb.linearVelocity.x + Vector3.forward * rb.linearVelocity.z; //if i y velocity directly...
        }
        //result show that the same still happen, so maybe its best to just change x and z to 0 when wall stick gets detected.
        //or maybe its because everytime it gets fixed, the next frame, velocity changed because current frame change the velocity...
        //then i might have to run the wall stick fix in a different frame.
        */
        /*
        if (
            Mathf.Abs(moveDirection.y) > 0.1f &&
            Mathf.Abs(rb.linearVelocity.y) < 0.1f &&
            !isGrounded
            ) //if gravity is supposed to be applied, 
        {
           rb.AddForce(moveDirection.y * Vector3.up, ForceMode.Impulse); //removing everything but y
        }
        //still a failure, maybe i should just use raycast. maybe a raycast that aims upward.
        //im gonna try using raycast this time, as for how:
        //im gonna raycast from near the feet of the character, then after getting raycast as true, if toward the move direction, there is something...
        //wait, maybe i need to make sure that i dont apply force when potential wallstick is about to happen...
        //just now im testing random method and im starting to feel nauseous from trying to keep track of methods. now i cannot keep track anymore. right now i felt like i can puke on command. my body felt like gagging just now.
        //this time im not sure if it's because im nauseaous, but im very sure of my next method. use collision, then if move direction is roughly moving toward that direction, dont apply horizontal force. nvm that, that only means that they will still add force when no collision is detected yet, we cannot add offset since it might prevent collision from happening in the first place.

        //what if we can just realign move
        */
        // im just gonna make do as intended now.
        /*
        if (
            Mathf.Abs(moveDirection.y) > 0.1f &&
            Mathf.Abs(rb.linearVelocity.y) < 0.1f &&
            !isGrounded
            ) //if gravity is supposed to be applied, 
        {
            rb.linearVelocity += Vector3.up * moveDirection.y * Time.fixedDeltaTime; //apply continuous downward gravity
        }
        //fail
        */
        /*
        if (
            (Vector3.right * moveDirection.x + Vector3.forward * moveDirection.z).magnitude > 0.1f &&
            (rb.linearVelocity.z * Vector3.forward + rb.linearVelocity.x * Vector3.right).magnitude < 0.1f &&
            !isGrounded
            )
        { //if moveDirection(x and z). magnitude is more than 0, yet velocity.magnitude is almost zero, and is not grounded. then forcefully drag it downward with transform.Translate() instead of rigidBody.
            rb.linearVelocity = moveDirection.y * Vector3.up + Vector3.right * rb.linearVelocity.x + Vector3.forward * rb.linearVelocity.z;
        }
        //this time its more promisingm lets try using rb.linearVelocity.y this time...
        */
        /*
        if (
            (Vector3.right * moveDirection.x + Vector3.forward * moveDirection.z).magnitude > 0.1f &&
            (rb.linearVelocity.z * Vector3.forward + rb.linearVelocity.x * Vector3.right).magnitude < 0.1f &&
            !isGrounded
            )
        { 
            rb.linearVelocity = rb.linearVelocity.y * Vector3.up + Vector3.right * rb.linearVelocity.x + Vector3.forward * rb.linearVelocity.z;
        }
        //i see, now i got reminded that the source of problem is rb.linearVelocity being almost 0.
        //but this is still promising
        //fail 1, 2, 3, 4, 5, 6
        */
        /*
        if (
            (Vector3.right * moveDirection.x + Vector3.forward * moveDirection.z).magnitude > 0.1f &&
            (rb.linearVelocity.z * Vector3.forward + rb.linearVelocity.x * Vector3.right).magnitude < 0.1f &&
            !isGrounded
            )
        {
            rb.linearVelocity = rb.linearVelocity.y * Vector3.up;
            rb.AddForce(playerGravity * Vector3.up * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
        //fail, this is all i can for now (removing wall sticking) since im very much tired after working for 69.5 hours since monday to now (saturday this hour). but i do believe it can still be refined.
        */
        if (
            (Vector3.right * moveDirection.x + Vector3.forward * moveDirection.z).magnitude > 0.1f &&
            (rb.linearVelocity.z * Vector3.forward + rb.linearVelocity.x * Vector3.right).magnitude < 0.1f &&
            !isGrounded
            )
        {
            rb.linearVelocity = moveDirection.y * Vector3.up + Vector3.right * rb.linearVelocity.x + Vector3.forward * rb.linearVelocity.z;
        }
        //tho on the brighter side of things, no more wall sticking.
    }

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
        if (isJumping && isGrounded && jumpTimer <= 0f)
        {
            jumpTimer = jumpCooldown;
            rb.AddForce(jumpStrength * Vector3.up, ForceMode.Impulse);
        }
        else
        {
            jumpTimer -= Time.fixedDeltaTime;
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

    //update pause key, will pause upon activation
    private void pauseKey()
    {
        if (Input.GetKeyDown(PauseKey))
        {
            if (hpScript.isAlive)
            {
                //toggle pause
                if (Time.timeScale > 0f)
                {
                    Time.timeScale = 0f;
                }
                else
                {
                    Time.timeScale = 1f;
                }

                if (Time.timeScale == 0)
                { //pause the game
                    uiPauseScreen.SetActive(true); //show screen
                    Cursor.lockState = CursorLockMode.None; //unlock mouse
                }
            }
        }

        if (Time.timeScale > 0f)
        { //unpause the game
            uiPauseScreen.SetActive(false); //close screen
            Cursor.lockState = CursorLockMode.Locked; //lock mouse
        }
    }

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
        uiDebugText.text = "Stat\n" +
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
