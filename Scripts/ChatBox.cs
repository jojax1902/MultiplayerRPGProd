using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;

public class ChatBox : MonoBehaviourPun
{
    public TextMeshProUGUI chatLogText;
    public TMP_InputField chatInput;

    public int index = 0;

    public string[] mono = new string[11] 
    { 
    "Welcome to the dungeon",
    "You have been brought here to pay off your student loan debt",
    "To the left, you may test you luck in lottery machines...",
    "To the right, you may test your might in dungeon trials...",
    "Above, you may sacrifice your precous blood for small bits of money",
    "Below lies the path to the end, collect your dept and pay it off to escape",
    "[In the casino you can find boons to purchase and upgrade your fighting]",
    "[The dungeons bear an incredible treasure if you can survive to the white room]",
    "[If you perish, you will lose half of your gold in order to be revived]",
    "[The dungeon is full of dangerous monsters and the casino isn't much safer so take risks only if they are worth the price...]",
    "Good luck."
    };
    
    // instance
    public static ChatBox instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int x = 0; x < 11; x++)
        {
            Invoke("monolague", 2f);
        }
            
    }

    // called when the player wants to send a message
    public void OnChatInputSend()
    {
        if (chatInput.text.Length > 0)
        {
            photonView.RPC("Log", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, chatInput.text);
            chatInput.text = "";
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    [PunRPC]
    void Log(string playerName, string message)
    {
        chatLogText.text += string.Format("<br>{0}:</b> {1}", playerName, message);
        chatLogText.rectTransform.sizeDelta = new Vector2(chatLogText.rectTransform.sizeDelta.x, chatLogText.mesh.bounds.size.y + 20); // ?
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (EventSystem.current.currentSelectedGameObject == chatInput.gameObject)
                OnChatInputSend();
            else
                EventSystem.current.SetSelectedGameObject(chatInput.gameObject);
        }
    }

    public void monolague()
    {
        photonView.RPC("Log", RpcTarget.All, "Master", mono[index]);
        index++;
    }
}
