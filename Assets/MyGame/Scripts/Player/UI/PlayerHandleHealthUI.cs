using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class PlayerHandleHealthUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text HealthText; 

    private Camera _mainCamera;


    public override void OnNetworkSpawn()
    {
        _mainCamera = GameObject.FindObjectOfType<Camera>();
        AllPlayerDataManager.Instance.OnPlayerHealthChanged +=
                                               InstanceOnPlayerHealthChangedServerRpc;

        InstanceOnPlayerHealthChangedServerRpc(GetComponentInParent<NetworkObject>().
                                                                        OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InstanceOnPlayerHealthChangedServerRpc(ulong id)
    {
        if (GetComponentInParent<NetworkObject>().OwnerClientId == id)
        {
            SetHealthTextClientRpc(id);
        }
    }

    [ClientRpc]
    private void SetHealthTextClientRpc(ulong id)
    {
        HealthText.text = AllPlayerDataManager.Instance.GetPlayerHealth(id).ToString();
    }

    private void Update()
    {
        if (_mainCamera)
        {
            HealthText.transform.LookAt(_mainCamera.transform);
        }
    }

    public override void OnNetworkDespawn()
    {
        AllPlayerDataManager.Instance.OnPlayerHealthChanged -=
                                               InstanceOnPlayerHealthChangedServerRpc;
    }



}
