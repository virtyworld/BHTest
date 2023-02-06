using System.Collections;
using Mirror;
using Mirror.Examples.NetworkRoom;
using UnityEngine;

public class PlayerComponent : NetworkBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private PlayerScore playerScore;
    [SerializeField] private PlayerDamageComponent playerDamageComponent;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 10f;
    [SerializeField] private float dashTime = 1f;

    private bool facingRight = true;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isDushing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        
        Running();
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartCoroutine(Jump());
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (!isDushing) StartCoroutine(MoveToTargetPosition());
        }
    }

    private IEnumerator Jump()
    {
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        isGrounded = false;
        yield return null;
    }

    private IEnumerator MoveToTargetPosition()
    {
        animator.Play("Upward");
        isDushing = true;
        float dashTime = 0;
        Vector3 targetPosition = transform.position + transform.forward * dashDistance;

        while (dashTime <= this.dashTime)
        {
            dashTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
            yield return null;
        }
       
        isDushing = false;
    }

    private void Running()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.W))
        {
            animator.SetBool("isRunning", true);
        }

        if (Input.GetKeyUp(KeyCode.A)|| Input.GetKeyUp(KeyCode.S)||Input.GetKeyUp(KeyCode.D)||Input.GetKeyUp(KeyCode.W))
            {
                animator.SetBool("isRunning",false);
            }
        Vector3 deltaMove = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) 
                            * speed * Time.deltaTime;
        transform.Translate(deltaMove);
    }
    
    
    
    [ServerCallback]
    private void OnCollisionEnter(Collision other)
    {
        PlayerComponent pc = other.gameObject.GetComponent<PlayerComponent>();
        
        if (pc!=null)
        {
            if (other.gameObject.CompareTag("Player") && isDushing && !pc.isDushing && !pc.playerDamageComponent.IsInvulnerable)
            {
                playerScore.score += 1;
                other.gameObject.GetComponent<PlayerDamageComponent>().AddDamageByDash();
            }
        }
    }
}