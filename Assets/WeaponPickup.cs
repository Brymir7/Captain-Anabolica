using System.Collections;
using System.Collections.Generic;
using Player;
using Player.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponPickup : MonoBehaviour
{
    private float _passedTime = 0.0f;
    public float rotationSpeed = 20f;
    [FormerlySerializedAs("w_type")] public WeaponType wType;

    void Update()
    {
        _passedTime += Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, _passedTime * rotationSpeed, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var wHandling = collision.gameObject.GetComponent<WeaponHandling>();
            if (wHandling.HasUnlockedWeapon(wType))
            {
                wHandling.UpgradeWeaponRandomAttribute(wType);
            }
            else
            {
                wHandling.UnlockWeapon(wType);
            }

            Destroy(gameObject);
        }
    }
}