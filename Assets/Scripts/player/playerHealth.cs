using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class playerHealth : NetworkBehaviour
{

    public float MaxHP;
    [NonSerialized] NetworkVariable<float>  PlayerHP;
    [SerializeField] private Image HPBar;
    private Image HudHealthBar;
    public bool CanTakeDamage = false;

    [SerializeField] private GameObject PlayerOBJ;

    private NetworkManager networkManager;

    [SerializeField] private GameObject HudHPBarOBJ;
    
    
    private void Awake()
    {
        PlayerHP = new NetworkVariable<float>(MaxHP);
    }


    public void resetHP()
    {
        PlayerHP.Value = MaxHP;
        PlayerOBJ.SetActive(true);
    }

    
    public void Damage(float damage)
    {
        if (CanTakeDamage)
        {
            PlayerHP.Value -= damage;
        }
    }

    private void updatehealthBar()
    {
        HudHealthBar.fillAmount = PlayerHP.Value / MaxHP;
    }
    // Start is called before the first frame update
    void Start()
    {
        
        HudHealthBar = GameObject.FindWithTag("HPBarImage").GetComponent<Image>();
        HudHealthBar.fillAmount = PlayerHP.Value / MaxHP;
        PlayerHP.OnValueChanged += OnValueChanged;
    }

    private void OnValueChanged(float previousvalue, float newvalue)
    {
        HPBar.fillAmount = PlayerHP.Value / MaxHP;
        if (IsLocalPlayer)
        {
            updatehealthBar();
        }

        if (PlayerHP.Value <= 0)
        {
            gameObject.GetComponent<player>().CanMove = false;
            int PlayerId = (int) gameObject.GetComponent<NetworkObject>().NetworkObjectId;
            GameManager.Instance.RoundEnd(PlayerId);
        }
    }

    [Rpc(SendTo.Server)]
    private void StartRPC()
    {
        HPBar.fillAmount = PlayerHP.Value / MaxHP;
    }

    


}
