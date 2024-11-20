using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuitGame : NetworkBehaviour
{
    [SerializeField] private Button QuitGameButton;

    private void Start()
    {
        QuitGameButton.onClick.AddListener(() =>
        {
            RequestServerToQuitGameServerRpc();
        });

    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestServerToQuitGameServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("LoadScene", 
                                                                LoadSceneMode.Single);
    }




}
