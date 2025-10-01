using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public float minJumpHeight = 1.0f;
    public float maxJumpHeight = 3.0f;
    public float maxJumpTime = 0.5f; // how long player can hold jump
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private bool isJumping;
    private float jumpTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Ground check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            isJumping = false;
            jumpTimer = 0f;
        }

        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Jump Pressed
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
            jumpTimer = 0f;
            velocity.y = Mathf.Sqrt(minJumpHeight * -2f * gravity);
        }

        // Hold jump to extend height
        if (isJumping && Input.GetKey(KeyCode.Space))
        {
            if (jumpTimer < maxJumpTime)
            {
                velocity.y = Mathf.Sqrt(maxJumpHeight * -2f * gravity);
                jumpTimer += Time.deltaTime;
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
