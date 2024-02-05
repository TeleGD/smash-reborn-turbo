using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Animator))]
public class BobbyAtk : MonoBehaviour
{
    PlayerControls controls;

    public PlayerInput Attack;

    private Animator anim;



    [Header("Character variables")] //Ces variables sont les variablesqui vont être modifiées lorsque l'ennemi va être attaqué. Les trois premières ont été regroupé dans un script qui sera le seul présent dans tous les personnages.
    public int playernumber;
    public int enemynumber;
    public int tempperc = 0; //cette variable et la suivante permettent de déterminer quand le personnage a été touché par une attaque.afin de réinit les upB.
    public int prevtempperc = 0;
    public bool justhit; //Booléen qui devient true si à une frame, les pourcents ont changé par rapport à la dernière frame, ce qui signifie qu'ils ont été touché par une attaque.
    private int atkinput; //détecte si le bouton d'attaque est pressé
    private int speinput; //détecte si le bouton d'attaque spéciale est pressé
    private bool grabed; //sert à déterminer si un perso est grab. Ce bool est récupéré du script charavalues
    private int temppercent;





    //variables d'attaques:
    //elles doivent être de la forme:nomtype. Par exemple, pour tiltpercent, le nom de l'attaque est tilt et le type est percent.

    //voici une liste et une description de chaque type (il n'est pas nécessaire de tous les avoirs. ça dépend de l'action):

    //attackpoint: la transform qui correspondra à la hitbox de l'attaque. Si l'attaque doit avoir plusieurs hitbox, il faudra plusieurs transform, alors il y aura des attackpoint2, attackpoint3, etc...
    //range: rayon de la sphère qu'est la hitbox
    //hbx: largeur du rectangle si la hitbox est un rectange
    //hby: hauteur du rectangle si la hitbox est un rectange
    //percent: les pourcents infligés par l'attaque
    //attackdelay: nombre de frame de CD après l'attaque
    //delaytcounter: compteur lié à attackdelay
    //shielddamage: points de boucliers enlevés sil'attaque touche le bouclier
    //length: durée pendant laquelle l'attaque est active (en frame)
    //lengthcounter: compteur lié à length.
    //startframe: première frame à laquelle les dégats sont faits.
    //secondhitboxstart: première frmae à partir de laquelle la seconde hitbox devient active
    //baserecoil: recul de base de l'attaque. Les pourcents de la cible seront multiplié par cette valeur pour donner la force d'éjection finale.
    //fixedrecoil: recul fixe de l'attaque. Cette variable existe pour faire une ejection fixe indépendante des pourcents de la cible
    //selfeject: ejection du perso qui lance l'attaque (par exemple pour une charge ou un upB)
    //used: bool qui sert à déterminer si l'attaque a déja été utilisée, typiquement pour les attaques spéciales
    //startdistance: float qui détermine une distance fixe à parcourrir.
    //direction: garde la direction de l'attaque si besoin;
    //startx: garde la coordonnée en x de depart du perso
    //activeproj: accède au gameObject qui est le projectile actif
    //projdistance: la distance maximale que parcourt un projectile avant de disparaître
    //projspeed: vitesse des projectiles

    //Il est à noter que ces variables peuvent être misent en privé ou en public. Comme aucun autre script ne les utilise, les mettre en public n'est techniquement pas utile.
    //Cependant, lors des phases de débeugage, il est conseillé de les mettre soit en public soit d'ajouter [SerializeField] devant private pour que les variables apparaissent dans l'éditeur.
    //Perso je les mettrai pas juste en private, sauf éventuellement les counter


    [Header("Grabattack variables")]
    public Transform grabattackpoint;
    public float grabrange;
    public int grabattackdelay;
    private int grabdelaycounter;
    public int grablength;
    private int grablengthcounter;
    public int grabstartframe;

    [Header("Tiltattack variables")]
    public Transform tiltattackpoint;
    public float tiltrange;
    public int tiltpercent;
    public int tiltattackdelay;
    private int tiltdelaycounter;
    public int tiltshielddamage;
    public int tiltlength;
    private int tiltlengthcounter;
    public int tiltstartframe;
    public float tiltbaserecoil;
    public float tiltfixedrecoil;

    [Header("Dtiltattack variables")]
    public Transform dtiltattackpoint;
    public float dtilhbx;
    public float dtilhby;
    public int dtiltpercent;
    public int dtiltattackdelay;
    private int dtiltdelaycounter;
    public int dtiltshielddamage;
    public int dtiltlength;
    private int dtiltlengthcounter;
    public int dtiltstartframe;
    public float dtiltbaserecoil;
    public float dtiltfixedrecoil;

    [Header("Uptiltattack variables")]
    public Transform uptiltattackpoint;
    public float uptilhbx;
    public float uptilhby;
    public int uptiltpercent;
    public int uptiltattackdelay;
    private int uptiltdelaycounter;
    public int uptiltshielddamage;
    public int uptiltlength;
    public int uptiltlengthcounter;
    public int uptiltstartframe;
    public float uptiltbaserecoil;

