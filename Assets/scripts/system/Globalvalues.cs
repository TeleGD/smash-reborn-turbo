using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class Globalvalues : MonoBehaviour
{
    //coordonn�es de respawn pour P1 et P2.
    [Header("Spawn/respawn")]
    public float xstartp1;
    public float ystartp1;
    public float xstartp2;
    public float ystartp2;

    [Header("death zone")]
    public float deathleft;
    public float deathright;
    public float deathup;
    public float deathdown;

    //variables concernant le shield
    [Header("Shield variables")]
    public int shieldmax; //correspond au bouclier maximal
    public int shielddimrate; //correspond � la diminution passive du shield lorsqu'il est actif
    public int shieldrecharge; //correspond � la vitesse de rechargement du bouclier
    public int shieldbreakCD; //correspond au temps pendant lequel le bouclier est inactif si il est cass�

    [Header("player variables")]
    public int playermaxhp; //nombre de vies des joueurs
    public int respawniframes;
    public string player1char;
    public string player2char;
    public Rigidbody2D rb2D1;
    public Rigidbody2D rb2D2;
    public float quickfallspeed;
    public float speedwhenoverplayer;
    public int grabtime; //nombre de frame de base de dur�e du grab. La dur�e est augment�e avec nbframe=grabtime*(1+percent/100)


    [Header("Health variables")]
    public int p1HP;
    public int p1maxHP;
    public int p2HP;
    public int p2maxHP;
    public int hitstun;
    public Healthbar P1healthbar;
    public Healthbar P2healthbar;



    public bool playable;


    // Update is called once per frame
    void Awake()
    {
        
        Application.targetFrameRate = 48;

        if (playable)
        {
            player2char = GameObject.Find("Optionvalues").GetComponent<optionvalues>().p2char;
            player1char = GameObject.Find("Optionvalues").GetComponent<optionvalues>().p1char;

            if (GameObject.Find("Optionvalues").GetComponent<optionvalues>().p1char == "Bobby")
            {
                GameObject.Find("Randy1").SetActive(false);

            }
            if (GameObject.Find("Optionvalues").GetComponent<optionvalues>().p1char == "Randy")
            {
                GameObject.Find("Bobby1").SetActive(false);

            }
            if (GameObject.Find("Optionvalues").GetComponent<optionvalues>().p2char == "Bobby")
            {
                GameObject.Find("Randy2").SetActive(false);

            }
            if (GameObject.Find("Optionvalues").GetComponent<optionvalues>().p2char == "Randy")
            {
                GameObject.Find("Bobby2").SetActive(false);
            }

        }

    }



    void Start()
    {
        p1maxHP = playermaxhp;
        p2maxHP = playermaxhp;
        p1HP = playermaxhp;
        p2HP = playermaxhp;
    }



    void Update()
    {

        Collider2D[] findplayers = Physics2D.OverlapCircleAll(new Vector2(0, 0), 100000); //va chercher tous les collider2D pr�sent sur la sc�ne

        foreach (Collider2D player in findplayers) //boucle for
        {
            if (player.tag == "Player1")
            {
                rb2D1 = player.GetComponent<Rigidbody2D>();

                if (rb2D1.position.x >= deathright || rb2D1.position.y <= deathdown || rb2D1.position.y >= deathup || rb2D1.position.x <= deathleft)
                {
                    if (p1HP > 1)
                    {
                        GameObject.Find("soundeffect").GetComponent<soundeffectmanager>().playdeath = true;
                        p1HP -= 1;
                        GameObject.Find("deathP1").transform.position = player.transform.position;
                        GameObject.Find("deathP1").GetComponent<Animator>().SetTrigger("playdeath");
                        player.transform.position = new Vector2(2, 2);
                        rb2D1.velocity = new Vector2(0, 0);
                        player.GetComponent<charavalues>().iframes = respawniframes;
                        player.GetComponent<charavalues>().percent = 0;

                    }
                    else
                    {
                        GameObject.Find("soundeffect").GetComponent<soundeffectmanager>().playdeath = true;
                        SceneManager.LoadScene("MainMenu");
                    }
                }
            }
            else if (player.tag == "Player2")
            {
                rb2D2 = player.GetComponent<Rigidbody2D>();

                if (rb2D2.position.x >= deathright || rb2D2.position.y <= deathdown || rb2D2.position.y >= deathup || rb2D2.position.x <= deathleft)
                {
                    if (p2HP > 1)
                    {
                        GameObject.Find("soundeffect").GetComponent<soundeffectmanager>().playdeath = true;
                        p2HP -= 1;
                        GameObject.Find("deathP2").transform.position = player.transform.position;
                        GameObject.Find("deathP2").GetComponent<Animator>().SetTrigger("playdeath");
                        player.transform.position = new Vector2(2, 2);
                        rb2D2.velocity = new Vector2(0, 0);
                        player.GetComponent<charavalues>().iframes = respawniframes;
                        player.GetComponent<charavalues>().percent = 0;

                    }
                    else
                    {
                        GameObject.Find("soundeffect").GetComponent<soundeffectmanager>().playdeath = true;
                        SceneManager.LoadScene("MainMenu");
                    }
                }
            }

        }


        
        


    }





}
