using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class KillPlayer : MonoBehaviour
{
    [SerializeField] private Button killPlayerButton;

    public static event Action<ulong> OnKillPlayer;


    private void Start()
    {
        killPlayerButton.onClick.AddListener(() =>
        {
            OnKillPlayer?.Invoke(NetworkManager.Singleton.LocalClientId);
        });
    }

}
