using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class player : NetworkBehaviour
{

    [SerializeField] private CharacterController controller;

    [SerializeField] private float speed = 6f;
    

    [SerializeField] private Transform orientation;
    private Camera EnemyCam;
    
    //NetWork
    private NetworkVariable<Vector3> moveInput = new NetworkVariable<Vector3>();
    private Camera playerCam;

    [NonSerialized] public  bool CanMove = true;
    private bool isusingchat =false;
    private void Awake()
    {
        playerCam = gameObject.transform.GetChild(1).GetComponent<Camera>();
        playerCam.enabled = false;
    }

    void Start()
    {
        if (IsLocalPlayer)
        {
            playerCam.enabled = true;
            
        }
        else
        {
            EnemyCam = playerCam;
        }

        if (IsServer)
        {
            GameManager.Instance.UpdatePlayerList(gameObject);
        }
    }
    
    
    
    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (moveInput.Value != Vector3.zero)
            {
                controller.Move(moveInput.Value * speed * Time.deltaTime);
            }
            
        }

        if (IsLocalPlayer && CanMove)
        {
            PlayerMovement();
        }

        if (IsLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.G) && isusingchat == false)
            {
                isusingchat = true;
                CanMove = false;
            }

            if (Input.GetKeyDown(KeyCode.Return) && isusingchat)
            {
                isusingchat = false;
                CanMove = true;
            }
        }
    }

    private void PlayerMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = orientation.forward * vertical + orientation.right * horizontal;
        direction += Vector3.down;
        MoveRPC(direction);
        
    }

    [Rpc(SendTo.Server)]
    private void SpawnRPC()
    { 
        //NetworkObject ob = Instantiate(ObjectToSpawn).GetComponent<NetworkObject>();
        //ob.Spawn();
    }

    [Rpc(SendTo.Server)]
    private void MoveRPC(Vector3 data)
    {
        moveInput.Value = data;
    }
    
}
