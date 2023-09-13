using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerJump : MonoBehaviour
{
    PlayerControls controls;


    //rb.velocity = new Vector2(rb.velocity.x,jumpForce);

    [Header("Jump details")]
    public float jumpForce;
    public float jumptime;
    public float walljumptime;
    public float jumpcounter;
    public float walljumpcounter;
    public bool stoppedjumping;
    public bool jumpedonce;

    [Header("Ground details")]
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float radOcircle;
    [SerializeField] private float hauteurgi1;
    [SerializeField] private float largeurgi1;
    [SerializeField] private LayerMask whatisground;
    [SerializeField] private LayerMask whatiswallleft;
    [SerializeField] private LayerMask whatiswallright;


    public bool istouchingfrontleft;
    public bool istouchingfrontright;
    public Transform frontcheck;
    public bool wallslidingleft;
    public bool wallslidingright;

    public bool grounded;

    public bool walljumping;
    public float xwallForce;
    public float ywallForce;
    public float walljumpTime;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator myanim;
    public float horizontal;


    public bool pressedjump=false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpcounter = jumptime;
        myanim = GetComponent<Animator>();
        

    }

    private void Awake()
    {
        controls = new PlayerControls();

        controls.gameplay.jump.performed += ctx => pressedjump = true;
        controls.gameplay.jump.canceled += ctx => pressedjump = false;


    }

    private void FixedUpdate()
    {
        HandleLayers();

        horizontal = GameObject.Find("player").GetComponent<PlayerMovement>().horizontal;
        grounded = Physics2D.OverlapCircle(groundcheck.position, radOcircle, whatisground);


        if (grounded)
        {
            jumpedonce = false;
            stoppedjumping = false;
            walljumping = false;
            jumpcounter = jumptime;
            myanim.ResetTrigger("jump");
            myanim.SetBool("falling", false);
        }

        if (pressedjump && grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            stoppedjumping = false;
            myanim.SetTrigger("jump");
        }

        if (pressedjump && !stoppedjumping && jumpcounter > 0 && !wallslidingleft && !wallslidingright)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpcounter -= Time.deltaTime;
            myanim.SetTrigger("jump");
        }

        if (!pressedjump & !grounded)
        {
            jumpcounter = 0;
            walljumpcounter = 0;
            stoppedjumping = true;
            myanim.SetBool("falling", true);
            myanim.SetTrigger("jump");

            if (rb.velocity.y < 0)
            {
                myanim.SetBool("falling", true);
            }


        }
        istouchingfrontleft = Physics2D.OverlapCircle(frontcheck.position, radOcircle, whatiswallleft);
        istouchingfrontright = Physics2D.OverlapCircle(frontcheck.position, radOcircle, whatiswallright);

        if (istouchingfrontleft) 
        {
            wallslidingleft = true;
            wallslidingright = false;
        }
        else
        {
            if (istouchingfrontright)
            {
                wallslidingleft = false;
                wallslidingright = true;
            }
            else
            {
                wallslidingleft = false;
                wallslidingright = false;
            }
        }

        if (wallslidingleft || wallslidingright)
        {
            walljumpcounter = walljumptime;
           // rb.velocity = new Vector2(rb.velocity.x/10, rb.velocity.y/10);
            rb.gravityScale = rb.gravityScale/20;
        }
        else
        {
            rb.gravityScale = 1;
        }
        if (wallslidingleft || wallslidingright)
        {
            if (wallslidingleft && pressedjump && !grounded )
            {
                rb.velocity = new Vector2(-xwallForce, ywallForce);
                walljumping = true;
                // Invoke("setwallpumpingtofalse", walljumpTime);
            }
            else
            {
                if (wallslidingright && pressedjump && !grounded)
                {
                    rb.velocity = new Vector2(xwallForce, ywallForce);
                    walljumping = true;
                    // Invoke("setwallpumpingtofalse", walljumpTime);
                }
                else
                {
                  
                   // rb.velocity = new Vector2(0, 0);
                }
            }
        }


        if (walljumping & pressedjump & horizontal !=0 & jumpcounter !=0 )
        {

            jumpedonce = true;
            //walljumpcounter -= Time.deltaTime;
            //rb.velocity = new Vector2(xwallForce * -horizontal, ywallForce);
            //rb.AddForce(new Vector2(-xwallForce* Input.GetAxis("Horizontal"), ywallForce));
            myanim.SetTrigger("jump");
        }
    }
    
        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(groundcheck.position, new Vector2(largeurgi1, hauteurgi1));
            Gizmos.DrawSphere(frontcheck.position, radOcircle);
        }


        private void HandleLayers()
        {
        if (!grounded)
            {
                myanim.SetLayerWeight(1, 1);
            }
            else
            {
                myanim.SetLayerWeight(1, 0);
            }
        }
    void setwallpumpingtofalse()
    {
        walljumping = false;
}

    void OnEnable()
    {
        controls.gameplay.Enable();
    }
    void OnDisable()
    {
        controls.gameplay.Disable();
    }
}