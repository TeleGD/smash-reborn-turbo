using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]


public class RandyJump : MonoBehaviour
{
    PlayerControls controls;

    [Header("hitbox of the attack")]
    public Transform attackpoint;
    public float range;

    [Header("Jump details")]
    public float jumpForce;
    public float jumptime;
    private float jumpcounter;
    public bool allowjump;

    [Header("DoubleJump details")]
    public bool allowdoublejump;
    public bool touchedground;
    public float dbjumpForce;
    public float dbjumptime;
    private float dbjumpcounter;
    public float dbjumpdelay;
    public float dbjumpdelaycounter;
    public bool jumplache;


    [Header("Ground details")]
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float radOcircle;
    [SerializeField] private float hauteurgi;
    [SerializeField] private float largeurgi;
    [SerializeField] private LayerMask whatisground;

    [Header("Platform")]
    public bool platformed;
    [SerializeField] private float plathbx;
    [SerializeField] private float plathby;
    public int platdowntime;
    public int platdowncnt;
    [SerializeField] private LayerMask whatisplatform;

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




        if (this.CompareTag("Player1"))
        {
            controls.gameplay.jump.performed += ctx => pressedjump = true;
            controls.gameplay.jump.canceled += ctx => pressedjump = false;
            controls.gameplay.down.performed += ctx => presseddown = true;
            controls.gameplay.down.canceled += ctx => presseddown = false;
        }
        else if (this.CompareTag("Player2"))
        {
            controls.gameplay.jump1.performed += ctx => pressedjump = true;
            controls.gameplay.jump1.canceled += ctx => pressedjump = false;
            controls.gameplay.down1.performed += ctx => presseddown = true;
            controls.gameplay.down1.canceled += ctx => presseddown = false;
        }



        rb = GetComponent<Rigidbody2D>();
        jumpcounter = jumptime;
        dbjumpdelaycounter = dbjumpdelay;
        dbjumpcounter = dbjumptime;
        myanim = GetComponent<Animator>();

    }

    private void FixedUpdate()
    {

        if (platdowncnt > 0)
        {
            platdowncnt -= 1;

        }

        HandleLayers();


        platformed = Physics2D.OverlapArea(new Vector2(groundcheck.position.x - (plathbx / 2), groundcheck.position.y + plathby / 2), new Vector2(groundcheck.position.x + plathbx / 2, groundcheck.position.y - plathby / 2), whatisplatform);

        if (platformed && !presseddown && !pressedjump && platdowncnt == 0)
        {
            if (horizontal == 0)
            {
                rb.velocity = new Vector2(0, 0);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }
        else if (presseddown && platformed)
        {
            platdowncnt = platdowntime;
            platformed = false;
        }
        if (!platformed)
        {
            rb.gravityScale = 1;
        }

        horizontal = GetComponent<Charamov>().horizontal;
        grounded = Physics2D.OverlapCircle(groundcheck.position, radOcircle, whatisground);
        Checkground();

        //normal jump

        if (pressedjump && grounded && !GetComponent<charavalues>().shielded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            myanim.SetTrigger("jump");

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
            myanim.SetTrigger("jump");
        }
        if (rb.velocity.y < 0)
        {
            myanim.SetBool("falling", true);
        }


        //double jump


        if (pressedjump && !grounded && allowdoublejump && touchedground && jumplache)
        {
            rb.velocity = new Vector2(rb.velocity.x, dbjumpForce);
            myanim.SetTrigger("jump");
            touchedground = false;
        }

        if (!grounded && pressedjump && dbjumpcounter > 0 && allowdoublejump && jumplache)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            dbjumpcounter -= Time.deltaTime;
            myanim.SetTrigger("jump");
            touchedground = false;
        }
        if(dbjumpcounter<=0)
        {
            allowdoublejump = false;
        }

    }



    void Checkground()
    {
        if (platformed)
        {
            grounded= true;
        }
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
