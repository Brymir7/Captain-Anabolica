using UnityEngine;

namespace Player
{
    public class CameraShake : MonoBehaviour
    {
        private Vector3 _originalPos;
        private Transform _cameraTransform;

        private float _shakeIntensity;
        private float _shakeDuration;
        private float _shakeFalloff;
        private float _shakeTimer;

        void Start()
        {
            _cameraTransform = Camera.main.transform;
        }

        public Vector3 GetShakeOffset()
        {
            if (_shakeTimer > 0)
            {
                // Calculate shake intensity based on timer and falloff
                float currentShakeIntensity =
                    _shakeIntensity * (_shakeTimer / _shakeDuration) * Mathf.Lerp(1f, 0f, _shakeFalloff);
                Vector3 shakeOffset = Random.insideUnitSphere * currentShakeIntensity;
                _shakeTimer -= Time.deltaTime;
                return shakeOffset;
            }
            else
            {
                return Vector3.zero;
            }
        }

        // Method to trigger camera shake
        public void Shake(float intensity, float duration, float falloff)
        {
            _shakeIntensity = intensity;
            _shakeDuration = duration;
            _shakeFalloff = falloff;
            _shakeTimer = duration;
        }
    }
}