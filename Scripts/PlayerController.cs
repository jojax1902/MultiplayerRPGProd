using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [HideInInspector]
    public int id;
    [Header("Boons")]
    public int  eLevel = 0,
                pLevel = 0,
                phLevel = 0,
                sLevel = 0,
                dLevel = 0,
                cLevel = 0,
                bLevel = 0,
                hLevel = 0;

    [Header("Info")]
    public float moveSpeed;
    public int gold;
    public int curHp;
    public int maxHp;
    public float resistance = 1; //will lower by .1
    public float healEffect = 1; //will rise by .5
    public float stealth = 1; //will lower by .2
    public float drag = 1; //will lower by .1
    public int looting = 1; // will raise by 1
    public float luck = 1; //will lower by .1
    public float vengence = 0; //will raise by 1
    public bool dead;

    public int dungeonLevel = 0;

    [Header("Attack")]
    public int damage;
    public float attackRange;
    public float attackRate;
    private float lastAttackTime;

    [Header("Components")]
    public Rigidbody2D rig;
    public Player photonPlayer;
    public SpriteRenderer sr;
    public Animator weaponAnim;
    public HeaderInfo headerInfo;
    public Camera mc;

    // local player
    public static PlayerController me;

    // Start is called before the first frame update
    void Start()
    {
        mc = FindObjectOfType<Camera>();
        this.photonView.RPC("GiveGold", this.photonPlayer, (int)(1000));
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
            return;


        Move();

        damage = 10 + (int)((maxHp - curHp) * vengence);

        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackRate)
            Attack();

        float mouseX = (Screen.width / 2) - Input.mousePosition.x;

        if (mouseX < 0)
            weaponAnim.transform.parent.localScale = new Vector3(1, 1, 1);
        else
            weaponAnim.transform.parent.localScale = new Vector3(-1, 1, 1);
    }

    void Move()
    {
        // get the horizontal and vertical inputs
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // apply that to our velocity
        rig.velocity = new Vector2(x, y) * moveSpeed;
    }

    // melee attacks towards the mouse
    void Attack()
    {
        lastAttackTime = Time.time;

        // calculate the direction
        Debug.Log(Input.mousePosition);
        Debug.Log(mc.WorldToScreenPoint(this.transform.position));
        Vector3 dir = (Input.mousePosition - mc.WorldToScreenPoint(this.transform.position)).normalized;
        Debug.Log(dir);

        // shoot a raycast in the direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position + dir, dir, attackRange);

        // did we hit an enemy?
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            // get the enemy and damage them
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
        }

        // play attack animation
        weaponAnim.SetTrigger("Attack");
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        curHp -= (int)(damage * resistance);

        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);

        if (curHp <= 0)
            Die();
        else
        {
            StartCoroutine(DamageFlash());
            IEnumerator DamageFlash()
            {
                sr.color = Color.red;
                yield return new WaitForSeconds(0.05f);
                sr.color = Color.white;
            }
        }
    }

    void Die()
    {
        dead = true;
        rig.isKinematic = true;

        transform.position = new Vector3(0, 99, 0);

        Vector3 spawnPos = GameManager.instance.spawnPoints[Random.Range(0, GameManager.instance.spawnPoints.Length)].position;

        StartCoroutine(Spawn(spawnPos, GameManager.instance.respawnTime));
    }

    IEnumerator Spawn(Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);

        dead = false;
        transform.position = spawnPos;
        curHp = maxHp;
        rig.isKinematic = false;

        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.instance.players[id - 1] = this;

        // initialize the health bar
        headerInfo.Initialize(player.NickName, maxHp);

        if (player.IsLocal)
            me = this;
        else
            rig.isKinematic = true; // turn off physics on other players so we don't process their collisions
    }

    [PunRPC]
    void Heal(int amountToHeal)
    {
        curHp = Mathf.Clamp(curHp + amountToHeal, 0, maxHp);

        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }

    [PunRPC]
    void GiveGold(int goldToGive)
    {
        gold += goldToGive;

        // update the ui
        GameUI.instance.UpdateGoldText(gold);
    }
}
