using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class BipedalIK : MonoBehaviour
{
    public GameObject leftLeg;
    public GameObject rightLeg;
    public bool animateLegs;
    private IK _leftLegIK;
    private IK _rightLegIK;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _nextLeftLegTarget;
    private Vector3 _nextRightLegTarget;
    private float _leftLegAnimationTime;
    private float _rightLegAnimationTime;
    [SerializeField] private float stepDistance;
    [SerializeField] private float animationTime = 0.5f;
    [SerializeField] private List<Transform> feetKeyFramesWalking;
    [SerializeField] private List<Transform> feetKeyFramesRunning;

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

        _leftLegAnimationTime = (_leftLegAnimationTime + Time.deltaTime) % animationTime;
        _rightLegAnimationTime = (_rightLegAnimationTime + Time.deltaTime) % animationTime;
        _leftLegIK.target = CalculateFootTarget(_leftLegIK, _leftLegAnimationTime, 1);
        _rightLegIK.target = CalculateFootTarget(_rightLegIK, _rightLegAnimationTime, -1);
    }

    private Vector3 CalculateFootTarget(IK legIK, float currAnimT, float dir)
    {
        Vector3 footPosition = legIK.joints[0].position + transform.up * (-1 * legIK.totalBoneLength);
        float stepProgress = Mathf.Sin(currAnimT / animationTime) * stepDistance;

        Vector3 target = stepProgress * dir * transform.forward + footPosition;
        // if (Physics.Raycast(target + Vector3.up, Vector3.down, out RaycastHit hit))
        // {
        //     target = hit.point;
        // }

        return target;
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