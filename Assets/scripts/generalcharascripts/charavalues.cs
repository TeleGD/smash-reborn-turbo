using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class charavalues : MonoBehaviour
{
    [Header("variables globales")]
    public bool shielded; //d�termine si le bouclier est activ�
    public int shield; //point de bouclier restant
    public int percent; //pourcents du joueur
    public int iframes; //nombres de frames d'invincibilit�s restantes
    public bool upb; //bool qui d�termine quand on active le upb. Il sert � d�sactiver le blocage de d�placement verticaux quand sur une plateforme
    public bool touch�; //bool qui d�termine quand un personnage est touch�. Il peut �tre utilis� pour une attaque qui envoit vers le haut. Il sert � d�sactiver le blocage de d�placement verticaux quand sur une plateforme
    public bool grabed; //bool qui d�termine si un personnage se fait grab, il ne peut alors pas bouger ni attaquer.
    public int hp; //int qui contient le nombre de vies des joueurs
    public bool attacking; //bool qui d�termine si le joueur est en animation d'attaque
    public int hitstuncnt;

    private int temppercent; //int qui contient les pourcents de la frame d'avant pour pouvoir savoir quand le perso a pris des d�gats pour pouvoir le hitstun

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

        if(grabed) //g�re les animations si le perso se fait grab
        {
            GetComponent<Animator>().SetBool("grabbed",true);
        }
        else
        {
            GetComponent<Animator>().SetBool("grabbed", false);
        }

        if (grabbing) //g�re les animations si le perso est en train de grab
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
            hitstuncnt = GameObject.Find("Global values").GetComponent<Globalvalues>().hitstun;
            GetComponent<Animator>().SetTrigger("hit");
            GetComponent<Rigidbody2D>().velocity=new Vector2(0f,0f);
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
