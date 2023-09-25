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
    public float slowdownspeed; //vitesse de ralentissement lorsque l'input n'a vers la même direction que la vitesse du perso
    public float valueright;
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
        //assigne les différents input possible à des variables

        controls = new PlayerControls();

        controls.gameplay.moveleft1.performed += ctx => valueleft = 1;
        controls.gameplay.moveright1.performed += ctx => valueright = 1;
        controls.gameplay.moveleft1.canceled += ctx => valueleft = 0;
        controls.gameplay.moveright1.canceled += ctx => valueright = 0;
        controls.gameplay.down1.performed += ctx => vertical = 1;
        controls.gameplay.down1.canceled += ctx => vertical = 0;
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

        if(shieldcancel==0 && shielded) //annule le bouclier si le bouton est relaché
        {
           shielded = false;
            myanimator.SetBool("shield", false);
        }

        shieldbar.Setshield(shield); //update la barre de bouclier

        grounded = GetComponent<EnJumpV3>().grounded; //check si le perso est au sol

        if(!grounded)
        {
            myanimator.SetBool("shield", false); //annule l'animation du bouclier si le perso n'est pas au sol
        }

        if (grounded && !shielded && shieldbreakcnter<=0 && shield<shieldmax) //si le shield est non actif mais qu'il n'a pas été shieldbreak et si le perso est au sol, le shield se fait recharger
        {
            shield += shieldrecharge;
        }

        if (shielded && shield > 0) //diminue le shield si il est activé
        {
            shield -= shielddimrate;
        }

        if (shielded && shield<=0) //déclenche le shieldbreak si le bouclier est actif et qu'il est vidé
        {
            shieldbreakcnter = shieldbreakCD; //initialisation du compteur qui calcule le CD du shield
            shielded = false;
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

    }
    private void FixedUpdate()
    {
        //Section des mouvements

        if(!shielded) //on ne saute pas si le bouclier est actif
        {
            if (GetComponent<EnJumpV3>().allowjump) //allowjump est une variable qui détermine si il est possible de se mouvoir dans les airs.
            {
                if (Mathf.Abs(rb2D.velocity.x) <= maxspeed) //check si la vitesse est inférieure à la vitesse max et dans ce cas gère les ralentissement
                {
                    rb2D.AddForce(new Vector2(horizontal * speed, 0));
                }
                if ((horizontal == 0 && Mathf.Abs(rb2D.velocity.x) > 0.05) || (horizontal > 0 && rb2D.velocity.x < 0) || (horizontal < 0 && rb2D.velocity.x > 0))
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
                if (!grounded && horizontal != 0)
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


    //fonction qui se déclenche lorsqu'on appuye sur le bouton de shield
    void OnShield1()
    {
        if(grounded && shieldbreakcnter<=0)
        {
            shielded = true;
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
