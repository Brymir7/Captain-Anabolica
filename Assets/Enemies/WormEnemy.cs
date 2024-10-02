using System.Collections.Generic;
using Player.Weapons;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemies
{
    public class WormEnemy : Enemy
    {
        public int amountOfSegments = 12;
        public GameObject jointPrefab;
        public GameObject connectorPrefab;
        private List<GameObject> _joints = new List<GameObject>();
        private List<GameObject> _connectors = new List<GameObject>();
        private bool reversedDirection = false;
        private IK _ik;

        public override void Attack()
        {
        }

        public override void SetForwardVecToPlayer()
        {
        }

        protected override void OnDirectionUpdate(Vector3 newDirection)
        {
            Velocity = (Player.transform.position - _ik.target).normalized;
            //  Vector3 perpendicular = Vector3.Cross(Velocity, Vector3.up).normalized;
            //  float randomOffset = Random.Range(-1f, 1f);
            //  Vector3 lateralOffset = perpendicular * (randomOffset);
            //  Velocity += lateralOffset;

            Velocity.y = 0;
            var distTailToPlayer = Vector3.Distance(Player.transform.position, _ik.joints[0].position);
            var distHeadToPlayer =
                Vector3.Distance(Player.transform.position, _ik.joints[_ik.joints.Count - 1].position);
            if (distTailToPlayer < distHeadToPlayer) // just use tail instead of head to move (smart worm)
            {
                _ik.joints.Reverse();
                reversedDirection = !reversedDirection;
                _ik.target = _ik.joints[_ik.joints.Count - 1].position;
            }
        }

        public override void Move()
        {
            _ik.target += Velocity * MoveSpeed;
            _ik.target.y =
                Mathf.Max(
                    (GetFirstGroundBelow() + 0.1f *
                        Vector3.up).y,
                    _ik.target.y);
        }

        void Start()
        {
            _ik = GetComponent<IK>();
            Assert.IsTrue(_ik != null);
            CreateWorm();
        }

        void CreateWorm()
        {
            Vector3 startPosition = transform.position;
            for (int i = 0; i < amountOfSegments; i++)
            {
                GameObject newJoint = Instantiate(jointPrefab, startPosition, Quaternion.identity);
                newJoint.transform.parent = transform;
                _ik.joints.Add(newJoint.transform);
                _joints.Add(newJoint);
                if (i > 0)
                {
                    GameObject newConnector = Instantiate(connectorPrefab, startPosition, Quaternion.identity);
                    newConnector.transform.parent = transform;
                    Connector connectorScript = newConnector.GetComponent<Connector>();
                    Assert.IsTrue(connectorScript != null);
                    connectorScript.startJoint = _joints[i - 1].transform;
                    connectorScript.endJoint = newJoint.transform;
                    connectorScript.UpdateConnector();
                    BoxCollider boxCollider = newConnector.AddComponent<BoxCollider>();
                    _connectors.Add(newConnector);
                }

                startPosition += transform.forward;
            }

            _ik.target = _joints[_joints.Count - 1].transform.position;
            _ik.Awake();
        }

        protected override void BulletOnTriggerEnterCallback(Collider other, ProjectileBase bullet)
        {
            HandleBulletInteraction(bullet);
            Instantiate(onHitVFX, other.transform.position, Quaternion.identity);
        }

        protected override void BulletOnCollisionEnterCallback(Collision collision, ProjectileBase bullet)
        {
            HandleBulletInteraction(bullet);
            Instantiate(onHitVFX, collision.contacts[0].point, Quaternion.identity);
        }

        private void HandleBulletInteraction(ProjectileBase bullet)
        {
            if (_joints.Count == 0 || _connectors.Count == 0) return;
            var dmg = bullet.GetDamage();
            print("dmg" + dmg);
            for (int i = 0; i < dmg; i++)
            {
                if (_joints.Count == 0 || _connectors.Count == 0) break;

                GameObject jointToRemove;
                GameObject connectorToRemove;
                if (reversedDirection)
                {
                    jointToRemove = _joints[0];
                    connectorToRemove = _connectors[0];
                    _joints.RemoveAt(0);
                    _connectors.RemoveAt(0);
                }
                else
                {
                    jointToRemove = _joints[_joints.Count - 1];
                    connectorToRemove = _connectors[_connectors.Count - 1];
                    _joints.RemoveAt(_joints.Count - 1);
                    _connectors.RemoveAt(_connectors.Count - 1);
                }

                Destroy(jointToRemove);
                Destroy(connectorToRemove);
                _ik.RemoveLastJoint();
                amountOfSegments--;
                if (amountOfSegments <= 1) break; // Prevents removing too many segments
            }

            TakeDamage(dmg + (amountOfSegments <= 1 ? 999 : 0)); // if theres no real worm anymore just kill it
        }
    }
}