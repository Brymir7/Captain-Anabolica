using System;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public float cooldown = 0.5f;
    private float lastShotTime = 0.0f;
    private GameObject bulletPrefab;
    public Transform bulletSpawn;

    public void SetBulletPrefab(GameObject bPreFab)
    {
        this.bulletPrefab = bPreFab;
    }

    public void ShootWeapon(Camera playerCamera)
    {   
            print("shoooooooo");
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            var hit_sth = Physics.Raycast(ray, out RaycastHit hit);
            Vector3 target;
            if (!hit_sth)
            {
                target = ray.GetPoint(1000.0f); // If nothing is hit, shoot towards far point.
            }
            else
            {
                target = hit.point;
            }

            shoot((target - transform.position).normalized);
            lastShotTime = Time.time;
      
    }

    private void shoot(Vector3 direction)
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawn);
        var bulletComponent = bullet.GetComponent<PistolBullet>();
        bulletComponent.SetDirection(direction);
    }
}