using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]



public class EnMovement : MonoBehaviour
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
    public float valueright;
    private float valueleft;
    public float horizontal; // 1,-1,0
    public float vertical;


    [Header("shield variables")]
    private bool grounded;
    public bool shielded; //indique si le bouclier est actif
    public int shieldmax; //correspond au bouclier maximal
    public int shield; //correspond � la valeur du bouclier
    public int shielddimrate; //correspond � la diminution passive du shield lorsqu'il est actif
    public int shieldrecharge; //correspond � la vitesse de rechargement du bouclier
    public int shieldbreakCD; //correspond au temps pendant lequel le bouclier est inactif si il est cass�
    public int shieldbreakcnter; //compteur du shieldbreak

    public Shieldbar shieldbar;
    public float shieldcancel;




    private void Awake()
    {
        controls = new PlayerControls();

        controls.gameplay.moveleft1.performed += ctx => valueleft = 1;
        controls.gameplay.moveright1.performed += ctx => valueright = 1;
        controls.gameplay.moveleft1.canceled += ctx => valueleft = 0;
        controls.gameplay.moveright1.canceled += ctx => valueright = 0;
        controls.gameplay.down1.performed += ctx => vertical = 1;
        controls.gameplay.down1.canceled += ctx => vertical = 0;
        controls.gameplay.shield1.canceled += ctx => shieldcancel = 0;
        controls.gameplay.shield1.performed+= ctx => shieldcancel = 1;
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

        if(shieldcancel==0 && shielded)
        {
           shielded = false;
            myanimator.SetBool("shield", false);
        }

        shieldbar.Setshield(shield);

        grounded = GetComponent<EnJumpV3>().grounded;

        if(!grounded)
        {
            myanimator.SetBool("shield", false);
        }

        if (grounded && !shielded && shieldbreakcnter<=0 && shield<shieldmax)
        {
            shield += shieldrecharge;
        }

        if (shielded && shield > 0)
        {
            shield -= shielddimrate;
        }

        if (shielded && shield<=0)
        {
            shieldbreakcnter = shieldbreakCD;
            shielded = false;
            myanimator.SetBool("shield", false);
        }

        if(shieldbreakcnter>0)
        {
            shieldbreakcnter = shieldbreakcnter - 1;
        }
        

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
            if (GetComponent<EnJumpV3>().allowjump)
            {
                if (Mathf.Abs(rb2D.velocity.x) <= maxspeed)
                {
                    rb2D.AddForce(new Vector2(horizontal * speed, 0));
                }
                if ((horizontal == 0 && Mathf.Abs(rb2D.velocity.x) > 0.01) || (horizontal > 0 && rb2D.velocity.x < 0) || (horizontal < 0 && rb2D.velocity.x > 0))
                {
                    if (rb2D.velocity.x > 0)
                    {
                        rb2D.AddForce(new Vector2(-slowdownspeed, 0));
                    }
                    else
                    {
                        rb2D.AddForce(new Vector2(slowdownspeed, 0));
                    }
                }

                Flip(horizontal);
                myanimator.SetFloat("speed", Mathf.Abs(rb2D.velocity.x));
            }
            else
            {
                if (!GetComponent<EnJumpV3>().grounded && horizontal != 0)
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

    

    void OnShield1()
    {
        if(grounded && shieldbreakcnter<=0)
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
