using System.Collections;
using UnityEngine;

public class BipedalIK : MonoBehaviour
{
    public GameObject leftLeg = null;
    public GameObject rightLeg = null;

    public GameObject leftArm = null;
    public GameObject rightArm = null;

    private IK leftLegIK = null;
    private IK rightLegIK = null;

    [SerializeField] private float stepHeight = 0.5f;
    [SerializeField] private float stepDistance = 2f;


    public Vector3 velocity = Vector3.one;


    private Vector3 nextLeftLegTarget = Vector3.zero;
    private Vector3 nextRightLegTarget = Vector3.zero;
    private bool initialMovement = false;

    void Start()
    {
        leftLegIK = leftLeg.GetComponent<IK>();
        rightLegIK = rightLeg.GetComponent<IK>();
        nextLeftLegTarget = leftLegIK.target;
        nextRightLegTarget = rightLegIK.target;
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }


    void FixedUpdate()
    {
        if (velocity == Vector3.zero)
        {
            var leftLegRestingPos = GetRestingFootPosition(leftLegIK);
            var rightLegRestingPos = GetRestingFootPosition(rightLegIK);
            if (Vector3.Distance(leftLegIK.target, leftLegRestingPos) > 0.1f)
            {
                StartCoroutine(MoveLegToTarget(leftLegIK, leftLegRestingPos));
                initialMovement = true;
            }

            if (Vector3.Distance(rightLegIK.target, rightLegRestingPos) > 0.1f)
            {
                StartCoroutine(MoveLegToTarget(rightLegIK, rightLegRestingPos));
            }

            return;
        }

        AnimateLegs();
    }


    void AnimateLegs()
    {
        var nextLegLeftPos = GetNextFootPosition(leftLegIK);
        if (initialMovement ||
            Vector3.Distance(nextLegLeftPos, leftLegIK.target) > stepDistance * 2.0)
        {
            initialMovement = false;
            StartCoroutine(MoveLegToTarget(leftLegIK, nextLegLeftPos));
        }

        var nextLegRightPos = GetNextFootPosition(rightLegIK);
        var distanceRightTargetToLeftTarget = Vector3.Dot(transform.forward, (nextLegRightPos - leftLegIK.target));

        if (distanceRightTargetToLeftTarget > stepDistance &&
            Vector3.Distance(nextLegRightPos, rightLegIK.target) > stepDistance * 2.0f)
        {
            StartCoroutine(MoveLegToTarget(rightLegIK, nextLegRightPos));
        }
    }

    IEnumerator MoveLegToTarget(IK leg, Vector3 target)
    {
        Vector3 initialPos = leg.target;
        float time = 0f;
        float duration = 0.5f; // Adjust the duration to control the speed of the movement.

        while (time < duration)
        {
            leg.target = Vector3.Lerp(initialPos, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        leg.target = target;
    }

    Vector3 GetNextFootPosition(IK legIK)
    {
        Vector3 forward = transform.forward * stepDistance;
        Vector3 stepPosition = legIK.joints[0].position + forward; // from hip forward

        if (Physics.Raycast(stepPosition + Vector3.up, Vector3.down, out RaycastHit hit))
        {
            stepPosition = hit.point;
        }

        return stepPosition;
    }

    Vector3 GetRestingFootPosition(IK legIK)
    {
        Vector3 hipPosition = legIK.joints[0].position;
        if (Physics.Raycast(hipPosition, Vector3.down, out RaycastHit hit))
        {
            hipPosition = hit.point;
        }

        return hipPosition + Vector3.down * stepDistance;
    }
}