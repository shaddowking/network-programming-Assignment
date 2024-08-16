using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class bullet : NetworkBehaviour
{
    [Rpc(SendTo.ClientsAndHost)]
    public void AddForceRPC(Vector3 directionWithspred, float ShootForece)
    {
        if (IsHost)
        {
            return;
        }
            
            
                gameObject.GetComponent<Rigidbody>()
                    .AddForce(directionWithspred.normalized * ShootForece, ForceMode.Impulse);
            
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!NetworkManager.Singleton.IsServer && !NetworkObject.IsSpawned)
        {
            Debug.Log("Error");
            return;
        }
        if (other.gameObject.TryGetComponent(out playerHealth playerHealth))
        {
            playerHealth.Damage(10);
        }

        if (NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn(true);
        }
        
    }
}