using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerAnimation : NetworkBehaviour
{
    private Animator _playerAnimator;
    private PlayerInputControls _playerInputControls;

    private AnimatorControllerParameter[] allPrams;


    public override void OnNetworkSpawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            _playerAnimator = GetComponentInChildren<Animator>();
            _playerInputControls = GetComponent<PlayerInputControls>();

            allPrams = _playerAnimator.parameters;

            _playerInputControls.OnMoveInput += PlayerInputControlsOnMoveInput;
            _playerInputControls.OnMoveActionCanceled += 
                                            _playerInputControlsOnMoveActionCanceled;

            _playerInputControls.OnShootInput += _playerInputControlsOnShootInput;
            _playerInputControls.OnShootInputCanceled += 
                                            _playerInputControlsOnShootInputCanceled;

        }
    }

    private void PlayerInputControlsOnMoveInput(Vector3 context)
    {
        if (context.magnitude > 0)
        {
            SetOneParameterToTrue("IsRunning");
        }
       
    }

    private void _playerInputControlsOnMoveActionCanceled()
    {
        SetOneParameterToTrue("IsIdle");
    }

    private void _playerInputControlsOnShootInput()
    {
        SetOneParameterToTrue("IsShooting");
    }

    private void _playerInputControlsOnShootInputCanceled()
    {
        SetOneParameterToTrue("IsIdle");
    }

    private void SetOneParameterToTrue(string param)
    {
        foreach (var parameter in allPrams)
        {
            if (parameter.name == param)
            {
                _playerAnimator.SetBool(parameter.nameHash, true);
                
            }
            else
            {
                _playerAnimator.SetBool(parameter.nameHash, false);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        _playerInputControls.OnMoveInput -= PlayerInputControlsOnMoveInput;
        _playerInputControls.OnMoveActionCanceled -=
                                        _playerInputControlsOnMoveActionCanceled;

        _playerInputControls.OnShootInput -= _playerInputControlsOnShootInput;
        _playerInputControls.OnShootInputCanceled -=
                                        _playerInputControlsOnShootInputCanceled;
    }


} 
