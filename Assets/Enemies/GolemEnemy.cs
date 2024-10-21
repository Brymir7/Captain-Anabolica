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
            _cameraShake.Shake(0.1f, 0.1f, 0.3f);
        }

        public override void Move()
        {
            transform.position += Velocity;
            _bipedal.SetVelocity(Velocity);
        }

        protected override void OnDirectionUpdate(Vector3 newDirection)
        {
            transform.rotation = transform.rotation * Quaternion.Euler(Mathf.Lerp(0f, 30f, 1.0f), 0, 0) ;
        }
    }
}