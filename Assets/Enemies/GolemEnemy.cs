using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    public class GolemEnemy : Enemy
    {
        private BipedalIK _bipedal;
        private CameraShake _cameraShake;

        public void Start()
        {
            _bipedal = GetComponent<BipedalIK>();
            if (Camera.main != null) _cameraShake = Camera.main.GetComponent<CameraShake>();
            print("camera shake is not null" + (_cameraShake != null));
            _bipedal.SetOnLegHitGroundCallback(() => { _cameraShake.Shake(0.5f, 0.5f, 0.1f); });
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