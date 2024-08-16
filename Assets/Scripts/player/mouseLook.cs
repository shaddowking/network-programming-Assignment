using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;


public class mouseLook : NetworkBehaviour
{
    [SerializeField] private GameObject Player;
    
    [SerializeField] private float sensX;
    [SerializeField] private float SensY;

    [SerializeField] private Transform Orientation;
    [SerializeField] private Transform playerOBJ;
    
    //network
    private NetworkVariable<Quaternion> rotationInput = new NetworkVariable<Quaternion>();

    private float xRotation;
    private float yRotation;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
     
        
        if (IsLocalPlayer && Player.GetComponent<player>().CanMove)
        {
            Rotation();
        }
    }

    private void Rotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime  ;
        float mouseY = Input.GetAxisRaw("Mouse Y") * SensY * Time.deltaTime  ;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 80f);
            
        Orientation.rotation = Quaternion.Euler(0,yRotation,0);
        Quaternion playerObjRotation = Quaternion.Euler(0,yRotation,0);
        Quaternion cameraRotation = Quaternion.Euler(xRotation,yRotation,0);
        RotateRPC(playerObjRotation,cameraRotation);
    }
    
    [Rpc(SendTo.Server)]
    private void RotateRPC(Quaternion data, Quaternion CamData)
    {
        transform.rotation = CamData;
        playerOBJ.rotation = data;
    }
}
