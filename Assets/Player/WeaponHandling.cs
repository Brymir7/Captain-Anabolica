using System.Collections;
using UnityEngine;

public class WeaponHandling : MonoBehaviour
{
    private bool isTransitioning;
    private bool isHolding;

    private GameObject bulletPrefab;

    // Animation duration fields
    private float timeToTransition = 0.5f;

    private Vector3 pistolTip;
    // target pos
    private Vector3 targetOffset = new(0.1f, -0.75f, 0.479f);

    private Vector3 targetRotatedBy = new(-90f, 0f, 14f);

    // Animation progress
    private float animationProgress;

    // Initial and target positions/rotations for Weapon
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Transform pistolArm;
    private Transform pistol;
    private Quaternion targetRotation;
    private Vector3 targetWeaponPosition;

    private void Start()
    {
        pistolArm = transform.Find("Shoulder/rightArm/pistolArm");
        if (pistolArm == null)
        {
            Debug.LogError("pistolArm not found!");
        }

        pistol = transform.Find("Shoulder/rightArm/pistolArm/Pistol");
        if (pistol == null)
        {
            Debug.LogError("pistol not found!");
        }
        else
        {
            pistolTip = pistol.position + pistol.forward * 0.3f;
            initialPosition = pistolArm.localPosition;
            initialRotation = pistolArm.localRotation;
        }
    }

    private void Update()
    {
        pistolTip = pistol.position + pistol.forward * 0.3f; 
        if (isTransitioning) // Play Animation
        {
            animationProgress += Time.deltaTime / timeToTransition;
            animationProgress = Mathf.Clamp01(animationProgress);

            pistolArm.localPosition = Vector3.Lerp(pistolArm.localPosition, targetWeaponPosition, animationProgress);
            pistolArm.localRotation = Quaternion.Slerp(pistolArm.localRotation, targetRotation, animationProgress);
            if (animationProgress >= 1f) {isTransitioning = false;}
        }
    }
    public Vector3 GetWeaponPosition() 
    {
        return pistolTip;
    }
    public void HoldWeapon()
    {
        if (!isHolding && !isTransitioning)
        {
            isTransitioning = true;
            isHolding = true;
            animationProgress = 0f;
            targetWeaponPosition = initialPosition + targetOffset;
            targetRotation =
                initialRotation * Quaternion.Euler(targetRotatedBy.x, targetRotatedBy.y, targetRotatedBy.z);
        }
    }

    public void StopHoldingWeapon()
    {
        if (isHolding && !isTransitioning)
        {
            isTransitioning = true;
            isHolding = false;
            animationProgress = 0f;
            targetWeaponPosition = initialPosition;
            targetRotation = initialRotation;
        }
    }


    public float shootCooldown = 0.1f;
    private float lastShootTime;

    public void ShootWeapon(Vector3 target)
    {
        if (isHolding && !isTransitioning && Time.time - lastShootTime >= shootCooldown)
        {

            shoot((target-pistolTip).normalized);
            lastShootTime = Time.time;
        }
    }
    public void shoot(Vector3 direction)
    {
        if (!isHolding || isTransitioning)
        {
            Debug.LogWarning(
                $"{gameObject.name}: Cannot shoot. Holding: {isHolding}, Transitioning: {isTransitioning}");
            return;
        }

        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab is not set!");
            return;
        }
        var bullet = Instantiate(bulletPrefab, pistolTip, Quaternion.identity);
        var bulletComponent = bullet.GetComponent<DefaultBullet>();
        if (bulletComponent != null)
        {
            bulletComponent.SetDirection(direction);
        }
        else
        {
            Debug.LogError("Bullet prefab does not have a Bullet component!");
            Destroy(bullet); // Clean up the instantiated object if it doesn't have the required component
        }
    }
}