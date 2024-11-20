using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class RestartGame : NetworkBehaviour
{
    [SerializeField] private Button restartButton;

    public static event Action OnRestartGame;

    private void Start()
    {
        restartButton.onClick.AddListener(() =>
        {
            RestartGameServerRpc();
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void RestartGameServerRpc()
    {
        RestartGameClientRpc();
    }

    [ClientRpc]
    private void RestartGameClientRpc()
    {
        OnRestartGame?.Invoke();
    }

}
