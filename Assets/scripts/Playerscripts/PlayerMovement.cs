using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]



public class PlayerMovement : MonoBehaviour
{

    PlayerControls controls;

    //safezone variables
    private float startx;
    private float starty;
    [SerializeField] private Transform groundcheck;

    //necessary for anim and physics
    private Rigidbody2D rb2D;
    private Animator myanimator;

    public bool facingRight = true;

    //modifiable variables 
    public float speed = 2.0f;
    public float maxspeed;
    public float slowdownspeed; //counterforcewhen no input
    private float valueright;
    private float valueleft;
    public float horizontal; // 1,-1,0
    public float vertical;


    private void Awake()
    {
        controls = new PlayerControls();

        controls.gameplay.moveleft.performed += ctx => valueleft = 1;
        controls.gameplay.moveright.performed += ctx => valueright = 1;
        controls.gameplay.moveleft.canceled += ctx => valueleft = 0;
        controls.gameplay.moveright.canceled += ctx => valueright = 0;
        controls.gameplay.down.performed += ctx => vertical = 1;
        controls.gameplay.down.canceled += ctx => vertical = 0;

    }

    private void Start()
    {
        //Define the gamobjects found on the player
        rb2D = GetComponent<Rigidbody2D>();
        myanimator = GetComponent<Animator>();
    }

    // Handles input of the physics
    private void Update()
    {
            //check if key pressed
            if (valueleft == 1 & valueright == 0)
            {
                horizontal = -1;
            }
            if (valueleft == 0 & valueright == 1)
            {
                horizontal = 1;
            }
            if (valueleft == 0 & valueright == 0)
            {
                horizontal = 0;
            }
            if (valueleft == 1 & valueright == 1)
            {
                horizontal = 0;
            }

    }
    //Handles running of the physics
    private void FixedUpdate()
    {
        //move player
       
       if (GetComponent<PlayerJumpV3>().allowjump)
        {
            if (Mathf.Abs(rb2D.velocity.x) <= maxspeed)
                {
                rb2D.AddForce(new Vector2(horizontal * speed,0));
            }
            if ((horizontal ==0 && Mathf.Abs(rb2D.velocity.x) > 0.01) || (horizontal>0 && rb2D.velocity.x<0) ||( horizontal<0 && rb2D.velocity.x>0))
            {
                if (rb2D.velocity.x > 0)
                {
                    rb2D.AddForce(new Vector2(-slowdownspeed,0));
                }
                else
                {
                    rb2D.AddForce(new Vector2(slowdownspeed,0));
                }
            }
           
            Flip(horizontal);
            myanimator.SetFloat("speed", Mathf.Abs(horizontal));
        }
        else
        {
            if (GetComponent<PlayerJumpV3>().grounded && horizontal != 0)
            {
                Flip(horizontal);
            }
        }
       
    }
    //flipping function
    private void Flip(float horizontal)
    {
        if (horizontal < 0 && facingRight || horizontal>0 && !facingRight)
        {
            facingRight = !facingRight;

            Vector3 Scale = transform.localScale;
            Scale.x *= -1;
            transform.localScale = Scale;

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
