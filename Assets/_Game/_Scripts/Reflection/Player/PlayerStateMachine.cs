using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMKOC.Reflection
{
    public interface IPlayerState
    {
        void Enter(PlayerStateMachine player);
        void Exit(PlayerStateMachine player);
        void Update(PlayerStateMachine player);

        void Jump(PlayerStateMachine player);
    }

    public class PlayerStateMachine : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float jumpHeight = 10f;
        public Transform groundCheck;
        public LayerMask groundMask;
        public float groundDistance = 0.2f;

        private Rigidbody2D rb;

        public Rigidbody2D Rb => rb;
        private Animator animator;

        private SpriteRenderer m_SpriteRenderer;

        private IPlayerState currentState;

        public float moveX;

        public Button m_LeftButton, m_RightButton;

        public bool m_IsDead = true;



        public void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();

            GameManager.OnFirstTimeGameStartAction += OnFirstTimeGameStartAction;
            GameManager.OnGameStart += OnGameStart;
            GameManager.OnGameLoose += OnGameLoose;
            GameManager.OnGameOver += OnGameOver;
            TutorialManager.Instance.OnTutorialStarted += OnTutorialStarted;
            TutorialManager.Instance.OnTutorialEnded += OnTutorialEnded;

        }

        private void OnFirstTimeGameStartAction()
        {
             rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }

        private void OnGameLoose()
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            m_IsDead = true;
        }

        private void OnTutorialEnded(int obj)
        {
            if (obj == TutorialIds.mirrorTutorial)
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
            
        }

        private void OnGameStart()
        {
            if(m_IsDead){
            transform.position = (ReflectionLevelManager.Instance.GetCurrentLevel() as ReflectionLevel).m_PlayerSpawnPoint.position;
            rb.isKinematic = false;
            m_IsDead = false;
            }

        }

        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        public void OnDisable()
        {
            GameManager.OnFirstTimeGameStartAction -= OnFirstTimeGameStartAction;
            GameManager.OnGameStart -= OnGameStart;
            GameManager.OnGameLoose -= OnGameLoose;
            GameManager.OnGameOver -= OnGameOver;
            TutorialManager.Instance.OnTutorialStarted -= OnTutorialStarted;
            TutorialManager.Instance.OnTutorialEnded -= OnTutorialEnded;


        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            ChangeState(new IdleState());
        }

        private void Update()
        {
            #if UNITY_EDITOR
            if(Input.GetButtonDown("Jump"))
            CallJumpOnState();
            #endif 
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
           
                
            if (direction > 0)
                m_SpriteRenderer.flipX = false;
            if (direction < 0)
                m_SpriteRenderer.flipX = true;
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

             if (TutorialManager.Instance.IsTutorialActive){
                TutorialEventManager.Instance.TriggerEvent("event_tutorial_movement");
                Invoke(nameof(HardCodePointUpOnMovementButton),1);
                
            }
        }

        public void HardCodePointUpOnMovementButton()
        {
            m_LeftButton.GetComponent<EventTrigger>().OnPointerUp(new PointerEventData(EventSystem.current));
            m_RightButton.GetComponent<EventTrigger>().OnPointerUp(new PointerEventData(EventSystem.current));

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

        public void SetMoveX(float direction)
        {
            moveX = direction;
        }

        public void CallJumpOnState()
        {
            if (TutorialManager.Instance.IsTutorialActive)
                TutorialEventManager.Instance.TriggerEvent("event_tutorial_jump");
            currentState?.Jump(this);
        }

        
    }
}
