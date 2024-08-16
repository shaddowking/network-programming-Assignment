using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class chat : NetworkBehaviour
{

    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private int maxChatLengh;
    private List<GameObject> ChatList = new List<GameObject>();
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private GameObject TextTemplate;
    [SerializeField] private GameObject ChatContent;
    private GameObject CurentChatMesage;
    private bool IsChatActive = false;
    private bool CanUseChat = false;
    private int playerid;
    [SerializeField] private GameObject ChatHolder;



    private void Start()
    {
        chatInput.interactable = false;
        ChatHolder.SetActive(false);
        networkManager.OnClientConnectedCallback += NetworkManagerOnOnClientConnectedCallback;
    }

    private void NetworkManagerOnOnClientConnectedCallback(ulong obj)
    {
        ChatHolder.SetActive(true);
        CanUseChat = true;
        if (IsServer)
        {
            int playerNetworkID = (int)obj;
            int playerNumber = playerNetworkID + 1;
            FixedString128Bytes PlayerJoinMesage = "player " + playerNumber + " joind";
            SubmittMessageRPC(PlayerJoinMesage,PlayerJoinMesage);
        }

      
        
    }
    
   

    private void Update()
    {

        if (IsChatActive == false && CanUseChat)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                IsChatActive = true;
                chatInput.interactable = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                
                chatInput.Select();
            }
        }
        
        if (IsChatActive)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {

                FixedString128Bytes message =   "Player" + " > " + chatInput.text;
                

                FixedString128Bytes origenalmessage = chatInput.text;
                SubmittMessageRPC(message,origenalmessage);
                chatInput.text = "";
                chatInput.interactable = false;
                IsChatActive = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

            }         
        }
    }
    
    

    [Rpc(SendTo.Server)]
    public void SubmittMessageRPC(FixedString128Bytes message,FixedString128Bytes origenalmessage)
    {
        UpdateMessageRPC(message,origenalmessage);
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateMessageRPC(FixedString128Bytes message,FixedString128Bytes origenalmessage)
    {

        if (string.IsNullOrWhiteSpace(origenalmessage.ToString()))
        {
            return;
        }

        ChatHolder.SetActive(true);
        CurentChatMesage = Instantiate(TextTemplate, ChatContent.transform);
        CurentChatMesage.GetComponent<TMP_Text>().text = message.ToString();
        ChatList.Add(CurentChatMesage);
        if (ChatList.Count > maxChatLengh)
        {
            GameObject temp = ChatList[0];
            ChatList.Remove(ChatList[0]);
            Destroy(temp);
        }



    }
    
    
}
