using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using static UnityEditor.PlayerSettings;
using System.Linq;

public class AllPlayerDataManager : NetworkBehaviour
{
    public static AllPlayerDataManager Instance;

    public event Action<ulong> OnPlayerDead;
    public event Action<ulong> OnPlayerHealthChanged;

    private NetworkList<PlayerData> allPlayerData;

    private const int LIFEPOINT = 10;
    private const int LIFEPOINT_TO_REDUCE = 1;

    private void Awake()
    {
        allPlayerData = new NetworkList<PlayerData>();

        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;

    }

    public void AddPlacedPlayer(ulong id)
    {
        for (int i = 0; i < allPlayerData.Count; i++)
        {
            if (allPlayerData[i].clientID == id)
            {
                PlayerData newData = new PlayerData(
                    allPlayerData[i].clientID,
                    allPlayerData[i].score,
                    allPlayerData[i].lifePoint,
                    true
                    );

                allPlayerData[i] = newData;
            }
        }
    }

    public bool GetHasPlacerPlaced(ulong id)
    {
        for (int i = 0; i < allPlayerData.Count; i++)
        {
            if (allPlayerData[i].clientID == id)
            {
                return allPlayerData[i].playerPlaced;
            }
        }
        return false;

    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            AddNewClientToList(NetworkManager.LocalClientId);
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += AddNewClientToList;
        BulletData.OnHitPlayer += BulletDataOnHitPlayer;
        KillPlayer.OnKillPlayer += KillPlayer_OnKillPlayer;
        RestartGame.OnRestartGame += RestartGame_OnRestartGame;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= AddNewClientToList;
        BulletData.OnHitPlayer -= BulletDataOnHitPlayer;
        KillPlayer.OnKillPlayer -= KillPlayer_OnKillPlayer;
        RestartGame.OnRestartGame -= RestartGame_OnRestartGame;
    }

    private void RestartGame_OnRestartGame()
    {
        if (!IsServer) return;

        List<NetworkObject> playerObjects = FindObjectsOfType<PlayerMovement>()
            .Select(x => x.transform.GetComponent<NetworkObject>()).ToList();

        List<NetworkObject> bulletObjects = FindObjectsOfType<BulletData>()
            .Select(x => x.transform.GetComponent<NetworkObject>()).ToList();

        foreach (var playerobj in playerObjects)
        {
            playerobj.Despawn();
        }

        foreach (var bulletobj in bulletObjects)
        {
            bulletobj.Despawn();
        }

        ResetNetworkList();
    }

    private void ResetNetworkList()
    {
        for (int i = 0; i < allPlayerData.Count; i++)
        {
            PlayerData resetPlayer = new PlayerData(
                allPlayerData[i].clientID,
                playerPlaced: false,
                lifePoint: LIFEPOINT,
                score: 0
                );
            allPlayerData[i] = resetPlayer;
        }
    }

    private void KillPlayer_OnKillPlayer(ulong id)
    {
        (ulong, ulong) fromTo = new(555, id);
        BulletDataOnHitPlayer(fromTo);
    }

    public float GetPlayerHealth(ulong id)
    {
        for (int i = 0; i < allPlayerData.Count; i++)
        {
            if (allPlayerData[i].clientID == id)
            {
                return allPlayerData[i].lifePoint;
            }
        }

        return default;
    }

    private void BulletDataOnHitPlayer((ulong from, ulong to) ids)
    {
        if (IsServer)
        {
            if(ids.from != ids.to)
            {
                for (int i = 0; i < allPlayerData.Count; i++)
                {
                    if (allPlayerData[i].clientID == ids.to)
                    {
                        int lifePointToReduce = allPlayerData[i].lifePoint == 0 ?
                                                              0 : LIFEPOINT_TO_REDUCE;

                        PlayerData newData = new PlayerData(
                            allPlayerData[i].clientID,
                            allPlayerData[i].score,
                            allPlayerData[i].lifePoint - lifePointToReduce,
                            allPlayerData[i].playerPlaced
                            );

                        if (newData.lifePoint <= 0)
                        {
                            OnPlayerDead?.Invoke(ids.to);
                        }

                        Debug.Log("Player got hit: " + ids.to + "LifePoint Left: " +
                                         newData.lifePoint + "Shot by: " + ids.from);

                        allPlayerData[i] = newData;
                        break;
                    }
                }
            }
        }

        SyncReducePlayerHealthClientRpc(ids.to);
    }

    [ClientRpc]
    private void SyncReducePlayerHealthClientRpc(ulong hitID)
    {
        OnPlayerHealthChanged?.Invoke(hitID);
    }

    private void AddNewClientToList(ulong clientID)
    {
        if (!IsServer) return;

        foreach (var playerData in allPlayerData)
        {
            if (playerData.clientID == clientID) return;
        }

        PlayerData newPlayerData = new PlayerData();
        newPlayerData.clientID = clientID;
        newPlayerData.score = 0;
        newPlayerData.lifePoint = LIFEPOINT;
        newPlayerData.playerPlaced = false;

        if (allPlayerData.Contains(newPlayerData)) return;

        allPlayerData.Add(newPlayerData);
        PrintAllPlayerPlayerList();

    }

    private void PrintAllPlayerPlayerList()
    {
        foreach (var playerData in allPlayerData)
        {
            Debug.Log("Player ID: " +  playerData.clientID + "has placed: " + 
                                        playerData.playerPlaced + "called by: " + 
                                            NetworkManager.Singleton.LocalClientId);
        }
    }
}