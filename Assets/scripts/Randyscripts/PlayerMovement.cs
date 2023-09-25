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

    [Header("shield variables")]
    private bool grounded;
    public bool shielded; //indique si le bouclier est actif
    private int shieldmax; //correspond au bouclier maximal
    public int shield; //correspond à la valeur du bouclier
    private int shielddimrate; //correspond à la diminution passive du shield lorsqu'il est actif
    private int shieldrecharge; //correspond à la vitesse de rechargement du bouclier
    private int shieldbreakCD; //correspond au temps pendant lequel le bouclier est inactif si il est cassé
    public int shieldbreakcnter; //compteur du shieldbreak

    public Shieldbar shieldbar;
    public float shieldcancel;


    private void Awake()
    {
        controls = new PlayerControls();

        controls.gameplay.moveleft.performed += ctx => valueleft = 1;
        controls.gameplay.moveright.performed += ctx => valueright = 1;
        controls.gameplay.moveleft.canceled += ctx => valueleft = 0;
        controls.gameplay.moveright.canceled += ctx => valueright = 0;
        controls.gameplay.down.performed += ctx => vertical = 1;
        controls.gameplay.down.canceled += ctx => vertical = 0;
        controls.gameplay.shield.canceled += ctx => shieldcancel = 0;
        controls.gameplay.shield.performed += ctx => shieldcancel = 1;

        shieldmax = GameObject.Find("Global values").GetComponent<Globalvalues>().shieldmax;
        shielddimrate = GameObject.Find("Global values").GetComponent<Globalvalues>().shielddimrate;
        shieldrecharge = GameObject.Find("Global values").GetComponent<Globalvalues>().shieldrecharge;
        shieldbreakCD = GameObject.Find("Global values").GetComponent<Globalvalues>().shieldbreakCD;

    }

    private void Start()
    {
        //Define the gamobjects found on the player
        rb2D = GetComponent<Rigidbody2D>();
        myanimator = GetComponent<Animator>();
        shield = shieldmax;
        shieldbar.SetMaxshield(shieldmax);
    }

    // Handles input of the physics
    private void Update()
    {

        if (shieldcancel == 0 && shielded)
        {
            shielded = false;
            myanimator.SetBool("shield", false);
        }

        shieldbar.Setshield(shield);

        grounded = GetComponent<PlayerJumpV3>().grounded;

        if (!grounded)
        {
            myanimator.SetBool("shield", false);
        }

        if(shieldbreakcnter > 0)
        {
            shieldbreakcnter--;
        }

        if (!shielded && shieldbreakcnter <= 0 && shield < shieldmax)
        {
            shield += shieldrecharge;
        }

        if (shielded && shield > 0)
        {
            shield -= shielddimrate;
        }

        if (shielded && shield <= 0)
        {
            shield = 0;
            shieldbreakcnter = shieldbreakCD;
            shielded = false;
            myanimator.SetBool("shield", false);
        }


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
       
        if(!shielded)
        {
            if (GetComponent<PlayerJumpV3>().allowjump)
            {
                if (Mathf.Abs(rb2D.velocity.x) <= maxspeed)
                {
                    rb2D.AddForce(new Vector2(horizontal * speed, 0));
                }
                if (horizontal == 0 || (horizontal > 0 && rb2D.velocity.x < 0) || (horizontal < 0 && rb2D.velocity.x > 0))
                {
                    if (rb2D.velocity.x > 0.1)
                    {
                        rb2D.AddForce(new Vector2(-slowdownspeed, 0));
                    }
                    else if (rb2D.velocity.x < -0.1)
                    {
                        rb2D.AddForce(new Vector2(slowdownspeed, 0));
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

    void OnShield()
    {
        if (grounded && shieldbreakcnter <= 0)
        {
            shielded = true;
            myanimator.SetBool("shield", true);

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
