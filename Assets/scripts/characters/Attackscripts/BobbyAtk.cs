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



    [Header("Character variables")] //Ces variables sont les variablesqui vont �tre modifi�es lorsque l'ennemi va �tre attaqu�. Les trois premi�res ont �t� regroup� dans un script qui sera le seul pr�sent dans tous les personnages.
    public int playernumber;
    public int enemynumber;
    public int tempperc = 0; //cette variable et la suivante permettent de d�terminer quand le personnage a �t� touch� par une attaque.afin de r�init les upB.
    public int prevtempperc = 0;
    public bool justhit; //Bool�en qui devient true si � une frame, les pourcents ont chang� par rapport � la derni�re frame, ce qui signifie qu'ils ont �t� touch� par une attaque.
    private int atkinput; //d�tecte si le bouton d'attaque est press�
    private int speinput; //d�tecte si le bouton d'attaque sp�ciale est press�
    private bool grabed; //sert � d�terminer si un perso est grab. Ce bool est r�cup�r� du script charavalues
    private int temppercent;





    //variables d'attaques:
    //elles doivent �tre de la forme:nomtype. Par exemple, pour tiltpercent, le nom de l'attaque est tilt et le type est percent.

    //voici une liste et une description de chaque type (il n'est pas n�cessaire de tous les avoirs. �a d�pend de l'action):

    //attackpoint: la transform qui correspondra � la hitbox de l'attaque. Si l'attaque doit avoir plusieurs hitbox, il faudra plusieurs transform, alors il y aura des attackpoint2, attackpoint3, etc...
    //range: rayon de la sph�re qu'est la hitbox
    //hbx: largeur du rectangle si la hitbox est un rectange
    //hby: hauteur du rectangle si la hitbox est un rectange
    //percent: les pourcents inflig�s par l'attaque
    //attackdelay: nombre de frame de CD apr�s l'attaque
    //delaytcounter: compteur li� � attackdelay
    //shielddamage: points de boucliers enlev�s sil'attaque touche le bouclier
    //length: dur�e pendant laquelle l'attaque est active (en frame)
    //lengthcounter: compteur li� � length.
    //startframe: premi�re frame � laquelle les d�gats sont faits.
    //secondhitboxstart: premi�re frmae � partir de laquelle la seconde hitbox devient active
    //baserecoil: recul de base de l'attaque. Les pourcents de la cible seront multipli� par cette valeur pour donner la force d'�jection finale.
    //fixedrecoil: recul fixe de l'attaque. Cette variable existe pour faire une ejection fixe ind�pendante des pourcents de la cible
    //selfeject: ejection du perso qui lance l'attaque (par exemple pour une charge ou un upB)
    //used: bool qui sert � d�terminer si l'attaque a d�ja �t� utilis�e, typiquement pour les attaques sp�ciales
    //startdistance: float qui d�termine une distance fixe � parcourrir.
    //direction: garde la direction de l'attaque si besoin;
    //startx: garde la coordonn�e en x de depart du perso
    //activeprojindex: donne l'index du projectile qui va �tre utilis� par l'attaque
    //activeproj: acc�de au gameObject qui est le projectile actif
    //projk: acc�de au gameObject qui est le projectile d'indice k
    //projdistance: la distance maximale que parcourt un projectile avant de dispara�tre
    //projspeed: vitesse des projectiles

    //Il est � noter que ces variables peuvent �tre misent en priv� ou en public. Comme aucun autre script ne les utilise, les mettre en public n'est techniquement pas utile.
    //Cependant, lors des phases de d�beugage, il est conseill� de les mettre soit en public soit d'ajouter [SerializeField] devant private pour que les variables apparaissent dans l'�diteur.
    //Perso je les mettrai pas juste en private, sauf �ventuellement les counter


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
    private int nbactiveprojindex;
    private GameObject nbactiveproj;
    private GameObject nbproj1;
    private GameObject nbproj2;
    private GameObject nbproj3;
    public int nbprojdistance;
    public float nbprojspeed;
    

    [Header("pummel variables")]
    public int pummelpercent;
    public int pummelattackdelay;
    private int pummeldelaycounter;

    [Header("collider touch�")]
    public Collider2D cible; //sert � garder en m�moire la derni�re cible touch�e lors d'une attaque non multi-hit. Cela permet de ne pas toucher deux fois avec la m�me attaque.

    [Header("recoil variables")]
    private Rigidbody2D enemyrb;

    private bool shielded;

    public bool grounded; //perso au sol

    private int hitstun; //perso a �t� touch� et ne peut pas attaquer

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

    void InputAttack() //se d�clenche si le bouton d'attaque est press�
    {
        if(!grabed && hitstun<=0 && !attacking && !shielded && !grabbing)
        {
            if (grounded) //check si le perso est sur le sol
            {
                if (tiltdelaycounter == 0 && !GetComponent<Charamov>().crouched && GetComponent<Charamov>().vertical == 0) //si le perso peut faire un tilt et que le bouclier est baiss�, la fonction correspondant au tilt se d�clenche et le delai entre deux tilts aussi.
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
            else //se d�clenche si le perso n'est pas au sol
            {
                if (GetComponent<Charamov>().valueright == 0 && GetComponent<Charamov>().valueright == 0 && nairdelaycounter == 0 ) //si le perso peut faire un nair, qu'aucune input de direction n'est activ�e et que le bouclier est baiss�, la fonction correspondant au nair se d�clenche et le delai entre deux nairs aussi.
                {
                    NairAttack();
                    nairdelaycounter = nairattackdelay;
                }
                else //si les conditions pour faire un nair ne sont pas remplis, comme il n'y a pas encore d'autres attaque a�riennes impl�ment�es, un tilt est fait.
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

        if (GetComponent<charavalues>().iframes==GameObject.Find("Global values").GetComponent<Globalvalues>().respawniframes) //on r�initialise les CD de toutes les attaques si le perso meurt.
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

        if (tempperc!=prevtempperc && !GetComponent<charavalues>().grabed) //active justhit si les les pourcents ont chang�s, donc si l'ennemi a �t� touch�.
        {
            justhit = true;
        }


        AttackCD(); //g�re les cooldown des move ainsi que les moves qui durent dans le temps

        if (justhit) //d�sactive justhit
        {
            justhit = false;
        }

        prevtempperc = tempperc;
    }


    //indications sur la fa�on dont les fonctions ont �t� faites:
    //pour chaque attaque, il y a une version de base et une version "lingering". (pummel est une exception)
    //la version de base correspond � ce qui se fait directement quand le bouton est press�.
    //la version lingering correspond � ce qui se passe dans les frames qui suivent.
    //Pour faire des comportement sp�ciaux tels que des multihits ou du lag avant l'attaque, il suffit de faire des case en fonction du nombre de frame �coul�, c'est � dire � la valeur de la variable lengthcounter.
    //Il est � noter que la suite fais des actions sur une entit� d�sign� par "enemi" et qui peut �tre n'importe quel personnage en fonction des combats. Ceci est possible gr�ce au script charavalues, qui fait le lien entre les diff�rents scripts des diff�rents persos. Il est essentiel sur tous les persos.


    //Je vais d�tailler pr�cis�ment ce qui se passe dans TiltAttack, puis pr�ciser quelques points sur LingeringTilt.
    //A une exception pr�s, je ne vais rien pr�ciser sur NairAttack et LingeringNair ni sur toutes celles qui vont suivre car elles n'ont pas de diff�rence majeures avec les tilts, si ce n'est la position de la transform et les valeurs des variables.

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
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0 && enemy != cible) //il est important de remarquer qu'ici intervient cible pour �viter que l'attaque ne la touche plusieurs fois.
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

        if(tiltlengthcounter==0)//ne se d�clenche que si lengthcounter est nul
        {
            //attack animation
            anim.SetTrigger("attack");

            tiltlengthcounter = tiltlength+tiltstartframe; //initialise lengthcounter

        }
    }

    //cette fonction correspond aux hitbox apr�s la premi�re. Dans le cas d'attaque plus complexe que la simple: hitbox active frame 1, c'est cette fonction qui fera le heavy lifting.
    //Une solution potentielle est de faire des if/elif/else pour d�finir � la main l'effet de l'attaque � chaque frame.
    //je ne vais pas d�tailler cette fonction car ici, elle est identique � la fonction TiltAttack, � l'exception du fait qu'au lieu d'�tre initialis�, lengthcounter est d�cr�ment�.

    void Lingeringtilt()
    {
        tiltlengthcounter -= 1;
        //get enemies in range

        if(tiltlengthcounter<=tiltlength && tiltlengthcounter>=0)
        {

            Collider2D[] hitenemies = Physics2D.OverlapCircleAll(tiltattackpoint.position, tiltrange);

            foreach (Collider2D enemy in hitenemies)
            {
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0 && enemy != cible) //il est important de remarquer qu'ici intervient cible pour �viter que l'attaque ne la touche plusieurs fois.
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
                            enemyrb.AddForce(new Vector2(-nairbaserecoil, 0)); //ici, l'�jection de l'attaque est fixe, car les pourcents de la cible ne rentrent pas en jeu.

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
            //Ici, comme on active la second hitbox, qui s'active plus tard que la premi�re
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
            GameObject nouveauproj = Instantiate(nbactiveproj);
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
            if(this.CompareTag("Player1"))
            {
                nouveauproj.tag = "P1proj";
            }
            else
            {
                nouveauproj.tag = "P2proj";
            }

            //nbactiveproj.SetActive(true);

            //nbactiveproj.transform.position = nbattackpoint.position;

            //nbactiveproj.GetComponent<proj>().percent = nbpercent;
            //nbactiveproj.GetComponent<proj>().shielddamage = nbshielddamage;
            //nbactiveproj.GetComponent<proj>().baserecoil = nbbaserecoil;
            //nbactiveproj.GetComponent<proj>().fixedrecoil = nbfixedrecoil;
            //nbactiveproj.GetComponent<proj>().startx = nbactiveproj.GetComponent<Rigidbody2D>().position.x;
            //nbactiveproj.GetComponent<proj>().maxdistance = nbprojdistance;
            //nbactiveproj.GetComponent<proj>().projspeed = nbprojspeed;
            //nbactiveproj.GetComponent<proj>().projdirection = nbdirection;
            //nbactiveproj.GetComponent<proj>().sendertag = this.tag;
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
                
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0 && enemy != cible) //il est important de remarquer qu'ici intervient cible pour �viter que l'attaque ne la touche plusieurs fois.
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
                         
                        enemyrb.AddForce(new Vector2(0, dtiltfixedrecoil + dtiltbaserecoil * enemy.GetComponent<charavalues>().percent)); //ici fixedrecoil est additionn� � baserecoil pour faire une attaque qui ejectera toujours environ � la m�me hauter, mais tout de m�me un tout petit peu augment� par les pourcents.

                        
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
                if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0 && enemy != cible) //il est important de remarquer qu'ici intervient cible pour �viter que l'attaque ne la touche plusieurs fois.
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




    void AttackCD() //g�re les d�cr�mentation des compteur des attaques ainsi que le d�clenchement des attaques actives apr�s frame 1
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



        if (justhit) //si le perso a �t� touch�, on termine toutes les animations en cours
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
                cible = null; //on r�initialise cible � la fin de l'attaque
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
                cible = null; //on r�initialise cible � la fin de l'attaque
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
                cible = null; //on r�initialise cible � la fin de l'attaque
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
                cible = null; //on r�initialise cible � la fin de l'attaque
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
                cible = null; //on r�initialise cible � la fin de l'attaque
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
                cible = null; //on r�initialise cible � la fin de l'attaque
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
        //permet d'afficher les hitbox d'attaques. D�commenter pour les afficher
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
