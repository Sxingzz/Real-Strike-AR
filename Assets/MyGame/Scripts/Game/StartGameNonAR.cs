using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class StartGameNonAR : NetworkBehaviour
{

    [SerializeField] private Button startHost;
    [SerializeField] private Button startClient;

    private void Start()
    {
        startHost.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        startClient.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }

}
