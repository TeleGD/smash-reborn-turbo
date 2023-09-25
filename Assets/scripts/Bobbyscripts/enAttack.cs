using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class enAttack : MonoBehaviour
{
    PlayerControls controls;

    public PlayerInput Attack;

    private Animator enanim;
  


    
    


    //variables d'attaques:
    //elles doivent �tre de la forme:nomtype. Par exemple, pour tiltpercent, le nom de l'attaque est tilt et le type est percent.

    //voici une liste et une description de chaque type:

    //attackpoint: la transform qui correspondra � la hitbox de l'attaque. Si l'attaque doit avoir plusieurs hitbox, il faudra plusieurs transform
    //range: rayon de la sph�re qu'est la hitbox
    //percent: les pourcents inflig�s par l'attaque
    //attackdelay: nombre de frame de CD apr�s l'attaque
    //delaytcounter: compteur li� � attackdelay
    //shielddamage: points de boucliers enlev�s sil'attaque touche le bouclier
    //length: dur�e pendant laquelle l'attaque est active (en frame)
    //lengthcounter: compteur li� � length.
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

    [Header("collider touch�")]
    public Collider2D cible; //sert � garder en m�moire la derni�re cible touch�e lors d'une attaque non multi-hit. Cela permet de ne pas toucher deux fois avec la m�me attaque.

    [Header("recoil variables")]
    private Rigidbody2D enemyrb;

    public bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        enanim = GetComponent<Animator>(); //initialisation de l'animateur
    }


    void OnAttack1() //se d�clenche si le bouton d'attaque est press�
    {
        if (grounded) //check si le perso est sur le sol
        {
            if (tiltdelaycounter == 0 && !GetComponent<EnMovement>().shielded) //si le perso peut faire un tilt et que le bouclier est baiss�, la fonction correspondant au tilt se d�clenche et le delai entre deux tilts aussi.
            {
                TiltAttack();
                tiltdelaycounter = tiltattackdelay;
            }
        }
        else //se d�clenche si le perso n'est pas au sol
        {
            if(GetComponent<EnMovement>().valueright==0 && GetComponent<EnMovement>().valueright == 0 && nairdelaycounter == 0 && !GetComponent<EnMovement>().shielded) //si le perso peut faire un nair, qu'aucune input de direction n'est activ�e et que le bouclier est baiss�, la fonction correspondant au nair se d�clenche et le delai entre deux nairs aussi.
            {
                NairAttack();
                nairdelaycounter = nairattackdelay;
            }
            else //si les conditions pour faire un nair ne sont pas remplis, comme il n'y a pas encore d'autres attaque a�riennes impl�ment�es, un tilt est fait.
            {
                if(tiltdelaycounter == 0 && !GetComponent<EnMovement>().shielded)
                {
                    TiltAttack();
                    tiltdelaycounter = tiltattackdelay;
                }
            }
        }
        
    }

    void Update()
    {

        grounded = GetComponent<EnJumpV3>().grounded;

    
        if (tiltdelaycounter>0) //CD du tilt
        {

            tiltdelaycounter -= 1;
        }
        if (tiltlengthcounter > 0) //activation du tilt si la hitbox est toujours actives
        {
            if(tiltlengthcounter==1) 
            {
                Lingeringtilt();
                cible = null; //on r�initialise cible � la fin de l'attaque
            }
            else
            {
                Lingeringtilt();
            }
        }




        if (nairdelaycounter > 0) //CD du nair
        {
            nairdelaycounter -= 1;
        }
        if (nairlengthcounter > 0) //activation du nair si la hitbox est toujours actives
        {
            lingeringnair();

        }

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
                if (enemy.tag == "Player1" && enemy.GetComponent<PlayerHP>().iframes == 0) //check le tag de chaque hitbox. Ceci permet d'�viter de se faire toucehr par sa propre attaque, et d�pend de si le personnage est le joueur 1 ou le joueur 2. Check �galement si le perso est pas en respawn iframes.
                {
                    cible = enemy;

                    if (enemy.GetComponent<PlayerMovement>().shielded) //fait des shield damage si le shield est actif
                    {
                        enemy.GetComponent<PlayerMovement>().shield -= tiltshielddamage;
                    }
                    else //fait des pourcents et de l'�j�ction sinon
                    {
                        enemy.GetComponent<PlayerHP>().player1percent += tiltpercent; //modifie les pourcents de l'ennemi
                        enemyrb = enemy.GetComponent<Rigidbody2D>();

                        //d�termine la direction vers laquelle projeter l'ennemi en fonction de si il est derri�re ou devant
                        //il est � noter que cette partie peut �tre modifi�e pour d�pendre de horizontale pour que l'attaque proj�te toujours vers la m�me direction peu importe comment la cible est frapp�e
                        if (transform.position.x >= enemy.transform.position.x) 
                        {
                            enemyrb.AddForce(new Vector2(-tiltbaserecoil * enemy.GetComponent<PlayerHP>().player1percent, 0));

                        }
                        else
                        {
                            enemyrb.AddForce(new Vector2(tiltbaserecoil * enemy.GetComponent<PlayerHP>().player1percent, 0));
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
                if (enemy.tag == "Player1" && enemy.GetComponent<PlayerHP>().iframes == 0 && enemy != cible) //il est important de remarquer qu'ici intervient cible pour �viter que l'attaque ne la touche plusieurs fois.
                {

                    cible = enemy;

                    if (enemy.GetComponent<PlayerMovement>().shielded)
                    {
                        enemy.GetComponent<PlayerMovement>().shield -= tiltshielddamage;
                    }
                    else
                    {
                        enemy.GetComponent<PlayerHP>().player1percent += tiltpercent;
                        enemyrb = enemy.GetComponent<Rigidbody2D>();

                        if (transform.position.x >= enemy.transform.position.x)
                        {
                            enemyrb.AddForce(new Vector2(-tiltbaserecoil * enemy.GetComponent<PlayerHP>().player1percent, 0));

                        }
                        else
                        {
                            enemyrb.AddForce(new Vector2(tiltbaserecoil * enemy.GetComponent<PlayerHP>().player1percent, 0));
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
                if (enemy.tag == "Player1" && enemy.GetComponent<PlayerHP>().iframes == 0)
                {

                    if (enemy.GetComponent<PlayerMovement>().shielded)
                    {
                        enemy.GetComponent<PlayerMovement>().shield -= nairshielddamage;
                    }
                    else
                    {
                        enemy.GetComponent<PlayerHP>().player1percent += nairpercent;
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
            if (enemy.tag == "Player1" && enemy.GetComponent<PlayerHP>().iframes == 0)
            {

                if (enemy.GetComponent<PlayerMovement>().shielded)
                {
                    enemy.GetComponent<PlayerMovement>().shield -= nairshielddamage;
                }
                else
                {
                    enemy.GetComponent<PlayerHP>().player1percent += nairpercent;
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

    private void OnDrawGizmos()
        //permet d'afficher les hitbox d'attaques
    {
        Gizmos.DrawWireSphere(tiltattackpoint.position, tiltrange);
        Gizmos.DrawWireSphere(nairattackpoint.position, nairrange);
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
