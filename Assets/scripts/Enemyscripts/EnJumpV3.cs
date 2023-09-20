using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]


public class EnJumpV3 : MonoBehaviour
{
    PlayerControls controls;

    [Header("hitbox of the attack")]
    public Transform attackpoint;
    public float range;

    [Header("Jump details")]
    public float jumpForce;
    public float jumptime;
    public float jumpcounter;
    public bool allowjump;

    [Header("DoubleJump details")]
    public bool allowdoublejump;
    public bool touchedground;
    public float dbjumpForce;
    public float dbjumptime;
    public float dbjumpcounter;
    public float dbjumpdelay;
    public float dbjumpdelaycounter;
    public bool jumplache;


    [Header("Ground details")]
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float radOcircle;
    [SerializeField] private float hauteurgi;
    [SerializeField] private float largeurgi;
    [SerializeField] private LayerMask whatisground;

    public bool grounded;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator myanim;
    public float horizontal;

    public bool pressedjump = false;
    public bool presseddown = false;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.gameplay.jump1.performed += ctx => pressedjump = true;
        controls.gameplay.jump1.canceled += ctx => pressedjump = false;
        controls.gameplay.down1.performed += ctx => presseddown = true;
        controls.gameplay.down1.canceled += ctx => presseddown = false;
        rb = GetComponent<Rigidbody2D>();
        jumpcounter = jumptime;
        dbjumpdelaycounter = dbjumpdelay;
        dbjumpcounter = dbjumptime;
        myanim = GetComponent<Animator>();

    }

    private void FixedUpdate()
    {
        HandleLayers();

        horizontal = GameObject.Find("player1").GetComponent<PlayerMovement>().horizontal;
        grounded = Physics2D.OverlapCircle(groundcheck.position, radOcircle, whatisground);
        Checkground();

        //normal jump

        if (pressedjump && grounded)
        {
            myanim.SetTrigger("jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
           myanim.SetBool("falling", true);
        }
            

        if (jumpcounter <= 0)
        {
            dbjumpdelaycounter -= Time.deltaTime;
            if (dbjumpdelaycounter <= 0)
            {
                allowdoublejump = true;
            }

        }
        if (!pressedjump && !grounded)
        {
            jumplache = true;
            jumpcounter = 0;
            dbjumpcounter = 0;
            myanim.SetBool("falling", true);
        }
        if (rb.velocity.y < 0)
        {
            myanim.SetBool("falling", true);
        }


        //double jump


        if (pressedjump && !grounded && allowdoublejump && touchedground && jumplache)
        {
            myanim.SetTrigger("jump");
            rb.velocity = new Vector2(rb.velocity.x, dbjumpForce);
            myanim.SetBool("falling", true);
            UnityEngine.Debug.Log("doublejump1");
            touchedground = false;
        }

        if (!grounded && pressedjump && dbjumpcounter > 0 && allowdoublejump && jumplache)
        {
            UnityEngine.Debug.Log("doublejump2");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            dbjumpcounter -= Time.deltaTime;
            myanim.SetTrigger("jump");
            myanim.ResetTrigger("jump");
            myanim.SetBool("falling", true);

            touchedground = false;
        }
        if(dbjumpcounter<=0)
        {
            allowdoublejump = false;
        }



    }



    void Checkground()
    {
        if (grounded)
        {
            jumplache = false;
            allowjump = true;
            touchedground = true;
            jumpcounter = jumptime;
            dbjumpcounter = dbjumptime;
            dbjumpdelaycounter = dbjumpdelay;
            myanim.ResetTrigger("jump");
            myanim.SetBool("falling", false);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(groundcheck.position, new Vector2(largeurgi, hauteurgi));
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
