using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    public int minimumBet;

    public GameObject UI;

    public TextMeshProUGUI winnings;
    public Button spin;

    public PlayerController pc;

    public List<Sprite> objects;

    public Image slotOne;
    public Image slotTwo;
    public Image slotThree;

    private void Awake()
    {
        UI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            UI.SetActive(true);
            pc = collision.gameObject.GetComponent<PlayerController>();
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        UI.SetActive(false);
    }

    public void Update()
    {
        if(pc != null)
        {
            if (pc.gold < minimumBet)
            {
                spin.interactable = false;
            }
            else
            {
                spin.interactable = true;
            }
        }
        
        
    }

    public void spinIt()
    {
        int multi = 1;
        pc.photonView.RPC("GiveGold", pc.photonPlayer, (int)(minimumBet * -1));
        slotOne.sprite = objects[Random.RandomRange(0, objects.Count)];
        slotTwo.sprite = objects[Random.RandomRange(0, objects.Count)];
        slotThree.sprite = objects[Random.RandomRange(0, objects.Count)];

        if (slotOne.sprite == slotTwo.sprite == slotThree.sprite)
        {
            foreach(Sprite s in objects)
            {
                multi *= 5;
                if (s == slotOne.sprite)
                {
                    break;
                }
            }
            pc.photonView.RPC("GiveGold", pc.photonPlayer, (int)(minimumBet * multi));
        }
    }
}
