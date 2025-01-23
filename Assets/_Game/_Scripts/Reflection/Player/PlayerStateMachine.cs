using UnityEngine;

namespace TMKOC.Reflection{
public interface IPlayerState
{
    void Enter(PlayerStateMachine player);
    void Exit(PlayerStateMachine player);
    void Update(PlayerStateMachine player);
}

public class PlayerStateMachine : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 10f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.2f;

    private Rigidbody2D rb;

    public Rigidbody2D Rb=> rb;
    private Animator animator;

    private IPlayerState currentState;

        

        public void OnEnable()
        {
            GameManager.OnGameStart += OnGameStart;
            GameManager.OnGameOver += OnGameOver;
            TutorialManager.Instance.OnTutorialStarted += OnTutorialStarted;
            TutorialManager.Instance.OnTutorialEnded += OnTutorialEnded;

        }

        private void OnTutorialEnded(int obj)
        {
            if(obj == TutorialIds.mirrorTutorial)
            {
                ControlsUI.Instance.EnableControls(ControlsUIConstants.jump);
                ControlsUI.Instance.EnableControls(ControlsUIConstants.movememnt);
                ControlsUI.Instance.EnableControls(ControlsUIConstants.mirror);


            }
        }

        private void OnTutorialStarted(int obj)
        {
            
        }

        private void OnGameOver()
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }

        private void OnGameStart()
        {
            transform.position = (ReflectionLevelManager.Instance.GetCurrentLevel() as ReflectionLevel).m_PlayerSpawnPoint.position;
            rb.isKinematic = false;

        }

        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        public void OnDisable()
        {
            GameManager.OnGameStart -= OnGameStart;
            GameManager.OnGameOver -= OnGameOver;
            

        }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeState(new IdleState());
    }

    private void Update()
    {
        currentState?.Update(this);
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public void Move(float direction)
    {
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(jumpHeight * -2f * Physics2D.gravity.y));
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);
    }

    public void SetAnimatorParameters(bool ground, float speed)
    {
        animator.SetBool("Ground", ground);
        animator.SetFloat("Speed", speed);
    }

     void OnDrawGizmosSelected()
        {
            // Draw the ground check circle in the editor for debugging

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        

        }
}
}
