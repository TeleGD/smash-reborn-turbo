using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerJumpV2 : MonoBehaviour
{
    PlayerControls controls;


    [Header("Jump details")]
    public float jumpForce;
    public float jumptime;
    public float jumpcounter;
    public bool allowjump;

    [Header("Ground details")]
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float radOcircle;
    [SerializeField] private float hauteurgi;
    [SerializeField] private float largeurgi;
    [SerializeField] private LayerMask whatisground;
    [SerializeField] private LayerMask whatiswallleft;
    [SerializeField] private LayerMask whatiswallright;
    public bool grounded;

    [Header("Wall details")]
    public bool istouchingfrontleft;
    public bool istouchingfrontright;
    public Transform frontcheck;
    public bool wallslidingleft;
    public bool wallslidingright;

    [Header("Walljump details")]
    public bool walljumping;
    public float xwallForce;
    public float ywallForce;
    public float gravityscale;
    public bool allowwalljump;
    private float walljumpcounter;
    public float walljumptime;
    public float movecounter;
    public float initialmovecounter;


    [Header("Components")]
    private Rigidbody2D rb;
    private Animator myanim;
    public float horizontal;

    public bool pressedjump = false;


    private void Awake()
    {
        controls = new PlayerControls();

        controls.gameplay.jump.performed += ctx => pressedjump = true;
        controls.gameplay.jump.canceled += ctx => pressedjump = false;
        rb = GetComponent<Rigidbody2D>();
        jumpcounter = jumptime;
        myanim = GetComponent<Animator>();
    }


    private void FixedUpdate()
    {
        HandleLayers();

        horizontal = GameObject.Find("player").GetComponent<PlayerMovement>().horizontal;
        grounded = Physics2D.OverlapCircle(groundcheck.position, radOcircle, whatisground);
        istouchingfrontleft = Physics2D.OverlapCircle(frontcheck.position, radOcircle, whatiswallleft);
        istouchingfrontright = Physics2D.OverlapCircle(frontcheck.position, radOcircle, whatiswallright);

        Checkground();

        if (movecounter > 0 && !wallslidingleft && !wallslidingright)
        {
            movecounter -= Time.deltaTime;
        }
        if (movecounter < 0)
        {
            movecounter = 0;
            walljumping = false;
        }

        //normal jump
        if (pressedjump && grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            myanim.SetTrigger("jump");
        }
        if (!grounded && pressedjump && jumpcounter > 0 && !wallslidingleft && !wallslidingright && !walljumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpcounter -= Time.deltaTime;
            myanim.SetTrigger("jump");
        }
        if (!pressedjump && !grounded)
        {
            jumpcounter = 0;
            myanim.SetBool("falling", true);
            myanim.SetTrigger("jump");
        }
        if (rb.velocity.y < 0)
        {
            myanim.SetBool("falling", true);
        }

        //wall jump
        //wallsliding
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
        if (wallslidingleft || wallslidingright && !walljumping)
        {
            rb.gravityScale = gravityscale;
            rb.velocity = new Vector2(rb.velocity.x /4, rb.velocity.y / 4);
            if (!pressedjump)
            {
                allowwalljump = true;
                walljumpcounter = walljumptime;
                if (movecounter <= 0)
                {
                    movecounter = initialmovecounter;
                }
            }
        }
        else
        {
            rb.gravityScale = 1;
        }
        if (wallslidingright && allowwalljump && !grounded && pressedjump && horizontal <0)
        {
            rb.velocity = new Vector2(xwallForce, ywallForce);
            walljumping = true;
            if (walljumpcounter > 0)
            {
                allowwalljump = false;
            }
            walljumpcounter-= Time.deltaTime;
            myanim.SetTrigger("jump");

        }
        if (wallslidingleft && allowwalljump && !grounded && pressedjump && horizontal > 0)
        {
            rb.velocity = new Vector2(-xwallForce, ywallForce);
            walljumping = true;
            if (walljumpcounter > 0)
            {
                allowwalljump = false;
            }
            walljumpcounter -= Time.deltaTime;
            myanim.SetTrigger("jump");

        }
    }
    void Checkground()
    {
        if (grounded)
        {
            movecounter =0;
            allowjump = true;
            walljumping = false;
            jumpcounter = jumptime;
            myanim.ResetTrigger("jump");
            myanim.SetBool("falling", false);

        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(groundcheck.position, new Vector2(largeurgi, hauteurgi));
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
    void OnEnable()
    {
        controls.gameplay.Enable();
    }
    void OnDisable()
    {
        controls.gameplay.Disable();
    }
}
