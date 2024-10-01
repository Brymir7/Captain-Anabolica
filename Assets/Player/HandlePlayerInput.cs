using UnityEngine;

public class HandlePlayerInput : MonoBehaviour
{
    private Rigidbody _rb = null;
    private WeaponHandling _weaponHandler;
    private Camera _playerCamera;
    private PlayerMovement _playerMovement;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true; // Prevent tipping over
        _weaponHandler = GetComponent<WeaponHandling>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCamera = Camera.main;
    }

    
    public float moveSpeed = 15f;

    void Update()
    {
        _playerMovement.UpdateMoveDirection(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetMouseButton(0)) // left mouse button
        {
            _weaponHandler.ShootWeapon(_playerCamera);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _weaponHandler.SwitchWeapon();
        }
    }
}
