using Unity.VisualScripting;
using UnityEngine;

public class Movement_Testing : MonoBehaviour
{
    //states
    private bool ground = false;

    //Inputs
    private bool jumpTrigger = false;
    private bool sprintTrigger = false;

    //stuff for movement
    private float walkSpeed = 4f;
    private float sprintSpeed = 6f;
    private float speed = 0f;
    private float jumpStrength = 1.2f;
    private float gravityMultiplier = 4f;

    //stuff for floating rigidbody
    private float rideHeight = 0.95f; //distance to ground
    private float springStrength = 100f; //push/pull strength for locking in place like a spring
    private float dampenerStrength = 20f; //spring dampener, makes spring lose speed each iteration/bounce
    
    //stuff for camera movement
    private Transform cam;
    public float sensitivity = 2f;

    //raycast for feet
    private float rayLength = 1f;

    //component
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //Input (since it only work in update)
        jumpTrigger = Input.GetKeyDown(KeyCode.Space) && !jumpTrigger ? true:jumpTrigger;
        sprintTrigger = Input.GetKey(KeyCode.LeftShift);

        //camera and rotation
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        this.transform.Rotate(new Vector3(0, mouseX, 0));
        cam.Rotate(new Vector3(-mouseY, 0, 0));
        cam.rotation = new Quaternion(Mathf.Clamp(cam.rotation.x, -89f, 89f), cam.rotation.y, cam.rotation.z, cam.rotation.w);
    }

    private void FixedUpdate()
    {
        //Raycast, standing, ground checking
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction);
        if (Physics.Raycast(ray, out hit, rayLength, 1 << 7))
        {
            if (!ground)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            standing(hit);
            ground = true;
        }
        else
        {
            ground = false;
            springStrength = 100f;
            dampenerStrength = 20f;
        }

        movement();
    }


    //to make the body stay on standing level
    void standing(RaycastHit hit)
    {
        float x = hit.distance - rideHeight;
        float springforce = x * springStrength;
        float dampforce = rb.linearVelocity.y * dampenerStrength;

        rb.AddForce(Vector3.down * springforce + Vector3.down * dampforce);
    }

    void movement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (sprintTrigger)
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = walkSpeed;
        }

        //movement done using rigidbody addforce, velocity change mode
        Vector3 movedir = (transform.right * h + transform.forward * v) * speed;
        Vector3 current_vel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 change_vel = movedir - current_vel;

        rb.AddForce(change_vel, ForceMode.VelocityChange);
        if (jumpTrigger && ground)
        {
            jumpTrigger = false;
            //disable spring temporarily (the one in standing())
            springStrength = 0;
            dampenerStrength = 0;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Vector3 jumpDir = new Vector3(0, jumpStrength, 0);
            rb.AddForce(jumpDir, ForceMode.Impulse);
        }

        //vertical accel
        if(rb.linearVelocity.y > 0.01 || rb.linearVelocity.y < -0.01)
        {
            rb.AddForce(new Vector3(0, Mathf.Sign(rb.linearVelocity.y) * gravityMultiplier, 0), ForceMode.Acceleration);
        }
    }
}
