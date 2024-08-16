using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HudHPBarController : MonoBehaviour
{
   [SerializeField] private NetworkManager networkManager;
   [SerializeField] private GameObject HPBar;
   [SerializeField] private GameObject ammoDisplay;
   [SerializeField] private GameObject croshair;

   private void Awake()
   {
      networkManager.OnClientConnectedCallback += NetworkManagerOnOnClientConnectedCallback;
   }

   private void Start()
   {
      HPBar.SetActive(false);
      ammoDisplay.SetActive(false);
      croshair.SetActive(false);

   }

   private void NetworkManagerOnOnClientConnectedCallback(ulong obj)
   {
      HPBar.SetActive(true);
      ammoDisplay.SetActive(true);
      croshair.SetActive(true);
   }
}
