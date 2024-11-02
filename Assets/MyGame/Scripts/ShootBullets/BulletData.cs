using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class BulletData : NetworkBehaviour
{
    private NetworkVariable<ulong> owner = new(999);
    private NetworkVariable<bool> isActiveSelf = new(true);

    private const int MAX_FLY_TIME = 3;

    public static event Action<(ulong from, ulong to)> OnHitPlayer;

    [ServerRpc(RequireOwnership = false)]
    public void SetOwnershipServerRpc(ulong id)
    {
        this.owner.Value = id;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetBulletIsActiveServerRpc(bool isActive)
    {
        isActiveSelf.Value = isActive;

        if (isActive == false)
        {
            GetComponent<NetworkObject>().Despawn();
        }
        else
        {
            GetComponent<NetworkObject>().Spawn();
        }
    }

    public void DeactivateSelfDelay()
    {
        StartCoroutine(DeactivateSelfDelayCoroutine());
    }

    IEnumerator DeactivateSelfDelayCoroutine()
    {
        yield return new WaitForSeconds(MAX_FLY_TIME);
        SetBulletIsActiveServerRpc(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            if (collision.transform.TryGetComponent(out NetworkObject networkObject))
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    (ulong, ulong) fromShooterToHit = new(owner.Value, 
                                                        networkObject.OwnerClientId);

                    OnHitPlayer?.Invoke(fromShooterToHit);

                    SetBulletIsActiveServerRpc(false);
                }
            }
            SetBulletIsActiveServerRpc(false);
        }
    }



}
