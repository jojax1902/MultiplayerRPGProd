using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Enemy : MonoBehaviourPun
{
    [Header("Info")]
    public string enemyName;
    public float moveSpeed;

    public int curHp;
    public int maxHp;

    public float chaseRange;
    public float attackRange;

    private PlayerController targetPlayer;

    public float playerDetectRate = 0.2f;
    private float lastPlayerDetectTime;

    public GameObject Low, Med, High, Health;
    public int drops = 1;
    public float rarity = 1;

    [Header("Attack")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;

    [Header("Components")]
    public HeaderInfo healthBar;
    public SpriteRenderer sr;
    public Rigidbody2D rig;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.Initialize(enemyName, maxHp);
      
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (targetPlayer != null)
        {
            // calculate the distance
            float dist = Vector3.Distance(transform.position, targetPlayer.transform.position);
            Debug.Log("dist to player = " + dist);

            // if we're able to attack, do so
            if (dist < attackRange && Time.time - lastAttackTime >= attackRange)
                Attack();
            // otherwise, do we move after the player?
            else if (dist > attackRange)
            {
                Vector3 dir = targetPlayer.transform.position - transform.position;
                rig.velocity = dir.normalized * moveSpeed;
            }
            else
            {
                rig.velocity = Vector2.zero;
            }
        }

        DetectPlayer();
    }

    // attacks the targeted player
    void Attack()
    {
        lastAttackTime = Time.time;
        targetPlayer.photonView.RPC("TakeDamage", targetPlayer.photonPlayer, damage);
    }

    // updates the targeted player
    void DetectPlayer()
    {
        if(Time.time - lastPlayerDetectTime > playerDetectRate)
        {
            // loop through all the players
            foreach (PlayerController player in GameManager.instance.players)
            {
                // calculate distance between us and the player
                float dist = Vector2.Distance(transform.position, player.transform.position);

                if (player == targetPlayer)
                {
                    if (dist > chaseRange)
                        targetPlayer = null;
                }
                else if (dist < chaseRange)
                {
                    if (targetPlayer == null)
                        targetPlayer = player;
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        curHp -= damage;

        // update the health bar
        healthBar.LocalUpdateHealthBar(curHp);

        if (curHp <= 0)
        {
            Die();
        }
        else
        {
            FlashDamage();
        }
    }

    [PunRPC]
    void FlashDamage()
    {
        StartCoroutine(DamageFlash());

        IEnumerator DamageFlash()
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            sr.color = Color.white;
        }
    }

    void Die()
    {
        for(int i = 1; i <+ drops; i++)
        {
            Vector3 randomInCircle = Random.insideUnitCircle * 1;
            int rand = Random.RandomRange(1, 100);
            if (rand < (50 * rarity))
            {
                Instantiate(Low, transform.position + randomInCircle, Quaternion.identity);
            }
            else if (rand > (50 * rarity) && rand < (75 * rarity))
            {
                Instantiate(Med, transform.position + randomInCircle, Quaternion.identity);
            }
            else if (rand > (75 * rarity) && rand > (95 * rarity))
            {
                Instantiate(High, transform.position + randomInCircle, Quaternion.identity);
            }
            else
            {
                Instantiate(Health, transform.position + randomInCircle, Quaternion.identity);
            }
        }
        

        Destroy(gameObject);
    }
}
