﻿using UnityEngine;
using UnityEngine.Assertions;

namespace Player.Weapons
{
    class LauncherBullet : ProjectileBase
    {
        [SerializeField] private int recursiveChildren; // Initial health of the bullet
        [SerializeField] private int amountOfChildrenPerRecursion;
        [SerializeField] private GameObject bulletPrefab; // Prefab for spawning new bullets
        [SerializeField] private GameObject miniExplosionVFX; // Prefab for mini explosion effect
        [SerializeField] private float childrenRelativeSpeed;

        protected void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Ground"))
            {
                SpawnMiniExplosion();
                if (IsEnabled())
                {
                    SpawnChildBullets(collision);
                }
                Destroy(gameObject);
            }
        }

        private void SpawnMiniExplosion()
        {
            if (miniExplosionVFX != null)
            {
                Instantiate(miniExplosionVFX, transform.position, Quaternion.identity);
            }
        }

        public float scatterForce = 10f;
        public float scatterRadius = 2f;

        private void SpawnChildBullets(Collision collision)
        {
            if (recursiveChildren >= 1 && bulletPrefab != null)
            {
                for (int i = 0; i < amountOfChildrenPerRecursion; i++)
                {
                    Vector3 randomDir = Random.insideUnitSphere;
                    randomDir.y = Mathf.Abs(randomDir.y); // Ensure upward direction
                    randomDir = Vector3.Slerp(Vector3.up, randomDir.normalized, Random.Range(0f, 0.5f));
                    Vector3 spawnPos = transform.position + Vector3.up + Random.insideUnitSphere * scatterRadius;
                    GameObject newBullet = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(randomDir));
                    Rigidbody childRb = newBullet.GetComponent<Rigidbody>();
                    childRb.AddForce(randomDir * scatterForce, ForceMode.Impulse);
                    LauncherBullet childBullet = newBullet.GetComponent<LauncherBullet>();
                    childBullet.recursiveChildren = recursiveChildren - 1;
                    childBullet.speed = speed * childrenRelativeSpeed;
                    Destroy(newBullet, 5f);
                }
            }
        }
    }
}