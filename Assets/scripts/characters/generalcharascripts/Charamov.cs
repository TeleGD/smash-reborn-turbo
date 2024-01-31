using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]



public class Charamov : MonoBehaviour
{

    PlayerControls controls;

    //safezone variables
    private float startx;
    private float starty;

    //necessary for anim and physics
    private Rigidbody2D rb2D;
    private Animator myanimator;

    public bool facingRight = true;

    //modifiable variables 
    public float speed = 2.0f;
    public float maxspeed;
    public float slowdownspeed; //vitesse de ralentissement lorsque l'input n'a vers la même direction que la vitesse du perso
    public float valueright;
    private float valueleft;
    public float horizontal; // 1,-1,0
    public float vertical;
    private float quickfallspeed; //va aller chercher la vitesse de quickfall dans globalvalues


    [Header("shield variables")]
    //public int shielded: a été déplacé dans Globalvalues pour faciliter l'accès et faciliter l'utilisation de plusieurs persos
    private int shieldmax; //correspond au bouclier maximal
    //public int shield; correspond à la valeur du bouclier, passé dans Globalvalues pour les mêmes raisons
    private int shielddimrate; //correspond à la diminution passive du shield lorsqu'il est actif
    private int shieldrecharge; //correspond à la vitesse de rechargement du bouclier
    private int shieldbreakCD; //correspond au temps pendant lequel le bouclier est inactif si il est cassé
    private int shieldbreakcnter; //compteur du shieldbreak

    public Shieldbar shieldbar;
    public float shieldcancel;

    [Header("crouch variables")]
    public bool crouched; //bool servant à savoir si le perso est baissé ou pas
    //servent à déterminer la taille et la position de la hitbox une fois baissé
    public float crouchedhbx;
    public float crouchedhby;
    public float crouchedhboffsety;
    //servent à déterminer la taille de la hitbox une fois debout
    public float standinghbx;
    public float standinghby;
    public float standinghboffsety;

    [Header("Ground details")]
    //ensemble de variables qui servent à détecter le sol
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float radOcircle;
    [SerializeField] private float hauteurgi;
    [SerializeField] private float largeurgi;
    [SerializeField] private LayerMask whatisground;
    [SerializeField] private LayerMask whatisplatform;
    public bool platformed;

    public bool overaplayer;

    public float groundchecksizey;

    public float groundcheckoffsety;

    public bool grounded;

    public float movewhenover;
    private bool overenemy;

    public bool attacking;

    private int hitstun;

    private bool grabed; //sert à déterminer si un perso est grab. Ce bool est récupéré du script charavalues

    private bool grabbing; 


    private void Awake()
    {
        //assigne les différents input possible à des variables

        controls = new PlayerControls();

        if (this.CompareTag("Player1"))
        {
            controls.gameplay.moveleft.performed += ctx => valueleft = 1;
            controls.gameplay.moveright.performed += ctx => valueright = 1;
            controls.gameplay.moveleft.canceled += ctx => valueleft = 0;
            controls.gameplay.moveright.canceled += ctx => valueright = 0;
            controls.gameplay.down.performed += ctx => vertical = -1;
            controls.gameplay.down.canceled += ctx => vertical = 0;
            controls.gameplay.up.performed += ctx => vertical = 1;
            controls.gameplay.up.canceled += ctx => vertical = 0;
            controls.gameplay.shield.canceled += ctx => shieldcancel = 0;
            controls.gameplay.shield.performed += ctx => shieldcancel = 1;
        }
        else
        {
            controls.gameplay.moveleft1.performed += ctx => valueleft = 1;
            controls.gameplay.moveright1.performed += ctx => valueright = 1;
            controls.gameplay.moveleft1.canceled += ctx => valueleft = 0;
            controls.gameplay.moveright1.canceled += ctx => valueright = 0;
            controls.gameplay.down1.performed += ctx => vertical = -1;
            controls.gameplay.down1.canceled += ctx => vertical = 0;
            controls.gameplay.up1.performed += ctx => vertical = 1;
            controls.gameplay.up1.canceled += ctx => vertical = 0;
            controls.gameplay.shield1.canceled += ctx => shieldcancel = 0;
            controls.gameplay.shield1.performed += ctx => shieldcancel = 1;
        }

        

        shieldmax = GameObject.Find("Global values").GetComponent<Globalvalues>().shieldmax;
        shielddimrate = GameObject.Find("Global values").GetComponent<Globalvalues>().shielddimrate;
        shieldrecharge = GameObject.Find("Global values").GetComponent<Globalvalues>().shieldrecharge;
        shieldbreakCD = GameObject.Find("Global values").GetComponent<Globalvalues>().shieldbreakCD;
        quickfallspeed = GameObject.Find("Global values").GetComponent<Globalvalues>().quickfallspeed;
        movewhenover = GameObject.Find("Global values").GetComponent<Globalvalues>().speedwhenoverplayer;
    }

