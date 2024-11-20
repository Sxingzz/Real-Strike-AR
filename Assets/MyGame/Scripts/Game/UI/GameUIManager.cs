using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameUIManager : NetworkBehaviour
{
    [SerializeField] private Canvas CreateGameCanvas;
    [SerializeField] private Canvas ControllerCanvas;
    [SerializeField] private Canvas RestartQuitCanvas;


    private void Start()
    {
        ShowCreateGameCanvas();
        AllPlayerDataManager.Instance.OnPlayerDead += Instance_OnPlayerDead;
        RestartGame.OnRestartGame += RestartGame_OnRestartGame;
    }

    private void RestartGame_OnRestartGame()
    {
        ShowPlayerControlsServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShowPlayerControlsServerRpc()
    {
        ShowPlayerControlsClientRpc();
    }

    [ClientRpc]
    private void ShowPlayerControlsClientRpc()
    {
        CreateGameCanvas.gameObject.SetActive(false);
        ControllerCanvas.gameObject.SetActive(true);
        RestartQuitCanvas.gameObject.SetActive(false);
    }

    private void Instance_OnPlayerDead(ulong obj)
    {
        if (IsServer)
        {
            PlayerIsDeadClientRpc();
        }
    }

    [ClientRpc]
    private void PlayerIsDeadClientRpc()
    {
        CreateGameCanvas.gameObject.SetActive(false);
        ControllerCanvas.gameObject.SetActive(false);
        RestartQuitCanvas.gameObject.SetActive(true);
    }

    private void ShowCreateGameCanvas()
    {
        CreateGameCanvas.gameObject.SetActive(true);
        ControllerCanvas.gameObject.SetActive(false);
        RestartQuitCanvas.gameObject.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        CreateGameCanvas.gameObject.SetActive(false);
        ControllerCanvas.gameObject.SetActive(true);
        RestartQuitCanvas.gameObject.SetActive(false);
    }

    public override void OnNetworkDespawn()
    {
        AllPlayerDataManager.Instance.OnPlayerDead -= Instance_OnPlayerDead;
        RestartGame.OnRestartGame -= RestartGame_OnRestartGame;
    }

}
