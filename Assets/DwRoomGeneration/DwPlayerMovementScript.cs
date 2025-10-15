using UnityEngine;

public class DwPlayerMovementScript : MonoBehaviour
{
    [Header("Inputs")]
    private int horizontal; //right/left
    private int vertical; //forward/backward
    private float mouseX; //mouse x move
    private float mouseY; //mouse y move
    [SerializeField] private float sensitivity = 225; //mouse sensitivity degree per second
    private bool sprinting;
    private bool jumping;


    [Header("InputKey")]
    private KeyCode RightKey = KeyCode.D;
    private KeyCode LeftKey = KeyCode.A;
    private KeyCode ForwardKey = KeyCode.W;
    private KeyCode BackwardKey = KeyCode.S;
    private KeyCode SprintKey = KeyCode.LeftShift;
    private KeyCode JumpKey = KeyCode.Space;


    [Header("Movement")]
    [SerializeField] private float baseSpeed = 8;
    private Vector3 moveDirection = Vector3.zero;

    [Header("Camera")]
    private GameObject camera;
    private float cameraX = 0; //latitude way of change (like when you nod your head), store camera.rotation.x (aka rotation on x axis or kinda like the magnetic flow of wire)





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = this.transform.GetChild(0).gameObject; //get first child, must be main camera
    }

    // Update is called once per frame
    void Update()
    {
        updateMovementInput();
    }

    private void LateUpdate()
    {
        movement();
    }




    //Functions ==============================================================

    //this function is for checking input
    private void updateMovementInput() //Checks for input - - - - - - - - - -
    {
        headAxisX();
        headAxisY();
        horizontalKey();
        verticalKey();
        sprintKey();
        jumpKey();
    }

    //function for translating input into movement
    private void movement() //moves and rotates the character - - - - - - - -
    {
        //movement
        moveDirection.z = vertical;
        moveDirection.x = horizontal;
        moveDirection.y = 0;
        this.transform.Translate(moveDirection * baseSpeed * Time.deltaTime);

        //rotation
        this.transform.Rotate(Vector3.up * mouseX * Time.deltaTime); //rotate on y axis
        cameraLatRotation();
    }

    //set the camera latitude according to it's current latitude
    private void cameraLatRotation()
    {
        cameraX -= mouseY * Time.deltaTime;
        cameraX = Mathf.Clamp(cameraX, -88f, 88f); //ensures the camera does not over rotate
        camera.transform.localRotation = Quaternion.Euler(Vector3.right * cameraX); //rotate on x axis
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
    
    //update jump key for movement
    private void jumpKey()
    {
        if (Input.GetKeyDown(JumpKey))
        {
            jumping = true;
        }
        else if (Input.GetKeyUp(JumpKey))
        {
            jumping = false;
        }
    }

    //update sprint key for movement
    private void sprintKey()
    {
        if (Input.GetKeyDown(SprintKey))
        {
            sprinting = true;
        }
        else if (Input.GetKeyUp(SprintKey))
        {
            sprinting = false;
        }
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
}