    private void Start()
    {


        //Define the gamobjects found on the player
        rb2D = GetComponent<Rigidbody2D>();
        myanimator = GetComponent<Animator>();
        GetComponent<charavalues>().shield = shieldmax; 
        shieldbar.SetMaxshield(shieldmax);


        
    }

    // Handles input of the physics
    private void FixedUpdate()
    {

        //if(GetComponent<charavalues>().initmov)
        //{

        //    GetComponent<charavalues>().initmov = false;

        //    //assigne les différents input possible à des variables

        //    controls = new PlayerControls();

        //    if (this.CompareTag("Player1"))
        //    {
        //        controls.gameplay.moveleft.performed += ctx => valueleft = 1;
        //        controls.gameplay.moveright.performed += ctx => valueright = 1;
        //        controls.gameplay.moveleft.canceled += ctx => valueleft = 0;
        //        controls.gameplay.moveright.canceled += ctx => valueright = 0;
        //        controls.gameplay.down.performed += ctx => vertical = -1;
        //        controls.gameplay.down.canceled += ctx => vertical = 0;
        //        controls.gameplay.up.performed += ctx => vertical = 1;
        //        controls.gameplay.up.canceled += ctx => vertical = 0;
        //        controls.gameplay.shield.canceled += ctx => shieldcancel = 0;
        //        controls.gameplay.shield.performed += ctx => shieldcancel = 1;
        //    }
        //    else
        //    {
        //        controls.gameplay.moveleft1.performed += ctx => valueleft = 1;
        //        controls.gameplay.moveright1.performed += ctx => valueright = 1;
        //        controls.gameplay.moveleft1.canceled += ctx => valueleft = 0;
        //        controls.gameplay.moveright1.canceled += ctx => valueright = 0;
        //        controls.gameplay.down1.performed += ctx => vertical = -1;
        //        controls.gameplay.down1.canceled += ctx => vertical = 0;
        //        controls.gameplay.up1.performed += ctx => vertical = 1;
        //        controls.gameplay.up1.canceled += ctx => vertical = 0;
        //        controls.gameplay.shield1.canceled += ctx => shieldcancel = 0;
        //        controls.gameplay.shield1.performed += ctx => shieldcancel = 1;
        //    }



        //    shieldmax = GameObject.Find("Global values").GetComponent<Globalvalues>().shieldmax;
        //    shielddimrate = GameObject.Find("Global values").GetComponent<Globalvalues>().shielddimrate;
        //    shieldrecharge = GameObject.Find("Global values").GetComponent<Globalvalues>().shieldrecharge;
        //    shieldbreakCD = GameObject.Find("Global values").GetComponent<Globalvalues>().shieldbreakCD;
        //    quickfallspeed = GameObject.Find("Global values").GetComponent<Globalvalues>().quickfallspeed;
        //    movewhenover = GameObject.Find("Global values").GetComponent<Globalvalues>().speedwhenoverplayer;
        //    //Define the gamobjects found on the player
        //    rb2D = GetComponent<Rigidbody2D>();
        //    myanimator = GetComponent<Animator>();
        //    GetComponent<charavalues>().shield = shieldmax;
        //    shieldbar.SetMaxshield(shieldmax);
        //}


        //Les trois lignes en dessous vont chercher les variables dans le script charavalues.
        hitstun= GetComponent<charavalues>().hitstuncnt;
        grabed = GetComponent<charavalues>().grabed;
        grabbing = GetComponent<charavalues>().grabbing;
        //Les deux ligens en dessous permettent de détecter le sol et les platformes.
        grounded = Physics2D.OverlapBox(new Vector2(rb2D.position.x, (rb2D.position.y - GetComponent<BoxCollider2D>().size.y / 2) - groundcheckoffsety), new Vector2(GetComponent<BoxCollider2D>().size.x, groundchecksizey),0, whatisground);
        platformed = Physics2D.OverlapBox(new Vector2(rb2D.position.x, (rb2D.position.y - GetComponent<BoxCollider2D>().size.y / 2) - groundcheckoffsety), new Vector2(GetComponent<BoxCollider2D>().size.x, groundchecksizey), 0, whatisplatform);
        overenemy = false;
        attacking=GetComponent<charavalues>().attacking;

        Collider2D[] enemiesunder = Physics2D.OverlapBoxAll(new Vector2(rb2D.position.x, (rb2D.position.y - GetComponent<BoxCollider2D>().size.y / 2) - groundcheckoffsety), new Vector2(GetComponent<BoxCollider2D>().size.x, groundchecksizey),0);

        foreach (Collider2D enemy in enemiesunder)
        {
            if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && !grounded &&!attacking)
            {
                overenemy = true;
                if(enemy.GetComponent<Rigidbody2D>().position.x >= rb2D.position.x)
                {
                    rb2D.velocity = new Vector2(rb2D.velocity.x - movewhenover, rb2D.velocity.y);
                }
                else
                {
                    rb2D.velocity = new Vector2(rb2D.velocity.x + movewhenover, rb2D.velocity.y);
                }
            }
        }


