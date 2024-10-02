using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerXpAttractor : MonoBehaviour
    {
        [SerializeField] private float attractionRadius = 5f;
        [SerializeField] private LayerMask xpOrbLayer;
        [SerializeField] private int maxXpOrbs = 20; // Adjust based on your needs

        private Collider[] _colliderResults;
        private List<XpOrb> _activeXpOrbs;

        private void Awake()
        {
            _colliderResults = new Collider[maxXpOrbs];
            _activeXpOrbs = new List<XpOrb>(maxXpOrbs);
        }

        private void FixedUpdate()
        {
            int numColliders =
                Physics.OverlapSphereNonAlloc(transform.position, attractionRadius, _colliderResults, xpOrbLayer);
            _activeXpOrbs.Clear();
            for (int i = 0; i < numColliders; i++)
            {
                var xpOrb = _colliderResults[i].GetComponent<XpOrb>();
                _activeXpOrbs.Add(xpOrb);
            }

            for (int i = 0; i < _activeXpOrbs.Count; i++)
            {
                _activeXpOrbs[i].StartAttraction(transform);
            }


            void OnDrawGizmosSelected()
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, attractionRadius);
            }
        }
    }
}