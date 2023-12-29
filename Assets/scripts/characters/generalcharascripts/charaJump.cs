using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]


public class charaJump : MonoBehaviour
{
    PlayerControls controls;

    [Header("Jump details")]
    public int maxnumberofjumps; //number of jumps
    private int numberjumped; //compte le nombre de sauts que le perso a fait
    public float jumpForce; //intensit� du saut
    public float jumptime; //dur�e du saut
    private float jumpcounter; //compteur qui compte la dur�e du saut
    public bool allowjump; //bool qui g�re quand le saut est possible

    [Header("DoubleJump details")]
    public bool allowdoublejump; //bool qui g�re quand le double saut est possible
    public bool touchedground; //bool qui g�re si le sol � �t� touch� apr�s un double saut
    public float dbjumpForce; //intensit� du double saut
    public float dbjumptime; //dur�e du double saut
    private float dbjumpcounter; //compteur qui compte la dur�e du double saut
    public float dbjumpdelay; //dur�e minimale entre un saut et un double saut
    private float dbjumpdelaycounter; //compteur qui compte la dur�e minimale entre un saut et un double saut
    public bool jumplache; //bool qui d�termine si le bouton de saut est press� ou pas


    public bool grounded; //bool qui dit si le perso est au sol ou pas

    [Header("Platform")]
    public bool platformed;
    public int platdowntime;
    public int platdowncnt;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator myanim;
    public float horizontal;

    //les deux bools suivants servent � d�tecter les inputs du bouton de saut
    public bool pressedjump = false;
    public bool presseddown = false;

    private bool grabed; //sert � d�terminer si un perso est grab. Ce bool est r�cup�r� du script charavalues
    private int hitstun;

    private bool grabbing;


    private void Awake()
    {
        //assigne les diff�rents input possible � des variables
        controls = new PlayerControls();

        if (this.CompareTag("Player1"))
        {
            controls.gameplay.jump.performed += ctx => pressedjump = true;
            controls.gameplay.jump.canceled += ctx => pressedjump = false;
            controls.gameplay.down.performed += ctx => presseddown = true;
            controls.gameplay.down.canceled += ctx => presseddown = false;
        }
        else if(this.CompareTag("Player2"))
        {
            controls.gameplay.jump1.performed += ctx => pressedjump = true;
            controls.gameplay.jump1.canceled += ctx => pressedjump = false;
            controls.gameplay.down1.performed += ctx => presseddown = true;
            controls.gameplay.down1.canceled += ctx => presseddown = false;
        }

        //initialisation de variables
        rb = GetComponent<Rigidbody2D>();
        jumpcounter = jumptime;
        dbjumpdelaycounter = dbjumpdelay;
        dbjumpcounter = dbjumptime;
        myanim = GetComponent<Animator>();

    }

    private void FixedUpdate()
    {

        grabbing = GetComponent<charavalues>().grabbing;

        hitstun = GetComponent<charavalues>().hitstuncnt;

        grabed = GetComponent<charavalues>().grabed;

        if (platdowncnt>0)
        {
            platdowncnt -= 1;

        }

        HandleLayers();


        platformed = GetComponent<Charamov>().platformed;

        if (platformed && !presseddown && !pressedjump && platdowncnt==0 && !GetComponent<charavalues>().upb)
        {
            if (horizontal == 0)
            {
                rb.velocity=new Vector2(0,0);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
           rb.gravityScale = 0;
        }
        else if(presseddown && platformed && !grabed && hitstun<=0)
        {
            platdowncnt = platdowntime;
            platformed = false;
        }
        if (!platformed)
        {
            rb.gravityScale = 1;
        }


        horizontal = GetComponent<Charamov>().horizontal; //r�cup�re la variable du script Charamov
        grounded = GetComponent<Charamov>().grounded;  
        Checkground();

        //normal jump

        if (pressedjump && grounded && !GetComponent<charavalues>().shielded && !grabed && hitstun <= 0) //si on est sur le sol, que le bouclier est d�sactiv� et qu'on appuie sur le bouton de saut, on change l'animation et on applique la vitesse verticale du saut
        {
            myanim.SetTrigger("jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
           myanim.SetBool("falling", true); //le bool de chute est toujours actif quand le perso est en l'air.
        }
            

        if (jumpcounter <= 0) //check si le saut est termin�
        {
            dbjumpdelaycounter -= Time.deltaTime; //diminue le compteur de d�lai de double jump
            if (dbjumpdelaycounter <= 0) //si le compteur est nul, alors le double jump devient possible
            {
                allowdoublejump = true;
            }

        }
        if (!pressedjump && !grounded) //check si le bouton de saut est relach� alors que le perso ne touche pas le sol. Permet d'�viter que le double saut se fasse tout seul si le joueur garde le bouton de saut appuy�.
        {
            jumplache = true;
            jumpcounter = 0;
            dbjumpcounter = 0;
            myanim.SetBool("falling", true);
        }
        if (rb.velocity.y < 0)
        {
            myanim.SetBool("falling", true);
        }


        //double jump


        if (pressedjump && !grounded && allowdoublejump && touchedground && jumplache && !grabed && !grabbing && hitstun <= 0 && numberjumped<=maxnumberofjumps) //v�rifie que toutes les requirements pour lancer un double saut sont v�rifi�s
        {

            myanim.SetTrigger("jump");
            rb.velocity = new Vector2(rb.velocity.x, dbjumpForce);
            myanim.SetBool("falling", true);
            touchedground = false;
            dbjumpdelaycounter = dbjumpdelay;
            numberjumped += 1;
        }

        if (!grounded && pressedjump && dbjumpcounter > 0 && allowdoublejump && jumplache) //continue de gagner de la hauteur si le bouton de saut est maintenu.
        {
            UnityEngine.Debug.Log("doublejump2");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            dbjumpcounter -= Time.deltaTime;
            myanim.SetTrigger("jump");
            myanim.ResetTrigger("jump");
            myanim.SetBool("falling", true);

            touchedground = false;
        }
        if(dbjumpdelaycounter<=0)
        {

            allowdoublejump = false;
            if (numberjumped < maxnumberofjumps)
            {
                touchedground = true;
            }
        }



    }



    void Checkground()
        //v�rifie si le perso est sur le sol et change toutes les variables qui doivent l'�tre
    {

        if(platformed)
        {
            grounded = true;
        }

        if (grounded)
        {
            numberjumped = 0;
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


    private void HandleLayers()
        //permet � l'animateur de changer entre la layer correspondant � l'air et celle correspondant au sol
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
