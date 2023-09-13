using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("movement")]
    public float detectdist;
    public float xspeed;
    private Transform target;
    private Rigidbody2D rb2D;

    [Header("HP & energy")]
    public int enemyHP;
    private int tempenemyhp;
    public int hitdelay;
    public float energystunduration;
    public int delaycounter;

    [Header("Attack")]
    public float attackrange;
    public float abandonrange;
    public bool cannotmove;
    public bool cannotmoveatk;
    public bool targetted;
    private bool walkingright = false;
    private bool lookingright = false;
    private bool initiateattack;
    private float distplayer;
    private float attackcounter=1;
    public float timebeforejump;
    public float jumpforcex;
    public float jumpforcey;
    private float atkcdcounter;
    public float atkcd;

    [Header("startpos")]
    //starting position for respawn
    public float startx;
    public float starty;




    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb2D = transform.GetComponent<Rigidbody2D>();
        enemyHP = this.GetComponent<EnemyHP>().enemyhp;
        tempenemyhp = enemyHP;
        startx = GetComponent<Rigidbody2D>().position.x;
        starty = GetComponent<Rigidbody2D>().position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (targetted && distplayer>= abandonrange || enemyHP <= 0)
        {
            targetted = false;
            GameObject.Find("music").GetComponent<musicmanager>().playcbt = false;
        }

        if (targetted)
        {
            GameObject.Find("music").GetComponent<musicmanager>().playcbt = true;
        }

        if (atkcdcounter != 0)
        {
            atkcdcounter -= 1;
        }

        if (walkingright && !lookingright)
        {
            GetComponent<SpriteRenderer>().flipX=true;
            lookingright = true;
        }
        if (!walkingright && lookingright)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            lookingright = false;
        }

        enemyHP = GetComponent<EnemyHP>().enemyhp;
        distplayer = Vector2.Distance(target.position, transform.position);


        if (tempenemyhp != enemyHP)
        {
            delaycounter = hitdelay;
            tempenemyhp = enemyHP;
        }

        if (delaycounter!=0)
        {
            delaycounter -= 1;
        }

        if (distplayer<=detectdist || targetted)
        {
            targetted = true;
            if (delaycounter == 0 & !cannotmove & !cannotmoveatk)
            {
                if (target.position.x < transform.position.x)
                {
                    if (rb2D.velocity.x> -xspeed)
                    {
                        rb2D.velocity = new Vector2(rb2D.velocity.x - xspeed * 0.08f, 0);
                    }
                    walkingright = false;
                }
                else
                {
                    if (rb2D.velocity.x < xspeed)
                    {
                        rb2D.velocity = new Vector2(rb2D.velocity.x + xspeed * 0.08f, 0);
                    }
                    walkingright = true;
                }
            }
            
        }

        if (targetted && distplayer < attackrange && !initiateattack && atkcdcounter==0)
        {
            cannotmoveatk = true;
            attackcounter = timebeforejump;
            initiateattack = true;

        }

        if (initiateattack && attackcounter!=0)
        {
            attackcounter -= 1;
        }

        if (initiateattack && attackcounter == 0)
        {
            if (target.position.x < transform.position.x)
            {
                rb2D.AddForce(new Vector2(-jumpforcex, jumpforcey));
            }
            if (target.position.x > transform.position.x)
            {
                rb2D.AddForce(new Vector2(jumpforcex, jumpforcey));
            }

            initiateattack = false;
            cannotmoveatk=false;
            atkcdcounter = atkcd;
        }


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectdist);
    }

    
}

