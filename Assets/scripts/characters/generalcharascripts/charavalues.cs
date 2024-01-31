using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class charavalues : MonoBehaviour
{
    [Header("variables globales")]
    public bool shielded; //détermine si le bouclier est activé
    public int shield; //point de bouclier restant
    public int percent; //pourcents du joueur
    public int iframes; //nombres de frames d'invincibilités restantes
    public bool upb; //bool qui détermine quand on active le upb. Il sert à désactiver le blocage de déplacement verticaux quand sur une plateforme
    public bool sb; //bool qui détermine quand on active le sideb. Il sert à désactiver le blocage de déplacement verticaux quand sur une plateforme
    public bool touché; //bool qui détermine quand un personnage est touché. Il peut être utilisé pour une attaque qui envoit vers le haut. Il sert à désactiver le blocage de déplacement verticaux quand sur une plateforme
    public bool grabed; //bool qui détermine si un personnage se fait grab, il ne peut alors pas bouger ni attaquer.
    public int hp; //int qui contient le nombre de vies des joueurs
    public bool attacking; //bool qui détermine si le joueur est en animation d'attaque
    public int hitstuncnt;

    public bool initmov;
    public bool initjump;

    private int temppercent; //int qui contient les pourcents de la frame d'avant pour pouvoir savoir quand le perso a pris des dégats pour pouvoir le hitstun

    public float grabbedframes;

    public bool grabbing;

    [Header("variables de clignotement lors d'iframe")]
    public int blinkcnt;
    public SpriteRenderer SR;
    public bool deblink;

    void Start()
    {
        SR= GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (grabbedframes>0f)
        {
            grabbedframes -= 1f;
            grabed = true;
        }
        else
        {
            grabed = false;
            grabbedframes = 0f;
        }

        if (this.CompareTag("Player2"))
        {
            foreach (GameObject O in GameObject.FindGameObjectsWithTag("Player1"))
            {
                if ( O.GetComponent<charavalues>().grabbedframes>0f)
                {
                    grabbing = true;
                }
                else
                {
                    grabbing= false;
                }
            }
        }
        else
        {
            foreach (GameObject O in GameObject.FindGameObjectsWithTag("Player2"))
            {
                if (O.GetComponent<charavalues>().grabbedframes > 0f)
                {
                    grabbing = true;
                }
                else
                {
                    grabbing = false;
                }
            }
        }

        if(grabed) //gère les animations si le perso se fait grab
        {
            GetComponent<Animator>().SetBool("grabbed",true);
        }
        else
        {
            GetComponent<Animator>().SetBool("grabbed", false);
        }

        if (grabbing) //gère les animations si le perso est en train de grab
        {
            GetComponent<Animator>().SetBool("grabbing", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("grabbing", false);
        }


        if (temppercent!=percent)
        {
            temppercent = percent;
            if(!grabed)
            {
                hitstuncnt = GameObject.Find("Global values").GetComponent<Globalvalues>().hitstun * (1 + percent / 50);
                GetComponent<Animator>().SetTrigger("hit");
            }
           
        }

        if (hitstuncnt > 0)
        {
            hitstuncnt--;
        }

        if (iframes > 0) //si on a des iframes, on diminue le nombre et on fait blink le joueur
        {
            iframes -= 1;
            if(blinkcnt < 10)
            {
                if (!deblink)
                {
                    SR.color = new Vector4(SR.color.r, SR.color.g, SR.color.b, SR.color.a*0.50f);
                    blinkcnt += 1;
                }
                else
                {
                    SR.color = new Vector4(SR.color.r, SR.color.g, SR.color.b, SR.color.a*1.750f);
                    blinkcnt += 1;
                }
                
            }
            else
            {
                blinkcnt = 0;
                if(deblink)
                {
                    deblink = false;
                }
                else
                {
                    deblink = true;
                }
            }
            
        }
        if (iframes <= 0) //si on a pas de iframes on remet la couleur normale
        {
            SR.color = new Vector4(SR.color.r, SR.color.g, SR.color.b, 255);
            blinkcnt = 0;
            deblink = false;
        }
       
    }
}
