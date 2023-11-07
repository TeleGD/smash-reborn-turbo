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
    public int hp;

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
        if (iframes > 0)
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
        if (iframes <= 0)
        {
            SR.color = new Vector4(SR.color.r, SR.color.g, SR.color.b, 255);
            blinkcnt = 0;
            deblink = false;
        }
       
    }
}
