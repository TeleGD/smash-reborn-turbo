using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]


public class PlayerJumpV3 : MonoBehaviour
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


    [Header("Ground details")]
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float radOcircle;
    [SerializeField] private float hauteurgi;
    [SerializeField] private float largeurgi;
    [SerializeField] private LayerMask whatisground;

    [Header("Walljump details")]
    public bool stuckinwall;
    public float gravity;
    public float wjforceside;
    public float wjforceup;
    public float lasthorizontal;
    public bool walljumpleftrdy;
    public bool walljumprightrdy;

    public bool grounded;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator myanim;
    public float horizontal;

    [Header("Corner")]
    public float cornlarg;
    public float cornhaut;
    public Transform corncheck;
    public float cornrange;
    public float cornstrver;
    public float cornstrhor;

    public bool pressedjump = false;
    public bool presseddown = false;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.gameplay.jump.performed += ctx => pressedjump = true;
        controls.gameplay.jump.canceled += ctx => pressedjump = false;
        controls.gameplay.down.performed += ctx => presseddown = true;
        controls.gameplay.down.canceled += ctx => presseddown = false;
        rb = GetComponent<Rigidbody2D>();
        jumpcounter = jumptime;
        dbjumpdelaycounter = dbjumpdelay;
        dbjumpcounter = dbjumptime;
        myanim = GetComponent<Animator>();

        gravity = rb.gravityScale;
    }

    private void FixedUpdate()
    {

        if (horizontal != 0f && grounded)
        {
            lasthorizontal = horizontal;
        }
        HandleLayers();

        if (grounded && (walljumpleftrdy ||walljumprightrdy))
        {
            walljumpleftrdy = false;
            walljumprightrdy = false;
        }

        horizontal = GameObject.Find("player1").GetComponent<PlayerMovement>().horizontal;
        grounded = Physics2D.OverlapCircle(groundcheck.position, radOcircle, whatisground);
        Checkground();

        //normal jump

        if (pressedjump && grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            myanim.SetTrigger("jump");
        }
        if (!grounded && pressedjump && jumpcounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpcounter -= Time.deltaTime;
            myanim.SetTrigger("jump");
            walljumpleftrdy = true;
            walljumprightrdy = true;
            
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


        if (pressedjump && !grounded && allowdoublejump && touchedground)
        {
            rb.velocity = new Vector2(rb.velocity.x, dbjumpForce);
            myanim.SetTrigger("jump");
            UnityEngine.Debug.Log("doublejump1");
            touchedground = false;
        }

        if (!grounded && pressedjump && dbjumpcounter > 0 && allowdoublejump)
        {
            UnityEngine.Debug.Log("doublejump2");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            dbjumpcounter -= Time.deltaTime;
            myanim.SetTrigger("jump");
            touchedground = false;
        }
        if(dbjumpcounter<=0)
        {
            allowdoublejump = false;
        }

        //wall jump
        if (stuckinwall && presseddown)
        {
            stuckinwall = false;
            rb.gravityScale = gravity;
        }
        if (stuckinwall)
        {
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0;
            if (pressedjump)
            {
                if (GameObject.Find("player1").GetComponent<EldonAttack>().walljumped.name=="wall_left")
                {
                    rb.velocity = new Vector2(wjforceside, wjforceup);
                    walljumpleftrdy = true;
                    walljumprightrdy = false;
                }
                else
                {
                    rb.velocity = new Vector2(-wjforceside, wjforceup);
                    walljumpleftrdy = false;
                    walljumprightrdy = true;
                }
                //rb.velocity = new Vector2(wjforceside*lasthorizontal*(-1), wjforceup);
                rb.gravityScale = gravity;
                stuckinwall = false;
                Vector3 Scale = transform.localScale;
                horizontal = -lasthorizontal;
                Scale.x *= -1;
                transform.localScale = Scale;
                if (GameObject.Find("player1").GetComponent<PlayerMovement>().facingRight)
                {
                    GameObject.Find("player1").GetComponent<PlayerMovement>().facingRight = false;
                }
                else
                {
                    GameObject.Find("player1").GetComponent<PlayerMovement>().facingRight = true;
                }
            }

        }

        //cornerjump
        Collider2D[] corner = Physics2D.OverlapCircleAll(corncheck.position, cornrange);
        foreach (Collider2D corn in corner)
        {
            if (corn.name == "corner_left"&& !grounded && walljumpleftrdy)
            {
                rb.velocity=new Vector2(+cornstrhor, cornstrver);
                walljumpleftrdy = false;
            }
            if (corn.name == "corner_right" && !grounded && walljumprightrdy)
            {
                rb.velocity=new Vector2(-1*cornstrhor, cornstrver);
                walljumprightrdy=false;
            }
        }






    }



    void Checkground()
    {
        if (grounded)
        {
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
        Gizmos.DrawCube(corncheck.position, new Vector2(cornlarg, cornhaut));
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
