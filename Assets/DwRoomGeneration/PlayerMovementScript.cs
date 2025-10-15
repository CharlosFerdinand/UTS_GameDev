using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    private int horizontal;
    private int vertical;
    private bool sprinting;
    private bool jumping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateInput();
    }


    //this function is for checking input
    private void updateInput()
    {
        horizontal = mathRoofing(Input.GetAxis("Horizontal"));
        vertical = mathRoofing(Input.GetAxis("Vertical"));
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sprinting = true;
        }
        else
        {
            sprinting= false;
        }
        if (Input.GetKeyDown(KeyCode.Space)) //will require cooldown in movement function
        {
            jumping = true;
        }
        else
        {
            jumping = false;
        }
    }

    private int mathRoofing(float f)
    {
        if (f > 0)
        {
            return 1;
        }
        else if (f<0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
