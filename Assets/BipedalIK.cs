using JetBrains.Annotations;
using UnityEngine;

public class BipedalIK : MonoBehaviour
{
    public GameObject leftLeg;
    public GameObject rightLeg;
    public bool animateLegs;
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

    public delegate void OnLegHitGroundCallback();

    [CanBeNull] private OnLegHitGroundCallback _onLegHitGroundCallback;

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

    public void MoveLegsToResting()
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
    }

    public void SetOnLegHitGroundCallback(OnLegHitGroundCallback onLegHitGroundCallback)
    {
        _onLegHitGroundCallback = onLegHitGroundCallback;
    }

    public void AnimateLegs()
    {
        if (_velocity == Vector3.zero)
        {
            MoveLegsToResting();
            return;
        }
        _leftLegAnimationTime += Time.deltaTime;
        _rightLegAnimationTime += Time.deltaTime;
    }

    Vector3 GetNextFootPosition(IK legIK)
    {
        Vector3 forward = transform.forward * stepDistance;
        Vector3 stepPosition = legIK.joints[0].position + forward; // from hip forward

        if (Physics.Raycast(stepPosition + Vector3.up, Vector3.down, out RaycastHit hit))
        {
            if (hit.transform.gameObject.CompareTag("Ground"))
            {
                stepPosition = hit.point;
                return stepPosition;
            }
        }

        return legIK.joints[legIK.joints.Count - 1].position + forward;
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


    void Update()
    {
        if (animateLegs)
        {
            AnimateLegs();
        }
    }
}