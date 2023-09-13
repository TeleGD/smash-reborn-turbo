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
    public float safedetectrange;
    [SerializeField] private LayerMask whatissafe;
    [SerializeField] private Transform groundcheck;
    public GameObject[] deadenemy;
    public bool issafe;

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
        if (!GameObject.Find("player1").GetComponent<PlayerJumpV3>().stuckinwall)
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
            //vertical = Input.GetAxis("Vertical");
        }

    }
    //Handles running of the physics
    private void FixedUpdate()
    {
        //move player
       // if (GameObject.Find("player").GetComponent<PlayerJumpV2>().allowjump && !GameObject.Find("player").GetComponent<PlayerJumpV2>().wallslidingleft && !GameObject.Find("player").GetComponent<PlayerJumpV2>().wallslidingright && GameObject.Find("player").GetComponent<PlayerJumpV2>().movecounter<=0)
       if (GameObject.Find("player1").GetComponent<PlayerJumpV3>().allowjump && !GetComponent<PlayerJumpV3>().stuckinwall)
        {
            //rb2D.velocity = new Vector2(horizontal * speed, rb2D.velocity.y);
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
            // if (!GameObject.Find("player").GetComponent<PlayerJumpV2>().grounded && horizontal!=0)
            if (!GameObject.Find("player1").GetComponent<PlayerJumpV3>().grounded && horizontal != 0 && !GetComponent<PlayerJumpV3>().stuckinwall)
            {
                Flip(horizontal);
            }
        }
        issafe = Physics2D.OverlapCircle(groundcheck.position, safedetectrange, whatissafe);
        //if (GameObject.Find("player").GetComponent<PlayerJumpV2>().grounded & Physics2D.OverlapCircle(groundcheck.position, safedetectrange, whatissafe) & vertical==1)
        if (GameObject.Find("player1").GetComponent<PlayerJumpV3>().grounded && Physics2D.OverlapCircle(groundcheck.position, safedetectrange, whatissafe) && vertical == 1)
        {
            safezone();
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
    void safezone()
    {
        deadenemy = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in deadenemy)
        {
            if (enemy.tag == "enemy")
            {
                enemy.GetComponent<EnemyHP>().enemyhp = enemy.GetComponent<EnemyHP>().enemymaxhp;
                enemy.GetComponent<EnemyHP>().enemyNRG = enemy.GetComponent<EnemyHP>().enemymaxNRG;
                enemy.GetComponentInChildren<Canvas>().enabled = true;
                enemy.GetComponent<Collider2D>().enabled = true;
                enemy.GetComponent<SpriteRenderer>().enabled = true;
                enemy.GetComponent<EnemyHP>().enabled = true;
                enemy.GetComponent<EnemyHP>().rez = true;
                enemy.GetComponent<EnemyAI>().targetted = false;
                enemy.GetComponent<EnemyHP>().execution = false;
                enemy.GetComponent<Animator>().SetBool("Stun", false);
            }
        }
        GameObject.Find("player1").GetComponent<PlayerHP>().Eldonhp = GameObject.Find("player1").GetComponent<PlayerHP>().Eldonmaxhp;
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