    [Header("Nairattack variables")]
    public Transform nairattackpoint;
    public float nairrange;
    public int nairpercent;
    public int nairattackdelay;
    private int nairdelaycounter;
    public int nairshielddamage;
    public int nairlength;
    private int nairlengthcounter;
    public float nairbaserecoil;

    [Header("UpBattack variables")]
    public Transform upbattackpoint;
    public float upbrange;
    public int upbpercent;
    public int upbattackdelay;
    private int upbdelaycounter;
    public int upbshielddamage;
    public int upblength;
    private int upblengthcounter;
    public int upbstartframe;
    public float upbbaserecoil;
    public float upbfixedrecoil;
    public float upbselfeject;
    public bool upbused;

    [Header("DownBattack variables")]
    public Transform dbattackpoint;
    public Transform dbattackpoint2;
    public float dbrange;
    public float dbhbx;
    public float dbhby;
    public int dbpercent;
    public int dbattackdelay;
    private int dbdelaycounter;
    public int dbshielddamage;
    public int dblength;
    private int dblengthcounter;
    public int dbstartframe;
    public int dbsecondhitboxstart;
    public float dbbaserecoil;
    public float dbfixedrecoil;
    public bool dbused;


    [Header("SideBattack variables")]
    public Transform sbattackpoint;
    public float sbhbx;
    public float sbhby;
    public int sbpercent;
    public int sbattackdelay;
    private int sbdelaycounter;
    public int sbshielddamage;
    public int sblength;
    private int sblengthcounter;
    public int sbstartframe;
    public float sbbaserecoil;
    public float sbfixedrecoil;
    public float sbselfeject;
    public bool sbused;
    public float sbdistance;
    private float sbdirection;
    private float sbstartx;

    [Header("NeutralBattack variables")]
    public Transform nbattackpoint;
    public int nbpercent;
    public int nbattackdelay;
    private int nbdelaycounter;
    public int nbshielddamage;
    public int nblength;
    private int nblengthcounter;
    public int nbstartframe;
    public float nbbaserecoil;
    public float nbfixedrecoil;
    private float nbdirection;
    private GameObject nbactiveproj;
    public int nbprojdistance;
    public float nbprojspeed;
    

    [Header("pummel variables")]
    public int pummelpercent;
    public int pummelattackdelay;
    private int pummeldelaycounter;

    [Header("collider touché")]
    public Collider2D cible; //sert à garder en mémoire la dernière cible touchée lors d'une attaque non multi-hit. Cela permet de ne pas toucher deux fois avec la même attaque.

    [Header("recoil variables")]
    private Rigidbody2D enemyrb;

    private bool shielded;

    public bool grounded; //perso au sol

    private int hitstun; //perso a été touché et ne peut pas attaquer

    public bool attacking; //le perso est en train d'attaquer

    private bool grabbing;

    // Start is called before the first frame update
    void Start()
    {

        anim = GetComponent<Animator>(); //initialisation de l'animateur

       

    }

    void OnAttack()
    {
        if(this.CompareTag("Player1"))
        {
            atkinput = 1;
        }
    }

    void OnAttack1()
    {
        if (this.CompareTag("Player2"))
        {
            atkinput = 1;
        }
    }

    void OnSpecial()
    {
        if (this.CompareTag("Player1"))
        {
            speinput = 1;
        }
    }

    void OnSpecial1()
    {
        if (this.CompareTag("Player2"))
        {
            speinput = 1;
        }
    }

    void InputAttack() //se déclenche si le bouton d'attaque est pressé
    {
        if(!grabed && hitstun<=0 && !attacking && !shielded && !grabbing)
        {
            if (grounded || GetComponent<Charamov>().platformed) //check si le perso est sur le sol
            {
                if (tiltdelaycounter == 0 && !GetComponent<Charamov>().crouched && GetComponent<Charamov>().vertical == 0) //si le perso peut faire un tilt et que le bouclier est baissé, la fonction correspondant au tilt se déclenche et le delai entre deux tilts aussi.
                {
                    TiltAttack();
                    tiltdelaycounter = tiltstartframe+tiltlength+tiltattackdelay;
                }
                else if (dtiltdelaycounter == 0 && GetComponent<Charamov>().crouched)
                {
                    DTiltAttack();
                    dtiltdelaycounter = dtiltattackdelay;
                }
                else if (uptiltdelaycounter == 0 && !GetComponent<Charamov>().crouched && GetComponent<Charamov>().vertical == 1)
                {
                    UpTiltAttack();
                    uptiltdelaycounter = uptiltattackdelay;
                }
            }
            else //se déclenche si le perso n'est pas au sol
            {
                if (GetComponent<Charamov>().valueright == 0 && GetComponent<Charamov>().valueright == 0 && nairdelaycounter == 0 ) //si le perso peut faire un nair, qu'aucune input de direction n'est activée et que le bouclier est baissé, la fonction correspondant au nair se déclenche et le delai entre deux nairs aussi.
                {
                    NairAttack();
                    nairdelaycounter = nairattackdelay;
                }
                else //si les conditions pour faire un nair ne sont pas remplis, comme il n'y a pas encore d'autres attaque aériennes implémentées, un tilt est fait.
                {
                    if (tiltdelaycounter == 0)
                    {
                        TiltAttack();
                        tiltdelaycounter = tiltattackdelay;
                    }
                }
            }
        }
        else if (!grabed && hitstun <= 0 && !attacking && !shielded && grabbing && pummeldelaycounter<=0)
        {
            anim.SetTrigger("pummel");
            Pummel();
            pummeldelaycounter = pummelattackdelay;


        }
        else if (!grabed && hitstun <= 0 && !attacking && shielded && !grabbing)
        {
            Grab();
            grabdelaycounter = grabattackdelay;
        }
    }

