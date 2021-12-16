using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class EnemySpawner : MonoBehaviourPun
{
    public string basicEnemyPrefabPath;
    public string rareEnemyPrefabPath;
    public string bossEnemyPrefabPath;
    public float maxEnemies;
    public float spawnRadius;
    public float spawnCheckTime;

    public Teleport tp;

    public TextMeshProUGUI waveCounter;
    public TextMeshProUGUI timer;

    public int waveMax;
    public int currWave = 0;
    public int ttnw = 30;

    public Enemy e;

    public PlayerController target;


    private List<GameObject> curEnemies = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        waveCounter.text = currWave + "/" + waveMax;
        if(ttnw < 10)
        {
            timer.color = Color.red;
            timer.text = "00:0" + ttnw;
        }
        else
        {
            timer.color = Color.white;
            timer.text = "00:" + ttnw;
        }
        


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            target = collision.gameObject.GetComponent<PlayerController>();
            WaveStart();
        }

    }


    public void WaveStart()
    {

            if(curEnemies.Count == 0)
            {
                CancelInvoke();
                currWave++;
                ttnw = 30;
                nextWave(currWave);

            }
        tp.activated = true;
    }

    public void nextWave(int i)
    {
        InvokeRepeating("timeDown", 1, 1);
        if (i < waveMax / 3)
        {
            TrySpawn(i,0,0);
        }
        else if (i > waveMax/3 && i < waveMax * 2 / 3)
        {
            TrySpawn(i,(int)i/2,0);
        }
        else
        {
            TrySpawn(0,0,1);
        }
        
    }



    public void timeDown()
    {
        if (ttnw == 0)
        {
            CancelInvoke();
            ttnw = 30;
            currWave++;
        }
        ttnw--;
        

    }

    void TrySpawn(int b, int r, int boss)
    {
        for (int x = 0; x < curEnemies.Count; ++x)
        {
            if (!curEnemies[x])
                curEnemies.RemoveAt(x);
        }

        for (b = b; b > 0; b--)
        {
            Vector3 randomInCircle = Random.insideUnitCircle * spawnRadius;
            GameObject enemy = PhotonNetwork.Instantiate(basicEnemyPrefabPath, transform.position + randomInCircle, Quaternion.identity);
            e = enemy.GetComponent<Enemy>();
            e.chaseRange *= target.drag;
            e.moveSpeed *= target.stealth;
            e.drops *= target.looting;
            e.rarity *= target.luck;
            curEnemies.Add(enemy);
        }
        for (r = r; r > 0; r--)
        {
            Vector3 randomInCircle = Random.insideUnitCircle * spawnRadius;
            GameObject enemy = PhotonNetwork.Instantiate(rareEnemyPrefabPath, transform.position + randomInCircle, Quaternion.identity);
            e = enemy.GetComponent<Enemy>();
            e.chaseRange *= target.drag;
            e.moveSpeed *= target.stealth;
            e.drops *= target.looting * 2;
            e.rarity *= target.luck;
            curEnemies.Add(enemy);
        }
        for (boss = boss; boss > 0; boss--)
        {
            Vector3 randomInCircle = Random.insideUnitCircle * spawnRadius;
            GameObject enemy = PhotonNetwork.Instantiate(bossEnemyPrefabPath, transform.position + randomInCircle, Quaternion.identity);
            e = enemy.GetComponent<Enemy>();
            e.chaseRange *= target.drag;
            e.moveSpeed *= target.stealth;
            e.drops *= target.looting * 3;
            e.rarity *= target.luck;
            curEnemies.Add(enemy);
        }

    }
}
