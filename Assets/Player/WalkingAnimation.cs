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
    private float speed = 2f; 
    private float angle = 50f; 
    private const float MAX_SPEED = 50f;

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
        float normalizedSpeed = Mathf.InverseLerp(0, MAX_SPEED, Mathf.Abs(speed));
        float currentAngle = angle * normalizedSpeed;
        float leftLegRotation = Mathf.Sin(Time.time * speed) * currentAngle;
        float rightLegRotation = Mathf.Sin(Time.time * speed + Mathf.PI) * currentAngle;

        leftLeg.localRotation = Quaternion.Euler(leftLegRotation, 0, 0);
        rightLeg.localRotation = Quaternion.Euler(rightLegRotation, 0, 0);
      // leftArm.localRotation = Quaternion.Euler(rightLegRotation, leftArm.localRotation.y, leftArm.localRotation.z);
      // rightArm.localRotation = Quaternion.Euler(leftLegRotation, leftArm.localRotation.y, leftArm.localRotation.z);   
        shoulder.localRotation = Quaternion.Euler(shoulder.localRotation.x, leftLegRotation, shoulder.localRotation.z);
    }

    public void UpdateAnimationSpeed(float newSpeed)
    {
        speed = Mathf.Clamp(newSpeed, 0, MAX_SPEED);    
    }
}