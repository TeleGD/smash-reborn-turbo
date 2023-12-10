using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class proj : MonoBehaviour
{

    public string sendertag;
    public float projspeed;
    public float projdirection; //=1 si vers la droite, =-1 si vers la gauche
    public float maxdistance;
    public float startx;
    public int percent;
    public float baserecoil;
    public float fixedrecoil;


    // Update is called once per frame
    void Update()
    {

        if(projdirection == 1 && transform.localScale.x<0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        if (projdirection == -1 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }


        if ((projdirection==1 && transform.position.x>startx+maxdistance)||(projdirection == -1 && transform.position.x < startx - maxdistance))
        {
            this.gameObject.SetActive(false);

        }
        GetComponent<Rigidbody2D>().velocity = new Vector2(projdirection*projspeed,0);

        Collider2D[] hitenemies = Physics2D.OverlapBoxAll(transform.position,new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y),0);

        foreach (Collider2D enemy in hitenemies)
        {
            if (((sendertag=="Player2" && enemy.tag == "Player1") || (sendertag == "Player1" && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0) 
            {
                enemy.GetComponent<charavalues>().percent +=percent;

                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(projdirection * (fixedrecoil + baserecoil* enemy.GetComponent<charavalues>().percent),20));

                this.gameObject.SetActive(false);
            }
        }

    }

}
