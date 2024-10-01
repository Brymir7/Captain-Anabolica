using UnityEngine;

public class BipedalIK : MonoBehaviour
{
    public GameObject leftLeg;
    public GameObject rightLeg;

    public GameObject leftArm;
    public GameObject rightArm;
    public bool animateLegs;
    public bool animateArms;
    public float epsilonForSameLoop = 0.1f;
    private IK _leftLegIK;
    private IK _rightLegIK;

    private Vector3 _velocity = Vector3.zero;
    private Vector3 _nextLeftLegTarget;
    private Vector3 _nextRightLegTarget;
    private float _leftLegAnimationTime;
    private float _rightLegAnimationTime;

    [SerializeField] private float stepDistance;
    [SerializeField] private float animationTime = 0.5f;

    void Start()
    {
        _leftLegIK = leftLeg.GetComponent<IK>();
        _rightLegIK = rightLeg.GetComponent<IK>();
        _nextLeftLegTarget = _leftLegIK.target;
        _nextRightLegTarget = _rightLegIK.target;
        _leftLegAnimationTime = 0f;
        _rightLegAnimationTime = 0f;
    }

    public void SetVelocity(Vector3 velocity)
    {
        this._velocity = velocity;
    }

    void Update()
    {
        if (_velocity == Vector3.zero)
        {
            var leftLegRestingPos = GetRestingFootPosition(_leftLegIK);
            var rightLegRestingPos = GetRestingFootPosition(_rightLegIK);

            if (Vector3.Distance(_leftLegIK.target, leftLegRestingPos) > 0.1f)
            {
                _nextLeftLegTarget = leftLegRestingPos;
            }

            if (Vector3.Distance(_rightLegIK.target, rightLegRestingPos) > 0.1f)
            {
                _nextRightLegTarget = rightLegRestingPos;
            }

            IK.MoveIKObjectToTarget(_leftLegIK, _nextLeftLegTarget, _leftLegAnimationTime, 0.2f);
            _leftLegAnimationTime += Time.deltaTime;
            IK.MoveIKObjectToTarget(_rightLegIK, _nextRightLegTarget, _rightLegAnimationTime, 0.2f);
            _rightLegAnimationTime += Time.deltaTime;
            return;
        }

        if (animateLegs)
        {
            var nextLegLeftPos = GetNextFootPosition(_leftLegIK);
            if (
                Vector3.Distance(nextLegLeftPos, _leftLegIK.target) > stepDistance * 2.0)
            {
                _nextLeftLegTarget = nextLegLeftPos;
            }
            var nextLegRightPos = GetNextFootPosition(_rightLegIK);
            // only move right feet if right is behind and left is in front by at least a step distance
            var forwardDistanceRightFeetToLeftTarget =
                Vector3.Dot(transform.forward, (nextLegRightPos - _leftLegIK.target));
            // if legs got stuck || are in the same loop due to physics || initial position, then only move left leg
            var sameLoop = Vector3.Distance(_rightLegIK.joints[_rightLegIK.joints.Count - 1].position,
                _leftLegIK.joints[_leftLegIK.joints.Count - 1].position) < epsilonForSameLoop;

            if (!sameLoop && forwardDistanceRightFeetToLeftTarget > stepDistance &&
                Vector3.Distance(nextLegRightPos, _rightLegIK.target) > stepDistance * 2.0f)
            {
                _nextRightLegTarget = nextLegRightPos;
            }

            var finishedLeft =
                IK.MoveIKObjectToTarget(_leftLegIK, _nextLeftLegTarget, _leftLegAnimationTime, animationTime);
            _leftLegAnimationTime += Time.deltaTime;
            if (finishedLeft) _leftLegAnimationTime = 0.0f;


            var finishedRight =
                IK.MoveIKObjectToTarget(_rightLegIK, _nextRightLegTarget, _rightLegAnimationTime, animationTime);
            _rightLegAnimationTime += Time.deltaTime;
            if (finishedRight) _rightLegAnimationTime = 0.0f;
        }
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