    void InputSpecial()
    {
        if(!grabed && hitstun<=0 && !attacking && !grabbing)
        {
            if (GetComponent<Charamov>().vertical == 1 && upbdelaycounter == 0 && !upbused)
            {
                upbAttack();
                upbdelaycounter = upbattackdelay;
            }
            else if (GetComponent<Charamov>().horizontal !=0 && sbdelaycounter == 0 && !sbused)
            {
                sbAttack(GetComponent<Charamov>().horizontal);
                sbdelaycounter = sbattackdelay;
            }
            else if (GetComponent<Charamov>().vertical == -1 && dbdelaycounter == 0 && !dbused)
            {
                dbAttack();
                dbdelaycounter = dbattackdelay;
            }
            else if (GetComponent<Charamov>().horizontal == 0 && GetComponent<Charamov>().vertical==0 && nbdelaycounter == 0)
            {
                anim.SetTrigger("neutralb");
                nbAttack(GetComponent<Charamov>().facingRight);
                nbdelaycounter = nbattackdelay;

            }
        }
    }

    void Update()
    {

        if (GetComponent<charavalues>().iframes==GameObject.Find("Global values").GetComponent<Globalvalues>().respawniframes) //on réinitialise les CD de toutes les attaques si le perso meurt.
        {
            Attackreinit();
        }


        shielded = GetComponent<charavalues>().shielded;

        grabbing = GetComponent<charavalues>().grabbing;

        grabed = GetComponent<charavalues>().grabed;

        if (tempperc != GetComponent<charavalues>().percent)
        {
            anim.SetTrigger("hit");
            tempperc = GetComponent<charavalues>().percent;
        }

        hitstun = GetComponent<charavalues>().hitstuncnt;

        if (dtiltlengthcounter>0|| nairlengthcounter>0 || tiltlengthcounter>0 || upblengthcounter>0 || uptiltlengthcounter>0 || grablengthcounter>0 || sblengthcounter>0 || dblengthcounter>0 || nblengthcounter>0)
        {
            GetComponent<charavalues>().attacking = true;
        }
        else
        {
            GetComponent <charavalues>().attacking = false;
        }

        

        if (atkinput==1)
        {
            InputAttack();
            atkinput = 0;
        }

        if (speinput==1)
        {
            InputSpecial();
            speinput = 0;
        }


        


        grounded = GetComponent<Charamov>().grounded;

        if(grounded)
        {
            if(upblengthcounter==0)
            {
                upbused = false;
                anim.SetBool("upb", false);
            }
            if(sblengthcounter==0)
            {
                sbused = false;
                anim.SetBool("sideb", false) ;
            }
            if (dblengthcounter == 0)
            {
                dbused = false;
                anim.SetBool("downb", false);
            }

        }

        tempperc=GetComponent<charavalues>().percent;

        if (tempperc!=prevtempperc && !GetComponent<charavalues>().grabed) //active justhit si les les pourcents ont changés, donc si l'ennemi a été touché.
        {
            justhit = true;
        }


        AttackCD(); //gère les cooldown des move ainsi que les moves qui durent dans le temps

        if (justhit) //désactive justhit
        {
            justhit = false;
        }

        prevtempperc = tempperc;
    }


    //indications sur la façon dont les fonctions ont été faites:
    //pour chaque attaque, il y a une version de base et une version "lingering". (pummel est une exception)
    //la version de base correspond à ce qui se fait directement quand le bouton est pressé.
    //la version lingering correspond à ce qui se passe dans les frames qui suivent.
    //Pour faire des comportement spéciaux tels que des multihits ou du lag avant l'attaque, il suffit de faire des case en fonction du nombre de frame écoulé, c'est à dire à la valeur de la variable lengthcounter.
    //Il est à noter que la suite fais des actions sur une entité désigné par "enemi" et qui peut être n'importe quel personnage en fonction des combats. Ceci est possible grâce au script charavalues, qui fait le lien entre les différents scripts des différents persos. Il est essentiel sur tous les persos.


