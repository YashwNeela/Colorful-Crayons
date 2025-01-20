using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



namespace TMKOC.Reflection
{
    public class PlayerController : SerializedMonoBehaviour
    {

        Animator m_Animator;
        public float moveSpeed = 5f;      // Speed of movement
        public float jumpHeight = 2f;    // Height of the jump
        public float gravity = -9.81f;   // Gravity value

        private Rigidbody2D rb;
        private Vector2 velocity;
        private bool isGrounded;

        private bool inAir;

        public Joystick m_Joystick;

        public Button m_JumpButton;

        public Transform groundCheck;    // A transform used to check if the player is on the ground
        public float groundDistance = 0.4f; // Radius of the circle to check ground
        public LayerMask groundMask;     // Mask to define what is ground

        public LayerMask wallMask;

        void Awake()
        {
            m_JumpButton.onClick.AddListener(() =>
            {
                if (isGrounded)
                {
                    if (TutorialManager.Instance.IsTutorialActive)
                        TutorialEventManager.Instance.TriggerEvent("event_tutorial_jump");

                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(jumpHeight * -2f * gravity));
                    m_Animator.SetTrigger("Jump");
                }

            });
        }

        public void OnEnable()
        {
            GameManager.OnGameStart += OnGameStart;
            GameManager.OnGameOver += OnGameOver;
            
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



        public void SetMoveX(float value)
        {
            if (TutorialManager.Instance.IsTutorialActive && value == 0)
                TutorialEventManager.Instance.TriggerEvent("event_tutorial_movement");

            Debug.Log("Moving " + value);
            moveX = value;
        }


        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            m_Animator = GetComponent<Animator>();
        }

        float moveX;
        void Update()
        {
            if (GameManager.Instance.CurrentGameState != GameState.Playing)
            {
                return;
            }
            // Check if the player is grounded
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);

            m_Animator.SetBool("Ground", isGrounded);
            // Get input for movement

            if (Mathf.Abs(moveX) > 0.1f && isGrounded && m_CanMove)
            {

                if (moveX > 0)
                    DoMove(1);
                else
                    DoMove(-1);


            }
            else
            {
                m_Animator.SetFloat("Speed", 0);
            }
            if (inAir)
            {
                Vector2 move = new Vector2(moveX * moveSpeed, rb.velocity.y);
                rb.velocity = move;
            }

            //  Jumping
            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(jumpHeight * -2f * gravity));
            }
        }

        void OnDrawGizmosSelected()
        {
            // Draw the ground check circle in the editor for debugging

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
            Gizmos.DrawRay(transform.position,
                GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right
            );

        }

        bool m_CanMove = true;
        bool CanMove(int direction)
        {

            // Perform the raycast
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                Vector2.right * direction,
                3,
                wallMask
            );

            // Visualize the ray in the editor
            //Debug.DrawRay(transform.position, transform.right * 1, Color.red);
            // Check if the ray hit anything
            if (hit.collider != null)
            {
                Debug.Log($"Hit {hit.collider.name} at {hit.point}");
                DOTween.KillAll();
                return false;
            }
            else
            {

                return true;
            }

        }
        [Button]
        public void DoMove(int direction)
        {
            if (direction == 1){
                if(!CanMove(1))
                    return;
                transform.GetComponent<SpriteRenderer>().flipX = false;
            }
            else{
                if(!CanMove(-1))
                    return;
                transform.GetComponent<SpriteRenderer>().flipX = true;
            }
            m_CanMove = false;
          
            m_Animator.SetFloat("Speed", Mathf.Abs(1));

            transform.DOJump((Vector2)transform.position + (Vector2.right * direction * 2f), 0.5f, 1, 0.5f).OnComplete(() =>
            {
                Invoke(nameof(SetCanMove), 0.5f);
            });
            
        }

        public void SetCanMove()
        {
            m_CanMove = true;
        }

        public void SetInAir(int value)
        {
            if (value == 1)
            {
                inAir = true;
                m_CanMove = false;
            }
            else
            {
                inAir = false;
                StartCoroutine(StaticCoroutine.Co_GenericCoroutine(1f, () =>
                {
                    m_CanMove = true;
                }));
            }
        }


        /// Sent when another object enters a trigger collider attached to this
        /// object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<TutorialTriggerer>(out TutorialTriggerer triggerer)){
                SetMoveX(0);
                TutorialManager.Instance.StartTutorial(triggerer.TutorialTriggererId);
            }
        }

    }
}