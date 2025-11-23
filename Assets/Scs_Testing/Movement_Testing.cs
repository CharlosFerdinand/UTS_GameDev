using Unity.VisualScripting;
using UnityEngine;

public class Movement_Testing : MonoBehaviour
{
    public enum State
    {
        MOBILE,
        IMMOBILE,
        RECOVERY
    }

    //initial state
    public State state = State.MOBILE;

    //stuff for movement
    private bool jumpTrigger = false;
    private float speed = 5f;
    private bool ground = false;
    private float jumpStrength = 1.6f;

    //stuff for grounded
    private float rideHeight = 0.95f;
    private float springStrength = 100f;
    private float dampenerStrength = 20f;
    private float immobilize_time = 0.5f;
    private float immobilize_duration;

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
        //camera and rotation
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        this.transform.Rotate(new Vector3(0, mouseX, 0));
        cam.Rotate(new Vector3(-mouseY, 0, 0));
        cam.rotation = new Quaternion(Mathf.Clamp(cam.rotation.x, -89f, 89f), cam.rotation.y, cam.rotation.z, cam.rotation.w);

        //Input (since it only work in update)
        jumpTrigger = Input.GetKeyDown(KeyCode.Space);
    }

    private void FixedUpdate()
    {
        //Raycast and standing
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction);
        if (Physics.Raycast(ray, out hit, rayLength, 1 << 7))
        {
            if (hit.distance < 0.3f && state == State.MOBILE)
            {
                state = State.IMMOBILE;
                immobilize_duration = immobilize_time;
            }
            standing(hit);
            ground = true;
        }
        else
        {
            ground = false;
            springStrength = 100f;
        }


        //movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 movedir = (transform.right * h + transform.forward * v) * speed;

        Vector3 current_vel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 change_vel = movedir - current_vel;
        
        if(state == State.MOBILE)
        {
            rb.AddForce(change_vel, ForceMode.VelocityChange);
            if (jumpTrigger && ground)
            {
                jumpTrigger = false;
                //disable spring temporarily (the one in standing())
                springStrength = 0;
                Vector3 jumpDir = new Vector3(0, jumpStrength, 0);
                rb.AddForce(jumpDir, ForceMode.Impulse);
            }
        }
    }


    //to make the body stay on standing level
    void standing(RaycastHit hit)
    {
        float x = hit.distance - rideHeight;
        float springforce = x * springStrength;
        float dampforce = rb.linearVelocity.y * dampenerStrength;

        
        
        if(state != State.IMMOBILE)
        {
            rb.AddForce(Vector3.down * springforce + Vector3.down * dampforce);
            if(state == State.RECOVERY && Mathf.Abs(Vector3.Distance(hit.point, this.transform.position) - rideHeight) < 0.1f)
            {
                state = State.MOBILE;
            }
        }
        else
        {
            immobilize_duration -= Time.fixedDeltaTime;
            if(immobilize_duration <= 0)
            {
                state = State.RECOVERY;
            }
        }
    }
}
