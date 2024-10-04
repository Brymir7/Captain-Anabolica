using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerSurroundingChecks : MonoBehaviour
    {
        [SerializeField] private float attractionRadius = 5f;
        [SerializeField] private LayerMask anyPickupsLayerMask;
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
                Physics.OverlapSphereNonAlloc(transform.position, attractionRadius, _colliderResults,
                    anyPickupsLayerMask);
            _activeXpOrbs.Clear();
            for (int i = 0; i < numColliders; i++)
            {
                if (_colliderResults[i].TryGetComponent<XpOrb>(out var xpOrb))
                {
                    _activeXpOrbs.Add(xpOrb);
                }
                // else if (_colliderResults[i].TryGetComponent<WeaponPickup>(out var pickup))
                // {
                // }
            }

            for (int i = 0; i < _activeXpOrbs.Count; i++)
            {
                _activeXpOrbs[i].StartAttraction(transform);
            }
        }
    }
}