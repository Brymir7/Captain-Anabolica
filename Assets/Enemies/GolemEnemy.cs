using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    public class GolemEnemy : Enemy
    {
        private BipedalIK _bipedal;
        private CameraShake _cameraShake;

        private float _shakeDelay;
        private bool _shouldShake;

        public void Start()
        {
            _bipedal = GetComponent<BipedalIK>();
            if (Camera.main != null) _cameraShake = Camera.main.GetComponent<CameraShake>();
            _bipedal.SetOnLegHitGroundCallback(HandleLegHitGround);
        }

        private void HandleLegHitGround()
        {
            _shouldShake = true; // Set the state to true to trigger the shake
        }

        private void Update()
        {
            if (_shouldShake)
            {
                _cameraShake.Shake(0.1f, 0.1f, 0.3f);
                _shouldShake = false; // Reset the state after shaking
            }
        }

        public override void Attack()
        {
        }

        public override void Move()
        {
            transform.position += Velocity;
            _bipedal.SetVelocity(Velocity);
        }
    }
}