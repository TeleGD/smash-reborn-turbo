using System.Collections;
using System.Collections.Generic;
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
    public float slowdownspeed; //vitesse de ralentissement lorsque l'input n'a vers la m�me direction que la vitesse du perso
    public float valueright;
    private float valueleft;
    public float horizontal; // 1,-1,0
    public float vertical;
    private float quickfallspeed; //va aller chercher la vitesse de quickfall dans globalvalues


    [Header("shield variables")]
    //public int shielded: a �t� d�plac� dans Globalvalues pour faciliter l'acc�s et faciliter l'utilisation de plusieurs persos
    private int shieldmax; //correspond au bouclier maximal
    //public int shield; correspond � la valeur du bouclier, pass� dans Globalvalues pour les m�mes raisons
    private int shielddimrate; //correspond � la diminution passive du shield lorsqu'il est actif
    private int shieldrecharge; //correspond � la vitesse de rechargement du bouclier
    private int shieldbreakCD; //correspond au temps pendant lequel le bouclier est inactif si il est cass�
    private int shieldbreakcnter; //compteur du shieldbreak

    public Shieldbar shieldbar;
    public float shieldcancel;

    [Header("crouch variables")]
    public bool crouched; //bool servant � savoir si le perso est baiss� ou pas
    //servent � d�terminer la taille et la position de la hitbox une fois baiss�
    public float crouchedhbx;
    public float crouchedhby;
    public float crouchedhboffsety;
    //servent � d�terminer la taille de la hitbox une fois debout
    public float standinghbx;
    public float standinghby;
    public float standinghboffsety;

    [Header("Ground details")]
    //ensemble de variables qui servent � d�tecter le sol
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float radOcircle;
    [SerializeField] private float hauteurgi;
    [SerializeField] private float largeurgi;
    [SerializeField] private LayerMask whatisground;
    public bool grounded;



    private bool grabed; //sert � d�terminer si un perso est grab. Ce bool est r�cup�r� du script charavalues


    private void Awake()
    {
        //assigne les diff�rents input possible � des variables

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
    private void Update()
    {
        grabed = GetComponent<charavalues>().grabed;
        grounded = Physics2D.OverlapCircle(groundcheck.position, radOcircle, whatisground);


        if (shieldcancel == 1)
        {
            ShieldInput();
        }


        if(!grounded && vertical==-1 && rb2D.velocity.y<0 && !grabed) //correspond au quickfall. On check que la vitesse en y est n�gative, car �a veut dire qu'on est d�ja en train de tomber.
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x,-quickfallspeed) ;
        }


        if (vertical==-1 && grounded && !GetComponent<charavalues>().shielded && !grabed)
        {
            myanimator.SetBool("crouch", true);
            crouched = true;
            GetComponent<BoxCollider2D>().size = new Vector2(crouchedhbx, crouchedhby); //change la taille de la hitbox du perso lorsqu'il est baiss�
            GetComponent<BoxCollider2D>().offset = new Vector2(GetComponent<BoxCollider2D>().offset.x, crouchedhboffsety); //change la position de la hitbox du perso lorsqu'il est baiss�
        }
        else //r�tablit la hitbox du perso si il n'est pas baiss�
        {
            myanimator.SetBool("crouch", false);
            crouched = false;
            GetComponent<BoxCollider2D>().size = new Vector2(standinghbx, standinghby);
            GetComponent<BoxCollider2D>().offset = new Vector2(GetComponent<BoxCollider2D>().offset.x, standinghboffsety);
        }

        if (shieldcancel==0 && GetComponent<charavalues>().shielded) //annule le bouclier si le bouton est relach�
        {
           GetComponent<charavalues>().shielded = false;
            myanimator.SetBool("shield", false);
        }

        shieldbar.Setshield(GetComponent<charavalues>().shield); //update la barre de bouclier


        if(!grounded || grabed)
        {
            myanimator.SetBool("shield", false); //annule l'animation du bouclier si le perso n'est pas au sol
        }

        if (grounded && !GetComponent<charavalues>().shielded && shieldbreakcnter<=0 && GetComponent<charavalues>().shield < shieldmax) //si le shield est non actif mais qu'il n'a pas �t� shieldbreak et si le perso est au sol, le shield se fait recharger
        {
            GetComponent<charavalues>().shield += shieldrecharge;
        }

        if (GetComponent<charavalues>().shielded && GetComponent<charavalues>().shield > 0) //diminue le shield si il est activ�
        {
            GetComponent<charavalues>().shield -= shielddimrate;
        }

        if (GetComponent<charavalues>().shielded && GetComponent<charavalues>().shield <= 0) //d�clenche le shieldbreak si le bouclier est actif et qu'il est vid�
        {
            shieldbreakcnter = shieldbreakCD; //initialisation du compteur qui calcule le CD du shield
            GetComponent<charavalues>().shielded = false;
            myanimator.SetBool("shield", false);
        }

        if(shieldbreakcnter>0) //diminution du compteur qui calcule le CD du shield
        {
            shieldbreakcnter = shieldbreakcnter - 1;
        }
        

        //d�termine la variable horizontal qui sert � savoir la direction souhait�e en fonction des diff�rentes inputs
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
    private void FixedUpdate()
    {
        //Section des mouvements

        if(!GetComponent<charavalues>().shielded) //on ne bouge pas si le bouclier est actif
        {
            if (!crouched && !grabed) //Si on est baiss� ou qu'on s'est fait grab, on ne peut pas bouger
            {
                if (Mathf.Abs(rb2D.velocity.x) <= maxspeed) //check si la vitesse est inf�rieure � la vitesse max et on acc�l�re. 
                {
                    rb2D.AddForce(new Vector2(horizontal * speed, 0));
                }
                if (horizontal==0 || (horizontal > 0 && rb2D.velocity.x < 0) || (horizontal < 0 && rb2D.velocity.x > 0))
                {
                    if (rb2D.velocity.x > 0.1)
                    {
                        rb2D.AddForce(new Vector2(-slowdownspeed, 0));
                    }
                    else if(rb2D.velocity.x < -0.1)
                    {
                        rb2D.AddForce(new Vector2(slowdownspeed, 0));
                    }
                }

                Flip(horizontal);
                myanimator.SetFloat("speed", Mathf.Abs(rb2D.velocity.x));
            }
            else
            {
                if (!grounded && horizontal != 0 && !crouched)
                {
                    Flip(horizontal); //retourne le personnage si besoin est.
                }
            }
        }

       

    }
    //permet de retourner la sprite du personnage
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


    //fonction qui se d�clenche lorsqu'on appuye sur le bouton de shield
    void ShieldInput()
    {
        if(grounded && shieldbreakcnter<=0 && !grabed)
        {
            GetComponent<charavalues>().shielded = true;
            myanimator.SetBool("shield", true);

        }
    }

    //g�re les contr�les
    void OnEnable()
    {
        controls.gameplay.Enable();
    }
    void OnDisable()
    {
        controls.gameplay.Disable();
    }
}
