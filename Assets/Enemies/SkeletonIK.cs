using UnityEngine;

public class SkeletonIK : MonoBehaviour
{
    public GameObject leftLeg = null;
    public GameObject rightLeg = null;
    public GameObject leftArm = null;
    public GameObject rightArm = null;

    private IK leftLegIK = null;
    private IK rightLegIK = null;

    [SerializeField] private float stepHeight = 0.5f;
    [SerializeField] private float stepDistance = 1f;
    [SerializeField] private float stepDuration = 0.5f;

    private Vector3 leftFootTarget;
    private Vector3 rightFootTarget;

    private Vector3 lastLeftFootPos;
    private Vector3 lastRightFootPos;

    private float leftStepProgress = 0f;
    private float rightStepProgress = 0f;

    private bool isLeftLegMoving = true;

    void Start()
    {
        leftLegIK = leftLeg.GetComponent<IK>();
        rightLegIK = rightLeg.GetComponent<IK>();

        lastLeftFootPos = leftLeg.transform.position;
        lastRightFootPos = rightLeg.transform.position;

        leftFootTarget = lastLeftFootPos;
        rightFootTarget = lastRightFootPos;
    }

    void FixedUpdate()
    {
        AnimateLegs();
    }

    void AnimateLegs()
    {
        if (isLeftLegMoving)
        {
            MoveLeg(leftLegIK, ref leftFootTarget, ref lastLeftFootPos, ref leftStepProgress);
        }
        else
        {
            MoveLeg(rightLegIK, ref rightFootTarget, ref lastRightFootPos, ref rightStepProgress);
        }

        if (leftStepProgress >= 1f || rightStepProgress >= 1f)
        {
            isLeftLegMoving = !isLeftLegMoving;
            ResetStepProgress();
        }
    }

    void MoveLeg(IK legIK, ref Vector3 targetFootPos, ref Vector3 lastFootPos, ref float stepProgress)
    {
        stepProgress += Time.fixedDeltaTime / stepDuration;

        Vector3 nextFootPosition = GetNextFootPosition(lastFootPos);

        if (stepProgress < 0.5f)
        {
            Vector3 midPos = Vector3.Lerp(lastFootPos, nextFootPosition, stepProgress * 2f) + Vector3.up * stepHeight;
            legIK.target = midPos;
        }
        else
        {
            legIK.target = Vector3.Lerp(lastFootPos, nextFootPosition, (stepProgress - 0.5f) * 2f);
        }

        if (stepProgress >= 1f)
        {
            lastFootPos = targetFootPos;
            targetFootPos = nextFootPosition;
        }
    }

    Vector3 GetNextFootPosition(Vector3 currentFootPos)
    {
        Vector3 forward = transform.forward * stepDistance;
        Vector3 stepPosition = transform.position + forward;

        if (Physics.Raycast(stepPosition + Vector3.up, Vector3.down, out RaycastHit hit))
        {
            stepPosition = hit.point;
        }

        return stepPosition;
    }

    void ResetStepProgress()
    {
        leftStepProgress = 0f;
        rightStepProgress = 0f;
    }
}
