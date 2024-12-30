using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;      // Speed of movement
    public float jumpHeight = 2f;    // Height of the jump
    public float gravity = -9.81f;   // Gravity value

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;

    public Transform groundCheck;    // A transform used to check if the player is on the ground
    public float groundDistance = 0.4f; // Radius of the sphere to check ground
    public LayerMask groundMask;     // Mask to define what is ground

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep the player grounded
        }

        // Get input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        characterController.Move(move * moveSpeed * Time.deltaTime);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        // Draw the ground check sphere in the editor for debugging
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
