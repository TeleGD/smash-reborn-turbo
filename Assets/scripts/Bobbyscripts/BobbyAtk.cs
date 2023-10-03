using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class BobbyAtk : MonoBehaviour
{
    PlayerControls controls;

    public PlayerInput Attack;

    private Animator enanim;



    [Header("Character variables")]
    public int playernumber;
    public int enemynumber;
    public string enemytag;
    public string enemyname;





    //variables d'attaques:
    //elles doivent �tre de la forme:nomtype. Par exemple, pour tiltpercent, le nom de l'attaque est tilt et le type est percent.

    //voici une liste et une description de chaque type (il n'est pas n�cessaire de tous les avoirs. �a d�pend de l'action):

    //attackpoint: la transform qui correspondra � la hitbox de l'attaque. Si l'attaque doit avoir plusieurs hitbox, il faudra plusieurs transform
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
    //baserecoil: recul de base de l'attaque. Les pourcents de la cible seront multipli� par cette valeur pour donner la force d'�jection finale.
    [Header("Tiltattack variables")]
    public Transform tiltattackpoint;
    public float tiltrange;
    public int tiltpercent;
    public int tiltattackdelay; 
    public int tiltdelaycounter;
    public int tiltshielddamage;
    public int tiltlength;
    public int tiltlengthcounter;
    public float tiltbaserecoil;

    [Header("Nairattack variables")]
    public Transform nairattackpoint;
    public float nairrange;
    public int nairpercent;
    public int nairattackdelay; 
    public int nairdelaycounter;
    public int nairshielddamage;
    public int nairlength;
    public int nairlengthcounter;
    public float nairbaserecoil;




    [Header("Dtiltattack variables")]
    public Transform dtiltattackpoint;
    public float dtilhbx;
    public float dtilhby;
    public int dtiltpercent;
    public int dtiltattackdelay;
    public int dtiltdelaycounter;
    public int dtiltshielddamage;
    public int dtiltlength;
    public int dtiltlengthcounter;
    public int dtiltstartframe;
    public float dtiltbaserecoil;

    [Header("Uptiltattack variables")]
    public Transform uptiltattackpoint;
    public float uptilhbx;
    public float uptilhby;
    public int uptiltpercent;
    public int uptiltattackdelay;
    public int uptiltdelaycounter;
    public int uptiltshielddamage;
    public int uptiltlength;
    public int uptiltlengthcounter;
    public int uptiltstartframe;
    public float uptiltbaserecoil;

    [Header("collider touch�")]
    public Collider2D cible; //sert � garder en m�moire la derni�re cible touch�e lors d'une attaque non multi-hit. Cela permet de ne pas toucher deux fois avec la m�me attaque.

    [Header("recoil variables")]
    private Rigidbody2D enemyrb;

    public bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        

        if(playernumber == 1)
        {
            enemyname = GameObject.Find("Global values").GetComponent<Globalvalues>().player2char;
        }
        else
        {
            enemyname = GameObject.Find("Global values").GetComponent<Globalvalues>().player1char;
        }

        enanim = GetComponent<Animator>(); //initialisation de l'animateur
    }


    void OnAttack1() //se d�clenche si le bouton d'attaque est press�
    {
        if (grounded) //check si le perso est sur le sol
        {
            if (tiltdelaycounter == 0 && !GetComponent<charavalues>().shielded && !GetComponent<BobbyMov>().crouched && GetComponent<BobbyMov>().vertical == 0) //si le perso peut faire un tilt et que le bouclier est baiss�, la fonction correspondant au tilt se d�clenche et le delai entre deux tilts aussi.
            {
                TiltAttack();
                tiltdelaycounter = tiltattackdelay;
            }
            else if(dtiltdelaycounter == 0 && !GetComponent<charavalues>().shielded && GetComponent<BobbyMov>().crouched)
            {
                DTiltAttack();
                dtiltdelaycounter = dtiltattackdelay;
            }
            else if(uptiltdelaycounter==0 && !GetComponent<charavalues>().shielded && !GetComponent<BobbyMov>().crouched && GetComponent<BobbyMov>().vertical==1)
            {
                UpTiltAttack();
                uptiltdelaycounter = uptiltattackdelay;
            }
        }
        else //se d�clenche si le perso n'est pas au sol
        {
            if(GetComponent<BobbyMov>().valueright==0 && GetComponent<BobbyMov>().valueright == 0 && nairdelaycounter == 0 && !GetComponent<charavalues>().shielded) //si le perso peut faire un nair, qu'aucune input de direction n'est activ�e et que le bouclier est baiss�, la fonction correspondant au nair se d�clenche et le delai entre deux nairs aussi.
            {
                NairAttack();
                nairdelaycounter = nairattackdelay;
            }
            else //si les conditions pour faire un nair ne sont pas remplis, comme il n'y a pas encore d'autres attaque a�riennes impl�ment�es, un tilt est fait.
            {
                if(tiltdelaycounter == 0 && !GetComponent<charavalues>().shielded)
                {
                    TiltAttack();
                    tiltdelaycounter = tiltattackdelay;
                }
            }
        }
        
    }

    void Update()
    {

        grounded = GetComponent<BobbyJump>().grounded;

        AttackCD();
    
        


    }


    //indications sur la fa�on dont les fonctions ont �t� faites:
    //pour chaque attaque, il y a une version de base et une version "lingering".
    //la version de base correspond � ce qui se fait directement quand le bouton est press�.
    //la version lingering correspond � ce qui se passe dans les frames qui suivent.
    //Pour faire des comportement sp�ciaux tels que des multihits ou du lag avant l'attaque, il suffit de faire des case en fonction du nombre de frame �coul�, c'est � dire � la valeur de la variable lengthcounter.
    

    //Je vais d�tailler pr�cis�ment ce qui se passe dans TiltAttack, puis pr�ciser quelques points sur LingeringTilt.
    //A une exception pr�s, je ne vais rien pr�ciser sur NairAttack et LingeringNair ni sur toutes celles qui vont suivre car elles n'ont pas de diff�rence majeures avec les tilts, si ce n'est la position de la transform et les valeurs des variables.


    void TiltAttack()
    {

        if(tiltlengthcounter==0)//ne se d�clenche que si lengthcounter est nul
        {
            //attack animation
            enanim.SetTrigger("attack");

            tiltlengthcounter = tiltlength; //initialise lengthcounter

            Collider2D[] hitenemies = Physics2D.OverlapCircleAll(tiltattackpoint.position, tiltrange); //va chercher tous les collider2D pr�sent dans la hitbox de l'attaque

            foreach (Collider2D enemy in hitenemies) //boucle for
            {
                if (enemy.tag == enemytag && enemy.GetComponent<RandyHP>().GetComponent<charavalues>().iframes == 0) //check le tag de chaque hitbox. Ceci permet d'�viter de se faire toucehr par sa propre attaque, et d�pend de si le personnage est le joueur 1 ou le joueur 2. Check �galement si le perso est pas en respawn iframes.
                {
                    cible = enemy;

                    if (enemy.GetComponent<charavalues>().shielded) //fait des shield damage si le shield est actif
                    {
                        enemy.GetComponent<charavalues>().shield -= tiltshielddamage;
                    }
                    else //fait des pourcents et de l'�j�ction sinon
                    {
                        enemy.GetComponent<charavalues>().percent += tiltpercent; //modifie les pourcents de l'ennemi
                        enemyrb = enemy.GetComponent<Rigidbody2D>();

                        //d�termine la direction vers laquelle projeter l'ennemi en fonction de si il est derri�re ou devant
                        //il est � noter que cette partie peut �tre modifi�e pour d�pendre de horizontale pour que l'attaque proj�te toujours vers la m�me direction peu importe comment la cible est frapp�e
                        if (transform.position.x >= enemy.transform.position.x) 
                        {
                            enemyrb.AddForce(new Vector2(-tiltbaserecoil * enemy.GetComponent<charavalues>().percent, 0));

                        }
                        else
                        {
                            enemyrb.AddForce(new Vector2(tiltbaserecoil * enemy.GetComponent<charavalues>().percent, 0));
                        }
                        GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
                    }

                }

            }
        }

        

    }

    //cette fonction correspond aux hitbox apr�s la premi�re. Dans le cas d'attaque plus complexe que la simple: hitbox active frame 1, c'est cette fonction qui fera le heavy lifting.
    //Une solution potentielle est de faire des if/elif/else pour d�finir � la main l'effet de l'attaque � chaque frame.
    //je ne vais pas d�tailler cette fonction car ici, elle est identique � la fonction TiltAttack, � l'exception du fait qu'au lieu d'�tre initialis�, lengthcounter est d�cr�ment�.

    void Lingeringtilt()
    {
        tiltlengthcounter -= 1;
        //get enemies in range

        if(tiltlengthcounter%2==0)
        {

            Collider2D[] hitenemies = Physics2D.OverlapCircleAll(tiltattackpoint.position, tiltrange);

            foreach (Collider2D enemy in hitenemies)
            {
                if (enemy.tag == "Player1" && enemy.GetComponent<charavalues>().iframes == 0 && enemy != cible) //il est important de remarquer qu'ici intervient cible pour �viter que l'attaque ne la touche plusieurs fois.
                {

                    cible = enemy;

                    if (enemy.GetComponent<charavalues>().shielded)
                    {
                        enemy.GetComponent<charavalues>().shield -= tiltshielddamage;
                    }
                    else
                    {
                        enemy.GetComponent<charavalues>().percent += tiltpercent;
                        enemyrb = enemy.GetComponent<Rigidbody2D>();

                        if (transform.position.x >= enemy.transform.position.x)
                        {
                            enemyrb.AddForce(new Vector2(-tiltbaserecoil * enemy.GetComponent<charavalues>().percent, 0));

                        }
                        else
                        {
                            enemyrb.AddForce(new Vector2(tiltbaserecoil * enemy.GetComponent<charavalues>().percent, 0));
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
            enanim.SetTrigger("nair");

            //get enemies in range
            Collider2D[] hitenemies = Physics2D.OverlapCircleAll(nairattackpoint.position, nairrange);

            foreach (Collider2D enemy in hitenemies)
            {
                if (enemy.tag == "Player1" && enemy.GetComponent<charavalues>().iframes == 0)
                {

                    if (enemy.GetComponent<charavalues>().shielded)
                    {
                        enemy.GetComponent<charavalues>().shield -= nairshielddamage;
                    }
                    else
                    {
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
            if (enemy.tag == "Player1" && enemy.GetComponent<charavalues>().iframes == 0)
            {

                if (enemy.GetComponent<charavalues>().shielded)
                {
                    enemy.GetComponent<charavalues>().shield -= nairshielddamage;
                }
                else
                {
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


    void DTiltAttack()
    {

        if (dtiltlengthcounter == 0)
        {

            dtiltlengthcounter = dtiltlength;

            //attack animation
            enanim.SetTrigger("attack");

        }


    }



    void Lingeringdtilt()
    {
       dtiltlengthcounter -= 1;
        //get enemies in range

        if (dtiltlengthcounter <= dtiltlength-dtiltstartframe)
        {

            Collider2D[] hitenemies = Physics2D.OverlapAreaAll(new Vector2(dtiltattackpoint.position.x - dtilhbx / 2, dtiltattackpoint.position.y + dtilhby / 2), new Vector2(dtiltattackpoint.position.x + dtilhbx / 2, dtiltattackpoint.position.y - dtilhby / 2));

            foreach (Collider2D enemy in hitenemies)
            {
                
                if (enemy.tag == "Player1" && enemy.GetComponent<charavalues>().iframes == 0 && enemy != cible) //il est important de remarquer qu'ici intervient cible pour �viter que l'attaque ne la touche plusieurs fois.
                {

                    cible = enemy;

                    if (enemy.GetComponent<charavalues>().shielded)
                    {
                        enemy.GetComponent<charavalues>().shield -= dtiltshielddamage;
                    }
                    else
                    {
                        enemy.GetComponent<charavalues>().percent += dtiltpercent;
                        enemyrb = enemy.GetComponent<Rigidbody2D>();
                        enemyrb.AddForce(new Vector2(0, 300 + uptiltbaserecoil * enemy.GetComponent<charavalues>().percent));

                        
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

            uptiltlengthcounter = uptiltlength;

            //attack animation
            enanim.SetTrigger("uptilt");

        }


    }

    void Lingeringuptilt()
    {
        uptiltlengthcounter -= 1;
        //get enemies in range

        if (uptiltlengthcounter <= uptiltlength - uptiltstartframe)
        {

            Collider2D[] hitenemies = Physics2D.OverlapAreaAll(new Vector2(uptiltattackpoint.position.x - uptilhbx / 2, uptiltattackpoint.position.y + uptilhby / 2), new Vector2(uptiltattackpoint.position.x + uptilhbx / 2, uptiltattackpoint.position.y - uptilhby / 2));

            foreach (Collider2D enemy in hitenemies)
            {
                if (enemy.tag == "Player1" && enemy.GetComponent<charavalues>().iframes == 0 && enemy != cible) //il est important de remarquer qu'ici intervient cible pour �viter que l'attaque ne la touche plusieurs fois.
                {

                    cible = enemy;

                    if (enemy.GetComponent<charavalues>().shielded)
                    {
                        enemy.GetComponent<charavalues>().shield -= uptiltshielddamage;
                    }
                    else
                    {
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
        if (tiltdelaycounter > 0) //CD du tilt
        {

            tiltdelaycounter -= 1;
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
        if (dtiltdelaycounter > 0) //CD du dtilt
        {

            dtiltdelaycounter -= 1;
        }




        if (nairdelaycounter > 0) //CD du nair
        {
            nairdelaycounter -= 1;
        }
        if (nairlengthcounter > 0) //activation du nair si la hitbox est toujours actives
        {
            lingeringnair();

        }


        if (uptiltdelaycounter > 0) //CD du tilt
        {

            uptiltdelaycounter -= 1;
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






    private void OnDrawGizmos()
        //permet d'afficher les hitbox d'attaques
    {
        Gizmos.DrawWireSphere(tiltattackpoint.position, tiltrange);
        Gizmos.DrawWireSphere(nairattackpoint.position, nairrange);
        Gizmos.DrawCube(dtiltattackpoint.position, new Vector2(dtilhbx, dtilhby));
        Gizmos.DrawCube(uptiltattackpoint.position, new Vector2(uptilhbx, uptilhby));
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
