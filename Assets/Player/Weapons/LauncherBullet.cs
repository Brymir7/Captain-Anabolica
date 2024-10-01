using UnityEngine;
using UnityEngine.Assertions;

class LauncherBullet : ProjectileBase
{
    [SerializeField] private int children = 1; // Initial health of the bullet
    [SerializeField] private GameObject bulletPrefab; // Prefab for spawning new bullets
    [SerializeField] private GameObject miniExplosionVFX; // Prefab for mini explosion effect

    protected void OnCollisionEnter(Collision collision)
    {   
        print("Collision");
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Ground"))
        {
            SpawnMiniExplosion();
            SpawnChildBullet(collision);
            DestroyProjectile();
        }
    }

    private void SpawnMiniExplosion()
    {
        if (miniExplosionVFX != null)
        {
            Instantiate(miniExplosionVFX, transform.position, Quaternion.identity);
        }
    }

    private void SpawnChildBullet(Collision collision)
    {
        if (children > 1 && bulletPrefab != null)
        {
            Vector3 reflection = Vector3.Reflect(transform.forward, collision.contacts[0].normal);
            Quaternion newRotation = Quaternion.LookRotation(reflection);
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, newRotation);
            LauncherBullet childBullet = newBullet.GetComponent<LauncherBullet>();
            Assert.IsTrue(childBullet);
            {
                childBullet.children = children - 1;
            }
        }
    }
}