using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.SceneManagement;


public class LoadGameScene : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Singleton.Shutdown();

        List<GameObject> netObjects = 
            FindObjectsOfType<NetworkObject>().Select(obj => obj.gameObject).ToList();

        foreach (var obj in netObjects)
        {
            Destroy(obj);
        }

        GameObject startGameARObject = FindObjectOfType<StartGameAR>().gameObject;
        Destroy(startGameARObject);

        Destroy(FindObjectOfType<NetworkManager>().transform.gameObject);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);

    }



}