    //Je vais détailler précisément ce qui se passe dans TiltAttack, puis préciser quelques points sur LingeringTilt.
    //A une exception près, je ne vais rien préciser sur NairAttack et LingeringNair ni sur toutes celles qui vont suivre car elles n'ont pas de différence majeures avec les tilts, si ce n'est la position de la transform et les valeurs des variables.

    void Pummel()
    {
        if (this.CompareTag("Player2"))
        {
            foreach (GameObject O in GameObject.FindGameObjectsWithTag("Player1"))
            {
                hurtsoundeffect();
                O.GetComponent<charavalues>().percent += pummelpercent;
            }
        }
        else
        {
            foreach (GameObject O in GameObject.FindGameObjectsWithTag("Player2"))
            {
                hurtsoundeffect();
                O.GetComponent<charavalues>().percent += pummelpercent;
            }
        }
        
    }
    void Grab()
    {
        anim.SetTrigger("grab");
        grablengthcounter=grablength+grabstartframe;
    }

    void LingeringGrab()
    {
        grablengthcounter -= 1;

        if(grablengthcounter <=grablength && grablengthcounter>0)
        {
            Collider2D[] hitenemies = Physics2D.OverlapCircleAll(grabattackpoint.position, grabrange);

            foreach (Collider2D enemy in hitenemies)
            {
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0 && enemy != cible) //il est important de remarquer qu'ici intervient cible pour éviter que l'attaque ne la touche plusieurs fois.
                {
                    pummeldelaycounter = pummelattackdelay;

                    cible = enemy;

                    enemy.GetComponent<charavalues>().grabbedframes = GameObject.Find("Global values").GetComponent<Globalvalues>().grabtime * (1 + enemy.GetComponent<charavalues>().percent/50);

                    if(enemy.GetComponent<Charamov>().facingRight && GetComponent<Charamov>().facingRight)
                    {
                        enemy.GetComponent<Charamov>().Flip(-1f);
                    }
                    else if(!enemy.GetComponent<Charamov>().facingRight && !GetComponent<Charamov>().facingRight)
                    {
                        enemy.GetComponent<Charamov>().Flip(1f);
                    }
                }
            }
        }
    }

    void TiltAttack()
    {

        if(tiltlengthcounter==0)//ne se déclenche que si lengthcounter est nul
        {
            //attack animation
            anim.SetTrigger("attack");

            tiltlengthcounter = tiltlength+tiltstartframe; //initialise lengthcounter

        }
    }

    //cette fonction correspond aux hitbox après la première. Dans le cas d'attaque plus complexe que la simple: hitbox active frame 1, c'est cette fonction qui fera le heavy lifting.
    //Une solution potentielle est de faire des if/elif/else pour définir à la main l'effet de l'attaque à chaque frame.
    //je ne vais pas détailler cette fonction car ici, elle est identique à la fonction TiltAttack, à l'exception du fait qu'au lieu d'être initialisé, lengthcounter est décrémenté.

    void Lingeringtilt()
    {
        tiltlengthcounter -= 1;
        //get enemies in range

        if(tiltlengthcounter<=tiltlength && tiltlengthcounter>=0)
        {

            Collider2D[] hitenemies = Physics2D.OverlapCircleAll(tiltattackpoint.position, tiltrange);

            foreach (Collider2D enemy in hitenemies)
            {
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0 && enemy != cible) //il est important de remarquer qu'ici intervient cible pour éviter que l'attaque ne la touche plusieurs fois.
                {
                    cible = enemy;
                    if (enemy.GetComponent<charavalues>().shielded)
                    {
                        enemy.GetComponent<charavalues>().shield -= tiltshielddamage;
                    }
                    else
                    {
                        hurtsoundeffect();
                        enemy.GetComponent<charavalues>().percent += tiltpercent;
                        enemyrb = enemy.GetComponent<Rigidbody2D>();

                        if (transform.position.x >= enemy.transform.position.x)
                        {
                            
                            enemyrb.AddForce(new Vector2(-tiltfixedrecoil-tiltbaserecoil * enemy.GetComponent<charavalues>().percent, 100));

                        }
                        else
                        {
                             
                            enemyrb.AddForce(new Vector2(tiltfixedrecoil+tiltbaserecoil * enemy.GetComponent<charavalues>().percent, 100));
                        }
                        GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
                    }

                }

            }
        }

    }

   

    void NairAttack()
    {

        if(nairlengthcounter==0)
        {

            nairlengthcounter = nairlength;

            //attack animation
            anim.SetTrigger("nair");

            //get enemies in range
            Collider2D[] hitenemies = Physics2D.OverlapCircleAll(nairattackpoint.position, nairrange);

            foreach (Collider2D enemy in hitenemies)
            {
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0)
                {


                    if (enemy.GetComponent<charavalues>().shielded)
                    {
                        
                        enemy.GetComponent<charavalues>().shield -= nairshielddamage;
                    }
                    else
                    {
                        hurtsoundeffect();
                        enemy.GetComponent<charavalues>().percent += nairpercent;
                        enemyrb = enemy.GetComponent<Rigidbody2D>();

                        if (transform.position.x >= enemy.transform.position.x)
                        {
                            enemyrb.AddForce(new Vector2(-nairbaserecoil, 0)); //ici, l'éjection de l'attaque est fixe, car les pourcents de la cible ne rentrent pas en jeu.

                        }
                        else
                        {
                            enemyrb.AddForce(new Vector2(nairbaserecoil, 0));
                        }
                        enemyrb.velocity = new Vector2(0, enemyrb.velocity.y);
                    }

                }

            }
        }


    }

    void lingeringnair()
    {
        nairlengthcounter -= 1;

        //get enemies in range
        Collider2D[] hitenemies = Physics2D.OverlapCircleAll(nairattackpoint.position, nairrange);

        foreach (Collider2D enemy in hitenemies)
        {
            if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0)
            {

                if (enemy.GetComponent<charavalues>().shielded)
                {
                    enemy.GetComponent<charavalues>().shield -= nairshielddamage;
                }
                else
                {
                    hurtsoundeffect();
                    enemy.GetComponent<charavalues>().percent += nairpercent;
                    enemyrb = enemy.GetComponent<Rigidbody2D>();

                    if (transform.position.x >= enemy.transform.position.x)
                    {
                        enemyrb.AddForce(new Vector2(-nairbaserecoil, 0));

                    }
                    else
                    {
                        enemyrb.AddForce(new Vector2(nairbaserecoil, 0));
                    }
                    enemyrb.velocity = new Vector2(0, enemyrb.velocity.y);
                }

            }

        }
    }



    void upbAttack()
    {

        if (upblengthcounter == 0)
        {
            GetComponent<charavalues>().upb = true;

            upbused = true;

            upblengthcounter = upblength+upbstartframe;

            //attack animation
            anim.SetBool("upb",true);

            
        }


    }

    void lingeringupb()
    {
        upblengthcounter -= 1;

        if( upblengthcounter <= upblength && upblengthcounter>=0)
        {
            //get enemies in range
            Collider2D[] hitenemies = Physics2D.OverlapCircleAll(upbattackpoint.position, upbrange);

            foreach (Collider2D enemy in hitenemies)
            {
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0 && cible != enemy)
                {

                    cible = enemy;

                    if (enemy.GetComponent<charavalues>().shielded)
                    {

                        enemy.GetComponent<charavalues>().shield -= upbshielddamage;
                    }
                    else
                    {
                        hurtsoundeffect();
                        enemy.GetComponent<charavalues>().percent += upbpercent;
                        enemyrb = enemy.GetComponent<Rigidbody2D>();

                        if (transform.position.x >= enemy.transform.position.x)
                        {
                            enemyrb.AddForce(new Vector2(-upbfixedrecoil - upbbaserecoil* enemy.GetComponent<charavalues>().percent, 0));

                        }
                        else
                        {
                             
                            enemyrb.AddForce(new Vector2(upbfixedrecoil + upbbaserecoil*enemy.GetComponent<charavalues>().percent, 0));
                        }
                        enemyrb.velocity = new Vector2(0, enemyrb.velocity.y);
                    }

                }

            }
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, upbselfeject); //fait aller le perso vers le haut (ben oui c'est un upb)

        }


    }

    void dbAttack()
    {

        if (dblengthcounter == 0)
        {

            dbused = true;

            dblengthcounter = dblength + dbstartframe;

            //attack animation
            anim.SetBool("downb", true);


        }


    }

    void lingeringdb()
    {
        dblengthcounter -= 1;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
        if (dblengthcounter <= dblength && dblengthcounter >= 0)
        {
            //get enemies in range
            Collider2D[] hitenemies = Physics2D.OverlapAreaAll(new Vector2(dbattackpoint.position.x - dbhbx / 2, dbattackpoint.position.y + dbhby / 2), new Vector2(dbattackpoint.position.x + dbhbx / 2, dbattackpoint.position.y - dbhby / 2));

            foreach (Collider2D enemy in hitenemies)
            {
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0 && cible != enemy)
                {

                    cible = enemy;

                    if (enemy.GetComponent<charavalues>().shielded)
                    {

                        enemy.GetComponent<charavalues>().shield -= dbshielddamage;
                    }
                    else
                    {
                        hurtsoundeffect();
                        enemy.GetComponent<charavalues>().percent += dbpercent;
                        enemyrb = enemy.GetComponent<Rigidbody2D>();

                        if (transform.position.x >= enemy.transform.position.x)
                        {
                            enemyrb.AddForce(new Vector2(-dbfixedrecoil - dbbaserecoil * enemy.GetComponent<charavalues>().percent, 0));

                        }
                        else
                        {

                            enemyrb.AddForce(new Vector2(dbfixedrecoil + dbbaserecoil * enemy.GetComponent<charavalues>().percent, 0));
                        }
                    }

                }

            }

        }
        if (dblengthcounter <= dbsecondhitboxstart && dblengthcounter >= 0)
        {
            //Ici, comme on active la second hitbox, qui s'active plus tard que la première
            Collider2D[] hitenemies = Physics2D.OverlapCircleAll(dbattackpoint.position, dbrange);

            foreach (Collider2D enemy in hitenemies)
            {
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0 && cible != enemy)
                {

                    cible = enemy;

                    if (enemy.GetComponent<charavalues>().shielded)
                    {

                        enemy.GetComponent<charavalues>().shield -= dbshielddamage;
                    }
                    else
                    {
                        hurtsoundeffect();
                        enemy.GetComponent<charavalues>().percent += dbpercent;
                        enemyrb = enemy.GetComponent<Rigidbody2D>();

                        if (transform.position.x >= enemy.transform.position.x)
                        {
                            enemyrb.AddForce(new Vector2(-dbfixedrecoil - dbbaserecoil * enemy.GetComponent<charavalues>().percent, dbfixedrecoil + dbbaserecoil* enemy.GetComponent<charavalues>().percent));

                        }
                        else
                        {

                            enemyrb.AddForce(new Vector2(dbfixedrecoil + dbbaserecoil * enemy.GetComponent<charavalues>().percent, dbfixedrecoil + dbbaserecoil * enemy.GetComponent<charavalues>().percent));
                        }
                    }

                }

            }

        }


    }


    void sbAttack(float dir)
    {

        if (sblengthcounter == 0)
        {

            sbdirection = dir;

            sbstartx=transform.position.x;

            GetComponent<charavalues>().sb = true;

            sbused = true;

            sblengthcounter = sblength + sbstartframe;

            //attack animation
            anim.SetBool("sideb", true);


        }


    }

    void lingeringsb()
    {

        sblengthcounter -= 1;

        if (sblengthcounter <= sblength+sbstartframe && sblengthcounter > sblength)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }


        if (sblengthcounter <= sblength && sblengthcounter >= 0)
        {

            if (sbdirection > 0 && transform.position.x < sbstartx + sbdistance)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(sbselfeject, 0);
            }
            else if (sbdirection < 0 && transform.position.x > sbstartx - sbdistance)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-sbselfeject, 0);
            }
            else
            {

                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

                sblengthcounter = 0;

                anim.SetBool("sideb", false);

                cible = null;

                GetComponent<charavalues>().sb = false;

                sbdelaycounter = sbdelaycounter - sblengthcounter;
            }

            //get enemies in range
            Collider2D[] hitenemies = Physics2D.OverlapAreaAll(new Vector2(sbattackpoint.position.x - sbhbx / 2, sbattackpoint.position.y + sbhby / 2), new Vector2(sbattackpoint.position.x + sbhbx / 2, sbattackpoint.position.y - sbhby / 2));

            foreach (Collider2D enemy in hitenemies)
            {
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0 && cible != enemy)
                {

                    cible = enemy;

                    if (enemy.GetComponent<charavalues>().shielded)
                    {

                        enemy.GetComponent<charavalues>().shield -= sbshielddamage;
                    }
                    else
                    {
                        hurtsoundeffect();
                        enemy.GetComponent<charavalues>().percent += sbpercent;
                        enemyrb = enemy.GetComponent<Rigidbody2D>();

                        enemyrb.AddForce(new Vector2(0, sbfixedrecoil + sbbaserecoil * enemy.GetComponent<charavalues>().percent));

                        enemyrb.velocity = new Vector2(0, enemyrb.velocity.y);


                    }

                }
            }

        }


    }


    void nbAttack(bool facingright)
    {

        if (nblengthcounter == 0)
        {

            if(facingright)
            {
                nbdirection = 1;
            }
            else
            {
                nbdirection = -1;
            }

            nblengthcounter = nblength + nbstartframe;

        }


    }

    void lingeringnb()
    {

        nblengthcounter -= 1;

        if (nblengthcounter == nblength)
        {

            nbactiveproj = GameObject.Find("slimenbproj1");
            GameObject nouveauproj = Instantiate(nbactiveproj); //On créée une copie d'un gameobject qui correspond à un projectile. Il est toujours sur la map, mais inactif et hors de la cam.

            //On initialise bien comme il faut tous les paramètres du projo

            nouveauproj.GetComponent<proj>().sendertag = this.tag;
            nouveauproj.transform.position = nbattackpoint.position;

            nouveauproj.GetComponent<proj>().percent = nbpercent;
            nouveauproj.GetComponent<proj>().shielddamage = nbshielddamage;
            nouveauproj.GetComponent<proj>().baserecoil = nbbaserecoil;
            nouveauproj.GetComponent<proj>().fixedrecoil = nbfixedrecoil;
            nouveauproj.GetComponent<proj>().startx = nbattackpoint.position.x;
            nouveauproj.GetComponent<proj>().maxdistance = nbprojdistance;
            nouveauproj.GetComponent<proj>().projspeed = nbprojspeed;
            nouveauproj.GetComponent<proj>().projdirection = nbdirection;
            nouveauproj.GetComponent<proj>().isactive = true;
            nouveauproj.GetComponent<SpriteRenderer>().enabled = true;
            if(this.CompareTag("Player1"))
            {
                nouveauproj.tag = "P1proj";
            }
            else
            {
                nouveauproj.tag = "P2proj";
            }
        }


    }


    void DTiltAttack()
    {

        if (dtiltlengthcounter == 0)
        {

            dtiltlengthcounter = dtiltlength+dtiltstartframe;

            //attack animation
            anim.SetTrigger("attack");

        }


    }



    void Lingeringdtilt()
    {
       dtiltlengthcounter -= 1;


        if (dtiltlengthcounter <= dtiltlength && dtiltlengthcounter>=0)
        {

            Collider2D[] hitenemies = Physics2D.OverlapAreaAll(new Vector2(dtiltattackpoint.position.x - dtilhbx / 2, dtiltattackpoint.position.y + dtilhby / 2), new Vector2(dtiltattackpoint.position.x + dtilhbx / 2, dtiltattackpoint.position.y - dtilhby / 2));

            foreach (Collider2D enemy in hitenemies)
            {
                
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0 && enemy != cible) //il est important de remarquer qu'ici intervient cible pour éviter que l'attaque ne la touche plusieurs fois.
                {

                    cible = enemy;

                    if (enemy.GetComponent<charavalues>().shielded)
                    {
                        enemy.GetComponent<charavalues>().shield -= dtiltshielddamage;
                    }
                    else
                    {
                        hurtsoundeffect();
                        enemy.GetComponent<charavalues>().percent += dtiltpercent;
                        enemyrb = enemy.GetComponent<Rigidbody2D>();
                         
                        enemyrb.AddForce(new Vector2(0, dtiltfixedrecoil + dtiltbaserecoil * enemy.GetComponent<charavalues>().percent)); //ici fixedrecoil est additionné à baserecoil pour faire une attaque qui ejectera toujours environ à la même hauter, mais tout de même un tout petit peu augmenté par les pourcents.

                        
                        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x,0);
                    }

                }

            }
        }

    }


    void UpTiltAttack()
    {

        if (uptiltlengthcounter == 0)
        {

            uptiltlengthcounter = uptiltlength+uptiltstartframe;

            //attack animation
            anim.SetTrigger("uptilt");

        }


    }

    void Lingeringuptilt()
    {
        uptiltlengthcounter -= 1;
        //get enemies in range

        if (uptiltlengthcounter <= uptiltlength - uptiltstartframe && uptiltlengthcounter >= uptiltstartframe)
        {

            Collider2D[] hitenemies = Physics2D.OverlapAreaAll(new Vector2(uptiltattackpoint.position.x - uptilhbx / 2, uptiltattackpoint.position.y + uptilhby / 2), new Vector2(uptiltattackpoint.position.x + uptilhbx / 2, uptiltattackpoint.position.y - uptilhby / 2));

            foreach (Collider2D enemy in hitenemies)
            {
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0 && enemy != cible) //il est important de remarquer qu'ici intervient cible pour éviter que l'attaque ne la touche plusieurs fois.
                {

                    cible = enemy;

                    if (enemy.GetComponent<charavalues>().shielded)
                    {
                        enemy.GetComponent<charavalues>().shield -= uptiltshielddamage;
                    }
                    else
                    {
                        hurtsoundeffect();
                        enemy.GetComponent<charavalues>().percent += uptiltpercent;
                        enemyrb = enemy.GetComponent<Rigidbody2D>();
                         
                        enemyrb.AddForce(new Vector2(0, uptiltbaserecoil * enemy.GetComponent<charavalues>().percent));


                        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
                    }

                }

            }
        }

    }




    void AttackCD() //gère les décrémentation des compteur des attaques ainsi que le déclenchement des attaques actives après frame 1
    {
        if (pummeldelaycounter > 0) //CD du pummel
        {
            pummeldelaycounter -= 1;
        }
        if (tiltdelaycounter > 0) //CD du tilt
        {
            attacking = true;
            tiltdelaycounter -= 1;
        }
        else if (dtiltdelaycounter > 0) //CD du dtilt
        {
            attacking = true;
            dtiltdelaycounter -= 1;
        }
        else if (nairdelaycounter > 0) //CD du nair
        {
            attacking = true;
            nairdelaycounter -= 1;
        }
        else if (upbdelaycounter > 0) //CD du upb
        {
            attacking = true;
            upbdelaycounter -= 1;
        }
        else if (sbdelaycounter > 0) //CD du sideb
        {
            attacking = true;
            sbdelaycounter -= 1;
        }
        else if (dbdelaycounter > 0) //CD du sideb
        {
            attacking = true;
            dbdelaycounter -= 1;
        }
        else if (nbdelaycounter > 0) //CD du neutralb
        {
            attacking = true;
            nbdelaycounter -= 1;
        }
        else if (uptiltdelaycounter > 0) //CD du tilt
        {
            attacking = true;
            uptiltdelaycounter -= 1;
        }
        else if(grabdelaycounter > 0)
        {
            attacking =true;
            grabdelaycounter -= 1;
        }
        else
        {
            attacking=false;
        }



        if (justhit) //si le perso a été touché, on termine toutes les animations en cours
        {
            Attackreinit();

        }

        if(grablengthcounter >0)
        {
            if(grablengthcounter == 1)
            {
                LingeringGrab();
                cible = null;
            }
            else
            {
                LingeringGrab();
            }
        }

        if (tiltlengthcounter > 0) //activation du tilt si la hitbox est toujours actives
        {
            if (tiltlengthcounter == 1)
            {
                Lingeringtilt();
                cible = null; //on réinitialise cible à la fin de l'attaque
            }
            else
            {
                Lingeringtilt();
            }
        }

        if (dtiltlengthcounter > 0) //activation du tilt si la hitbox est toujours actives
        {
            if (dtiltlengthcounter == 1)
            {
                Lingeringdtilt();
                cible = null; //on réinitialise cible à la fin de l'attaque
            }
            else
            {
                Lingeringdtilt();
            }
        }
        
        
        if (nairlengthcounter > 0) //activation du nair si la hitbox est toujours actives
        {

            lingeringnair();


        }

        
        if (upblengthcounter > 0) //activation du upb si la hitbox est toujours actives
        {
            if(upblengthcounter == 1)
            {
                lingeringupb();
                cible = null; //on réinitialise cible à la fin de l'attaque
                anim.SetBool("upb", false);
                GetComponent<charavalues>().upb = false;
            }
            else
            {
                lingeringupb();
            }

        }




        if (sblengthcounter > 0) //activation du sideb si la hitbox est toujours actives
        {
            if (sblengthcounter == 1)
            {
                lingeringsb();
                cible = null; //on réinitialise cible à la fin de l'attaque
                anim.SetBool("sideb", false);
                GetComponent<charavalues>().sb = false;
            }
            else
            {
                lingeringsb();
            }

        }

        if (dblengthcounter > 0) //activation du sideb si la hitbox est toujours actives
        {
            if (dblengthcounter == 1)
            {
                lingeringdb();
                cible = null; //on réinitialise cible à la fin de l'attaque
                anim.SetBool("downb", false);
            }
            else
            {
                lingeringdb();
            }

        }

        if (nblengthcounter > 0) //activation du neutralb si la hitbox est toujours actives
        {
            if (nblengthcounter == 1)
            {
                lingeringnb();
            }
            else
            {
                lingeringnb();
            }

        }



        if (uptiltlengthcounter > 0) //activation du tilt si la hitbox est toujours actives
        {
            if (uptiltlengthcounter == 1)
            {
                Lingeringuptilt();
                cible = null; //on réinitialise cible à la fin de l'attaque
            }
            else
            {
                Lingeringuptilt();
            }
        }



    }



    public void Attackreinit()
    {
        tiltlengthcounter = 0;

        dtiltlengthcounter = 0;

        uptiltlengthcounter = 0;

        nairlengthcounter = 0;

        upblengthcounter = 0;

        sblengthcounter = 0;

        grablengthcounter = 0;

        dblengthcounter = 0;

        nblengthcounter = 0;

        upbused = false;

        anim.SetBool("upb", false);

        dbused = false;
        anim.SetBool("downb", false);

        GetComponent<charavalues>().upb = false;

        sbused = false;
        anim.SetBool("sideb", false);

        GetComponent<charavalues>().sb = false;

        cible = null;
    }


    private void OnDrawGizmos()
        //permet d'afficher les hitbox d'attaques. Décommenter pour les afficher
    {
        //Gizmos.DrawWireSphere(tiltattackpoint.position, tiltrange);
        //Gizmos.DrawWireSphere(nairattackpoint.position, nairrange);
        //Gizmos.DrawWireSphere(upbattackpoint.position, upbrange);
        Gizmos.DrawWireSphere(dbattackpoint2.position, dbrange);
        //Gizmos.DrawCube(dtiltattackpoint.position, new Vector2(dtilhbx, dtilhby));
        //Gizmos.DrawCube(uptiltattackpoint.position, new Vector2(uptilhbx, uptilhby));
        //Gizmos.DrawCube(sbattackpoint.position, new Vector2(sbhbx, sbhby));
        Gizmos.DrawCube(dbattackpoint.position, new Vector2(dbhbx, dbhby));
    }

    private void hurtsoundeffect()
    {
        GameObject.Find("soundeffect").GetComponent<soundeffectmanager>().playhurt=true;
    }
//    void OnEnable()
  //  {
    //    controls.gameplay.Enable();
    //}
    //void OnDisable()
    //{
     //   controls.gameplay.Disable();
    //}
}
