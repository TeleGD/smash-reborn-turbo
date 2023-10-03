using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]



public class BobbyMov : MonoBehaviour
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
    public float slowdownspeed; //vitesse de ralentissement lorsque l'input n'a vers la m�me direction que la vitesse du perso
    public float valueright;
    private float valueleft;
    public float horizontal; // 1,-1,0
    public float vertical;


    [Header("shield variables")]
    private bool grounded;
    public bool shielded; //indique si le bouclier est actif
    private int shieldmax; //correspond au bouclier maximal
    public int shield; //correspond � la valeur du bouclier
    private int shielddimrate; //correspond � la diminution passive du shield lorsqu'il est actif
    private int shieldrecharge; //correspond � la vitesse de rechargement du bouclier
    private int shieldbreakCD; //correspond au temps pendant lequel le bouclier est inactif si il est cass�
    public int shieldbreakcnter; //compteur du shieldbreak

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





    private void Awake()
    {
        //assigne les diff�rents input possible � des variables

        controls = new PlayerControls();

        controls.gameplay.moveleft1.performed += ctx => valueleft = 1;
        controls.gameplay.moveright1.performed += ctx => valueright = 1;
        controls.gameplay.moveleft1.canceled += ctx => valueleft = 0;
        controls.gameplay.moveright1.canceled += ctx => valueright = 0;
        controls.gameplay.down1.performed += ctx => vertical = -1;
        controls.gameplay.down1.canceled += ctx => vertical = 0;
        controls.gameplay.up1.performed += ctx => vertical = 1;
        controls.gameplay.up1.canceled += ctx => vertical = 0;
        controls.gameplay.shield1.canceled += ctx => shieldcancel = 0;
        controls.gameplay.shield1.performed+= ctx => shieldcancel = 1;

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
        if (vertical==-1 && grounded && !shielded)
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

        if (shieldcancel==0 && shielded) //annule le bouclier si le bouton est relach�
        {
           shielded = false;
            myanimator.SetBool("shield", false);
        }

        shieldbar.Setshield(shield); //update la barre de bouclier

        grounded = GetComponent<BobbyJump>().grounded; //check si le perso est au sol

        if(!grounded)
        {
            myanimator.SetBool("shield", false); //annule l'animation du bouclier si le perso n'est pas au sol
        }

        if (grounded && !shielded && shieldbreakcnter<=0 && shield<shieldmax) //si le shield est non actif mais qu'il n'a pas �t� shieldbreak et si le perso est au sol, le shield se fait recharger
        {
            shield += shieldrecharge;
        }

        if (shielded && shield > 0) //diminue le shield si il est activ�
        {
            shield -= shielddimrate;
        }

        if (shielded && shield<=0) //d�clenche le shieldbreak si le bouclier est actif et qu'il est vid�
        {
            shieldbreakcnter = shieldbreakCD; //initialisation du compteur qui calcule le CD du shield
            shielded = false;
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

        if(!shielded) //on ne bouge pas si le bouclier est actif
        {
            if (GetComponent<BobbyJump>().allowjump && !crouched) //allowjump est une variable qui d�termine si il est possible de se mouvoir dans les airs. Si on est baiss�, on ne peut pas bouger
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
    void OnShield1()
    {
        if(grounded && shieldbreakcnter<=0)
        {
            shielded = true;
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
