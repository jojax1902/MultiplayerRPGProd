using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoonControl : MonoBehaviour
{
    public enum BoonName
    {
        Elephant,
        Prayer,
        Phantom,
        Snail,
        Demon,
        Cleaver,
        Blade,
        Harvester
    }

    public BoonName myBoon;

    public BoxCollider2D inSpace;

    [Header("UI Stuff")]
    public GameObject myUI;
    public TextMeshProUGUI cost;
    public Button upgrade;
    public TextMeshProUGUI level;

    public string levelValue = "0";
    public int upgradeCost = 999999;
    public PlayerController pc;
    // Start is called before the first frame update
    void Start()
    {
        myUI.SetActive(false);
        upgrade.interactable = false;


    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            pc = collision.gameObject.GetComponent<PlayerController>();
            myUI.SetActive(true);

            switch (myBoon)
            {
                case BoonName.Elephant: //Resist damage
                    upgradeCost = (int)100 + (int)Mathf.Exp(pc.eLevel);
                    levelValue = pc.eLevel.ToString();
                    break;
                case BoonName.Prayer: //Heal more
                    upgradeCost = (int)100 + (int)Mathf.Exp(pc.pLevel);
                    levelValue = pc.pLevel.ToString();
                    break;
                case BoonName.Phantom: //Draw less attention
                    upgradeCost = (int)100 + (int)Mathf.Exp(pc.phLevel);
                    levelValue = pc.phLevel.ToString();
                    break;
                case BoonName.Snail: //Enemies are slower
                    upgradeCost = (int)100 + (int)Mathf.Exp(pc.sLevel);
                    levelValue = pc.sLevel.ToString();
                    break;
                case BoonName.Demon: //Pain = power
                    upgradeCost = (int)100 + (int)Mathf.Exp(pc.dLevel);
                    levelValue = pc.dLevel.ToString();
                    break;
                case BoonName.Cleaver: //Knock expensive things from your victims
                    upgradeCost = (int)100 + (int)Mathf.Exp(pc.cLevel);
                    levelValue = pc.cLevel.ToString();
                    break;
                case BoonName.Blade: //Deal more damage
                    upgradeCost = (int)100 + (int)Mathf.Exp(pc.bLevel);
                    levelValue = pc.bLevel.ToString();
                    break;
                case BoonName.Harvester: //Enemies drop more loot
                    upgradeCost = (int)100 + (int)Mathf.Exp(pc.hLevel);
                    levelValue = pc.hLevel.ToString();
                    break;
            }
            
            
            
        }
    }

    public void UpgradeBoon()
    {
        switch (myBoon) 
        {
            case BoonName.Elephant: //Resist damage
                if (pc.gold >= 100 + Mathf.Exp(pc.eLevel))
                {
                    pc.photonView.RPC("GiveGold", pc.photonPlayer, (int)(100 + Mathf.Exp(pc.eLevel)));
                    pc.eLevel++;
                    pc.resistance -= .1f;
                }
                else
                {

                }
                break;
            case BoonName.Prayer: //Heal more
                if (pc.gold >= 100 + Mathf.Exp(pc.pLevel))
                {
                    pc.photonView.RPC("GiveGold", pc.photonPlayer, (int)(100 + Mathf.Exp(pc.pLevel)));
                    pc.pLevel++;
                    pc.healEffect += .5f;
                }
                else
                {

                }
                break;
            case BoonName.Phantom: //Draw less attention
                if (pc.gold >= 100 + Mathf.Exp(pc.phLevel))
                {
                    pc.photonView.RPC("GiveGold", pc.photonPlayer, (int)(100 + Mathf.Exp(pc.phLevel)));
                    pc.phLevel++;
                    pc.stealth -= .2f;
                }
                else
                {

                }
                break;
            case BoonName.Snail: //Enemies are slower
                if (pc.gold >= 100 + Mathf.Exp(pc.sLevel))
                {
                    pc.photonView.RPC("GiveGold", pc.photonPlayer, (int)(100 + Mathf.Exp(pc.sLevel)));
                    pc.sLevel++;
                    pc.drag -= .1f;
                }
                else
                {

                }
                break;
            case BoonName.Demon: //Pain = power
                if (pc.gold >= 100 + Mathf.Exp(pc.dLevel))
                {
                    pc.photonView.RPC("GiveGold", pc.photonPlayer, (int)(100 + Mathf.Exp(pc.dLevel)));
                    pc.dLevel++;
                    pc.vengence +=.1f;

                }
                else
                {

                }
                break;
            case BoonName.Cleaver: //Knock expensive things from your victims
                if (pc.gold >= 100 + Mathf.Exp(pc.cLevel))
                {
                    pc.photonView.RPC("GiveGold", pc.photonPlayer, (int)(100 + Mathf.Exp(pc.cLevel)));
                    pc.cLevel++;
                    pc.luck += .1f;
                }
                else
                {

                }
                break;
            case BoonName.Blade: //Deal more damage
                if (pc.gold >= 100 + Mathf.Exp(pc.bLevel))
                {
                    pc.photonView.RPC("GiveGold", pc.photonPlayer, (int)(100 + Mathf.Exp(pc.bLevel)));
                    pc.bLevel++;
                    pc.damage++;
                }
                else
                {

                }
                break;
            case BoonName.Harvester: //Enemies drop more loot
                if (pc.gold >= 100 + Mathf.Exp(pc.hLevel))
                {
                    pc.photonView.RPC("GiveGold", pc.photonPlayer, (int)(100 + Mathf.Exp(pc.hLevel)));
                    pc.hLevel++;
                    pc.looting += 1;
                }
                else
                {

                }
                break;


        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        myUI.SetActive(false);
    }






    // Update is called once per frame
    void Update()
    {
        cost.text = "Cost: " + upgradeCost + "Gp";
        level.text = levelValue;
        if (upgradeCost != 999999)
        {
            if (pc.gold >= upgradeCost)
            {
                upgrade.interactable = true;
            }
            else
            {
                upgrade.interactable = false;
            }
        }
        
    }
}
