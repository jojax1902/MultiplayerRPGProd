using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class Teleport : MonoBehaviour
{
    public bool lastStage;
    public bool stageUP;
    public bool toHub;

    public bool activated = true;
    
    public List<Transform> options;


    public Transform destination;
    public Transform hub;
    public Transform haven;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (activated)
        {
            if (collision.gameObject.tag == "Player")
            {
                move(collision.gameObject);
                if (lastStage)
                {
                    collision.gameObject.GetComponent<PlayerController>().dungeonLevel++;
                }
            }
        }
        
    }

    public void move(GameObject g)
    {
        if (lastStage)
        {
            g.transform.position = haven.position;
        }
        if (stageUP)
        {
            g.transform.position = options[g.GetComponent<PlayerController>().dungeonLevel].position;
        }
        else
        {
            g.transform.position = destination.position;
        }
        if (toHub)
        {
            g.GetComponent<PlayerController>().dungeonLevel = 0;
            g.transform.position = hub.position;
        }
    }
}
