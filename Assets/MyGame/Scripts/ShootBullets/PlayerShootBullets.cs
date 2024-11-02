using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerShootBullets : NetworkBehaviour
{
    private PlayerInputControls _playerInputControl;

    private const float BULLET_DELAY = 0.2f;
    private const float SHOOTING_DELAY = 0.2f;
    private const float BULLET_SPEED = 5f;
    private const float BULLET_ANGLE_AMPLYFY = 0.25f;
    private const float BULLETSHOOTANGLEMAX = 25;

    private Transform bulletSpawnTransform;
    private float bulletShootAngle;
    private Coroutine ShootAutoCoroutine;

    [SerializeField] private GameObject bulletPrefab;



    public override void OnNetworkSpawn()
    {
        bulletSpawnTransform = GetComponentInChildren<ShootBulletTransformReference>().
                                                                            transform;

        if (GetComponent<NetworkObject>().IsOwner)
        {
            _playerInputControl = GetComponent<PlayerInputControls>();

            _playerInputControl.OnShootInput += StartShooting;
            _playerInputControl.OnShootInputCanceled += StopShooting;

            _playerInputControl.OnShootAngleInput +=
                                            _playerInputControlOnShootAngleInput;
            _playerInputControl.OnShootAngleCanceled +=
                                            _playerInputControlOnShootAngleCanceled;

        }

    }

    private void StartShooting()
    {
        if (ShootAutoCoroutine == null)
        {
            ShootAutoCoroutine = StartCoroutine(ShootCoroutine());
        }
    }

    private IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(SHOOTING_DELAY);

        while (true)
        {
            StartShootBulletServerRpc(bulletShootAngle);

            yield return new WaitForSeconds(BULLET_DELAY);

        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartShootBulletServerRpc(float bulletShootAngle)
    {
        Quaternion rotation = Quaternion.Euler(bulletShootAngle, 0, 1);

        bulletSpawnTransform.localRotation = rotation;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position,
                                Quaternion.LookRotation(bulletSpawnTransform.up));

        NetworkObject bulletNetworkObject = bullet.GetComponent<NetworkObject>();

        bulletNetworkObject.Spawn();

        Rigidbody bulletRigitBody = bullet.GetComponent<Rigidbody>();

        bulletRigitBody.AddForce(bulletSpawnTransform.forward * BULLET_SPEED,
                                                            ForceMode.VelocityChange);
    }

    private void StopShooting()
    {
        if (ShootAutoCoroutine != null)
        {
            bulletShootAngle = 0f;
            StopCoroutine(ShootAutoCoroutine);
            ShootAutoCoroutine = null;
        }
    }

    private void _playerInputControlOnShootAngleInput(Vector2 angleValue)
    {
        float newAngle;

        if (angleValue == Vector2.zero)
        {
            newAngle = 0f;
        }
        else
        {
            newAngle = bulletShootAngle + angleValue.y * -BULLET_ANGLE_AMPLYFY;
            newAngle = Mathf.Clamp(newAngle, -BULLETSHOOTANGLEMAX, BULLETSHOOTANGLEMAX);
        }

        bulletShootAngle = newAngle;
    }

    private void _playerInputControlOnShootAngleCanceled()
    {
        throw new NotImplementedException();
    }



}
