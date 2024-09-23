using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAnimation : MonoBehaviour
{
    private Transform leftLeg = null;
    private Transform rightLeg = null;
    private Transform leftArm = null;
    private Transform rightArm = null;
    private Transform shoulder = null;
    private float speed = 0f;
    public float maxAngle = 50f;
    public float animationSpeed = 0.5f;
    public float maxSpeed = 0.3f;

    void Start()
    {
        leftLeg = transform.Find("Legs/leftLeg");
        rightLeg = transform.Find("Legs/rightLeg");
        leftArm = transform.Find("Shoulder/leftArm");
        rightArm = transform.Find("Shoulder/rightArm");
        shoulder = transform.Find("Shoulder");
    }

    void Update()
    {
        if (speed <= 0)
        {
            //leftLeg.localRotation = Quaternion.Euler(0f, 0f, 0f);
            //rightLeg.localRotation = Quaternion.Euler(0f, 0f, 0f);
            return;
        }

        float normalizedSpeed = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(speed));
        float currentAngle = maxAngle * normalizedSpeed;
        float leftLegRotation = Mathf.Sin(Time.time * animationSpeed) * currentAngle;
        float rightLegRotation = Mathf.Sin(Time.time * animationSpeed + Mathf.PI) * currentAngle;

        leftLeg.localRotation = Quaternion.Euler(leftLegRotation, 0, 0);
        rightLeg.localRotation = Quaternion.Euler(rightLegRotation, 0, 0);
        shoulder.localRotation = Quaternion.Euler(shoulder.localRotation.x, leftLegRotation, shoulder.localRotation.z);
    }

    public void UpdateAnimationSpeed(float newSpeed)
    {
        speed = Mathf.Clamp(newSpeed, 0, maxSpeed);
    }
}