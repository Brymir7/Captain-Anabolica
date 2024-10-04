using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemArmIK : MonoBehaviour
{
    [SerializeField] private IK leftArm;
    [SerializeField] private IK rightArm;
    [SerializeField] private Transform leftArmRoot;
    [SerializeField] private Transform rightArmRoot;

    private Vector3 _leftArmTarget;
    private Vector3 _rightArmTarget;
    private Vector3 _leftArmRelativeOffset;
    private Vector3 _rightArmRelativeOffset;
    [SerializeField] private float swingSpeed = 2f;
    [SerializeField] private float swingAmplitude = 0.5f;

    void Start()
    {
        // Ensure arm roots are assigned
        if (leftArmRoot == null) leftArmRoot = leftArm.joints[0];
        if (rightArmRoot == null) rightArmRoot = rightArm.joints[0];

        // Calculate relative offsets
        _leftArmRelativeOffset =
            leftArm.joints[leftArm.joints.Count - 1].position - leftArmRoot.position +
            Vector3.up * transform.position.y * 0.75f;
        _rightArmRelativeOffset =
            rightArm.joints[rightArm.joints.Count - 1].position - rightArmRoot.position +
            Vector3.up * transform.position.y * 0.75f;
    }

    void Update()
    {
        float swingOffset = Mathf.Sin(Time.time * swingSpeed) * swingAmplitude;
        float heightOffset = Mathf.Cos(Time.time * swingSpeed) * swingAmplitude;
        Vector3 leftBaselinePosition = leftArmRoot.TransformPoint(_leftArmRelativeOffset);
        Vector3 leftSwing = transform.forward * swingOffset - transform.up * (Mathf.Abs(heightOffset * 0.55f));
        _leftArmTarget = leftBaselinePosition + leftSwing;
        leftArm.target = _leftArmTarget;

        Vector3 rightBaselinePosition = rightArmRoot.TransformPoint(_rightArmRelativeOffset);
        Vector3 rightSwing = -transform.forward * swingOffset - transform.up * (Mathf.Abs(heightOffset * 0.55f));
        _rightArmTarget = rightBaselinePosition + rightSwing;
        rightArm.target = _rightArmTarget;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || leftArmRoot == null || rightArmRoot == null) return;

        // Draw green sphere for left arm target
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_leftArmTarget, 0.1f);

        // Draw red sphere for right arm target
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_rightArmTarget, 0.1f);

        // Draw yellow sphere for left arm baseline
        Vector3 leftBaselinePosition = leftArmRoot.TransformPoint(_leftArmRelativeOffset);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(leftBaselinePosition, 0.05f);

        // Draw blue sphere for right arm baseline
        Vector3 rightBaselinePosition = rightArmRoot.TransformPoint(_rightArmRelativeOffset);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(rightBaselinePosition, 0.05f);

        // Draw lines between baseline and current target
        Gizmos.color = Color.white;
        Gizmos.DrawLine(leftBaselinePosition, _leftArmTarget);
        Gizmos.DrawLine(rightBaselinePosition, _rightArmTarget);
    }
}