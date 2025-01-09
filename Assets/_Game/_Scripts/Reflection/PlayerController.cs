using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
   public float moveSpeed = 5f;      // Speed of movement
    public float jumpHeight = 2f;    // Height of the jump
    public float gravity = -9.81f;   // Gravity value

    private Rigidbody2D rb;
    private Vector2 velocity;
    private bool isGrounded;

    public Joystick m_Joystick;

    public Button m_JumpButton;

    public Transform groundCheck;    // A transform used to check if the player is on the ground
    public float groundDistance = 0.4f; // Radius of the circle to check ground
    public LayerMask groundMask;     // Mask to define what is ground

    void Awake()
    {
        m_JumpButton.onClick.AddListener(()=>
        {
            if(isGrounded)
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(jumpHeight * -2f * gravity));
            
        });
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);
        
        
        // Get input for movement
        float moveX = (int)(m_Joystick.Horizontal);

        if(Mathf.Abs(moveX)> 0.1f && isGrounded && m_CanJump)
        {
            if(moveX > 0)
            DoJump(1);
            else
            DoJump(-1);
        }
        // Vector2 move = new Vector2(moveX * moveSpeed, rb.velocity.y);
        // rb.velocity = move;

        // Jumping
        // if (Input.GetButtonDown("Jump") && isGrounded)
        // {
        //     rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(jumpHeight * -2f * gravity));
        // }
    }

    void OnDrawGizmosSelected()
    {
        // Draw the ground check circle in the editor for debugging
        
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
  
    }

    bool m_CanJump = true;

    [Button]
    public void DoJump(int direction)
    {
        m_CanJump = false;
        if(direction == 1)
            transform.GetComponent<SpriteRenderer>().flipX = false;
        else
            transform.GetComponent<SpriteRenderer>().flipX = true;

        transform.DOJump((Vector2)transform.position + ( Vector2.right * direction *1.5f), 0.5f,1,0.5f).OnComplete(()=>
        {
            m_CanJump = true;
        });
    }
}