            if (shieldcancel == 1)
        {
            ShieldInput();
        }

        

        if(!(grounded || platformed) && vertical==-1 && rb2D.velocity.y<0 && !grabed && !grabbing) //correspond au quickfall. On check que la vitesse en y est négative, car ça veut dire qu'on est déja en train de tomber.
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x,-quickfallspeed) ;
        }


        if (vertical==-1 && (grounded || platformed) && !GetComponent<charavalues>().shielded && !grabed && !grabbing && hitstun<=0)
        {
            myanimator.SetBool("crouch", true);
            crouched = true;
            //GetComponent<BoxCollider2D>().size = new Vector2(crouchedhbx, crouchedhby); //change la taille de la hitbox du perso lorsqu'il est baissé
            //GetComponent<BoxCollider2D>().offset = new Vector2(GetComponent<BoxCollider2D>().offset.x, crouchedhboffsety); //change la position de la hitbox du perso lorsqu'il est baissé
        }
        else //rétablit la hitbox du perso si il n'est pas baissé
        {
            myanimator.SetBool("crouch", false);
            crouched = false;
            GetComponent<BoxCollider2D>().size = new Vector2(standinghbx, standinghby);
            GetComponent<BoxCollider2D>().offset = new Vector2(GetComponent<BoxCollider2D>().offset.x, standinghboffsety);
        }

        if (shieldcancel==0 && GetComponent<charavalues>().shielded || GetComponent<charavalues>().attacking) //annule le bouclier si le bouton est relaché
        {
           GetComponent<charavalues>().shielded = false;
            myanimator.SetBool("shield", false);
        }

        shieldbar.Setshield(GetComponent<charavalues>().shield); //update la barre de bouclier


        if(!(grounded || platformed) || grabed)
        {
            myanimator.SetBool("shield", false); //annule l'animation du bouclier si le perso n'est pas au sol
        }

        if ((grounded || platformed) && !GetComponent<charavalues>().shielded && shieldbreakcnter<=0 && GetComponent<charavalues>().shield < shieldmax) //si le shield est non actif mais qu'il n'a pas été shieldbreak et si le perso est au sol, le shield se fait recharger
        {
            GetComponent<charavalues>().shield += shieldrecharge;
        }

        if (GetComponent<charavalues>().shielded && GetComponent<charavalues>().shield > 0 && !GetComponent<charavalues>().attacking) //diminue le shield si il est activé
        {
            GetComponent<charavalues>().shield -= shielddimrate;
        }

        if (GetComponent<charavalues>().shielded && GetComponent<charavalues>().shield <= 0) //déclenche le shieldbreak si le bouclier est actif et qu'il est vidé
        {
            shieldbreakcnter = shieldbreakCD; //initialisation du compteur qui calcule le CD du shield
            GetComponent<charavalues>().shielded = false;
            myanimator.SetBool("shield", false);
        }

        if(shieldbreakcnter>0) //diminution du compteur qui calcule le CD du shield
        {
            shieldbreakcnter = shieldbreakcnter - 1;
        }
        

        //détermine la variable horizontal qui sert à savoir la direction souhaitée en fonction des différentes inputs
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

        //Section des mouvements

        if (!GetComponent<charavalues>().shielded && !grabed && !attacking && hitstun <= 0 && !grabbing && !overenemy) //on ne bouge pas si le bouclier est actif, si on attaque, si on est grab ou si on est sur un enemi
        {
            if (!crouched) //Si on est baissé ou qu'on s'est fait grab, on ne peut pas bouger
            {
                if (Mathf.Abs(rb2D.velocity.x) <= maxspeed) //check si la vitesse est inférieure à la vitesse max et on accélère. 
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
                myanimator.SetFloat("speed", Mathf.Abs(rb2D.velocity.x));
            }
            else
            {
                if (!(grounded || platformed) && horizontal != 0 && !crouched)
                {
                    Flip(horizontal); //retourne le personnage si besoin est.
                }
            }
        }

    }

    //permet de retourner la sprite du personnage
    public void Flip(float horizontal) 
    {
        if (horizontal < 0 && facingRight || horizontal>0 && !facingRight)
        {
            facingRight = !facingRight;

            Vector3 Scale = transform.localScale;
            Scale.x *= -1;
            transform.localScale = Scale;

        }
    }


    //fonction qui se déclenche lorsqu'on appuye sur le bouton de shield
    void ShieldInput()
    {
        if(((grounded || platformed) || GetComponent<charaJump>().platformed) && shieldbreakcnter<=0 && !grabed && hitstun<=0 && !GetComponent<charavalues>().attacking && !grabbing)
        {
            GetComponent<charavalues>().shielded = true;
            myanimator.SetBool("shield", true);

        }
    }

    //gère les contrôles
    void OnEnable()
    {
        controls.gameplay.Enable();
    }
    void OnDisable()
    {
        controls.gameplay.Disable();
    }

}


