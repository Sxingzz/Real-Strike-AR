using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class PlayerInputControls : NetworkBehaviour
{
    private PlayerControlInputAction _playerControlsInputAction;
    private Vector3 movementVector;

    public event Action<Vector3> OnMoveInput;
    public event Action OnMoveActionCanceled;

    public event Action OnShootInput;
    public event Action OnShootInputCanceled;

    public event Action<Vector2> OnShootAngleInput;
    public event Action OnShootAngleCanceled;

    public override void OnNetworkSpawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            _playerControlsInputAction = new PlayerControlInputAction();
            _playerControlsInputAction.Enable();

            _playerControlsInputAction.PlayerControlsMap.Move.performed +=
                                                                 MoveActionPerformed;
            _playerControlsInputAction.PlayerControlsMap.Move.canceled +=
                                                                 MoveActionCanceled;

            _playerControlsInputAction.PlayerControlsMap.Shoot.performed += 
                                                                 ShootOnPerFormed;
            _playerControlsInputAction.PlayerControlsMap.Shoot.canceled +=
                                                                 ShootOnCanceled;

            _playerControlsInputAction.PlayerControlsMap.ShootAngle.performed +=
                                                                 ShootAnglePerformed;
            _playerControlsInputAction.PlayerControlsMap.ShootAngle.canceled +=
                                                                 ShootAngleCanceled;
        }
    }

    private void MoveActionPerformed(InputAction.CallbackContext context)
    {
        Vector2 v2Movement = context.ReadValue<Vector2>();
        movementVector = new Vector3(v2Movement.x, 0, v2Movement.y);
    }

    private void MoveActionCanceled(InputAction.CallbackContext context)
    {
        movementVector = Vector3.zero;
        OnMoveActionCanceled?.Invoke();
    }


    private void ShootOnPerFormed(InputAction.CallbackContext context)
    {
        OnShootInput?.Invoke();
    }

    private void ShootOnCanceled(InputAction.CallbackContext context)
    {
        OnShootInputCanceled?.Invoke();
    }

    private void ShootAnglePerformed(InputAction.CallbackContext context)
    {
        OnShootAngleInput?.Invoke(context.ReadValue<Vector2>());
    }

    private void ShootAngleCanceled(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }


    private void Update()
    {
        if (movementVector != Vector3.zero)
        {
            OnMoveInput?.Invoke(movementVector);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            _playerControlsInputAction.PlayerControlsMap.Move.performed -=
                                                                MoveActionPerformed;
            _playerControlsInputAction.PlayerControlsMap.Move.canceled -=
                                                                MoveActionCanceled;

            _playerControlsInputAction.PlayerControlsMap.Shoot.performed -=
                                                                 ShootOnPerFormed;
            _playerControlsInputAction.PlayerControlsMap.Shoot.canceled -=
                                                                 ShootOnCanceled;
        }
       
    }

}

