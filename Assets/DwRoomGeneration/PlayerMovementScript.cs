using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [Header("Inputs")]
    public int horizontal;
    public int vertical;
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





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateMovementInput();
        movement();
    }

    private void LateUpdate()
    {
        
    }




    //Functions ==============================================================

    //this function is for checking input
    private void updateMovementInput()
    {
        horizontalKey();
        verticalKey();
        sprintKey();
        jumpKey();
    }

    //function for translating input into movement
    private void movement()
    {
        moveDirection.z = vertical;
        moveDirection.x = horizontal;
        moveDirection.y = 0;
        this.transform.Translate(moveDirection * Time.deltaTime);
    }




    //Keys ===================================================================

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
