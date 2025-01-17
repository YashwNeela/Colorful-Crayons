using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



namespace TMKOC.Reflection{
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

    void Awake()
    {
        m_JumpButton.onClick.AddListener(()=>
        {
            if(isGrounded){
                if(TutorialManager.Instance.IsTutorialActive)
                    TutorialEventManager.Instance.TriggerEvent("event_tutorial_jump");

                rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(jumpHeight * -2f * gravity));
                m_Animator.SetTrigger("Jump");
            }
            
        });
    }

    public void OnEnable()
    {
        GameManager.OnGameStart +=   OnGameStart;
    }

        private void OnGameStart()
        {
           transform.position = (ReflectionLevelManager.Instance.GetCurrentLevel() as ReflectionLevel).m_PlayerSpawnPoint.position;
        }

        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
    public void OnDisable()
    {
        GameManager.OnGameStart -=   OnGameStart;
        
    }

       

    public void SetMoveX(float value)
    {
        if(TutorialManager.Instance.IsTutorialActive)
            TutorialEventManager.Instance.TriggerEvent("event_tutorial_movement");

        Debug.Log("Moving");
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
        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);
        
        m_Animator.SetBool("Ground",isGrounded);
        // Get input for movement

        if(Mathf.Abs(moveX)> 0.1f && isGrounded && m_CanMove)
        {
            
            if(moveX > 0)
            DoMove(1);
            else
            DoMove(-1);


        }else
        {
            m_Animator.SetFloat("Speed",0);
        }
        if(inAir){
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
  
    }

    bool m_CanMove = true;

    [Button]
    public void DoMove(int direction)
    {
        m_CanMove = false;
        if(direction == 1)
            transform.GetComponent<SpriteRenderer>().flipX = false;
        else
            transform.GetComponent<SpriteRenderer>().flipX = true;

        m_Animator.SetFloat("Speed",Mathf.Abs(1));
        
        transform.DOJump((Vector2)transform.position + ( Vector2.right * direction *2f), 0.5f,1,0.5f).OnComplete(()=>
        {
            Invoke(nameof(SetCanMove),0.5f);
        });
    }

    public void SetCanMove()
    {
        m_CanMove = true;
    }

    public void SetInAir(int value)
    {
        if(value == 1){
        inAir = true;
        m_CanMove = false;
        }
        else{
        inAir = false;
        StartCoroutine(StaticCoroutine.Co_GenericCoroutine(0.75f,()=>
        {
            m_CanMove = true;
        }));
        }
    }
    
}
}