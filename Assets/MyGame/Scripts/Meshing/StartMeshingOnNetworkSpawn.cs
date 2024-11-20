using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.ARFoundation;

public class StartMeshingOnNetworkSpawn : NetworkBehaviour
{
    [SerializeField] private ARMeshManager _meshManager;


    private void Start()
    {
        _meshManager.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        _meshManager.enabled = true;
    }

}
