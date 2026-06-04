using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DaniTech_SummonMoveTest : MonoBehaviour
{
    private enum MoveState
    {
        Forward,
        BounceBack,
        SideKick
    }

    [Header("자동 전진 설정")]
    [SerializeField] private float _forwardSpeed = 1.4f;
    [SerializeField] private Vector2 _moveDirection = Vector2.up;

    [Header("벽 감지 설정")]
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _frontCheckDistance = 0.3f;
    [SerializeField] private float _sideCheckDistance = 0.25f;
    [SerializeField] private float _checkRadius = 0.1f;

    [Header("뒤로 튕김 설정")]
    [SerializeField] private float _bounceBackSpeed = 0.8f;
    [SerializeField] private float _bounceBackTime = 0.1f;

    [Header("좌우 회피 설정")]
    [SerializeField] private float _sideKickSpeed = 1.2f;
    [SerializeField] private float _sideKickForwardMultiplier = 0.12f;
    [SerializeField] private float _minSideKickTime = 0.2f;
    [SerializeField] private float _sideChangeCooldown = 0.25f;

    private Rigidbody2D _rigidBody;

    private MoveState _moveState = MoveState.Forward;

    private int _sideDirection;
    private float _bounceTimer;
    private float _sideKickTimer;
    private float _nextSideChangeTime;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        _rigidBody.gravityScale = 0f;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (_moveDirection == Vector2.zero)
        {
            _moveDirection = Vector2.up;
        }

        if (_wallLayer.value == 0)
        {
            _wallLayer = LayerMask.GetMask("wall");
        }

        _sideDirection = Random.value < 0.5f ? -1 : 1;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 forwardDirection = _moveDirection.normalized;
        Vector2 velocity = Vector2.zero;

        if (_moveState == MoveState.Forward)
        {
            velocity = MoveForward(forwardDirection);
        }
        else if (_moveState == MoveState.BounceBack)
        {
            velocity = MoveBounceBack(forwardDirection);
        }
        else if (_moveState == MoveState.SideKick)
        {
            velocity = MoveSideKick(forwardDirection);
        }

        _rigidBody.linearVelocity = velocity;
    }

    private Vector2 MoveForward(Vector2 forwardDirection)
    {
        bool frontBlocked = IsBlocked(forwardDirection, _frontCheckDistance);

        if (frontBlocked == true)
        {
            StartBounceBack();
            return -forwardDirection * _bounceBackSpeed;
        }

        return forwardDirection * _forwardSpeed;
    }

    private Vector2 MoveBounceBack(Vector2 forwardDirection)
    {
        _bounceTimer -= Time.fixedDeltaTime;

        if (_bounceTimer <= 0f)
        {
            StartSideKick();
        }

        return -forwardDirection * _bounceBackSpeed;
    }

    private Vector2 MoveSideKick(Vector2 forwardDirection)
    {
        _sideKickTimer += Time.fixedDeltaTime;

        Vector2 rightDirection = GetRightDirection(forwardDirection);
        Vector2 sideVector = rightDirection * _sideDirection;

        bool sideBlocked = IsBlocked(sideVector, _sideCheckDistance);

        if (sideBlocked == true && Time.time >= _nextSideChangeTime)
        {
            _sideDirection *= -1;
            _nextSideChangeTime = Time.time + _sideChangeCooldown;

            sideVector = rightDirection * _sideDirection;
        }

        Vector2 forwardVelocity = forwardDirection * (_forwardSpeed * _sideKickForwardMultiplier);
        Vector2 sideVelocity = sideVector * _sideKickSpeed;

        bool sideKickTimeEnough = _sideKickTimer >= _minSideKickTime;
        bool frontClear = IsBlocked(forwardDirection, _frontCheckDistance) == false;

        if (sideKickTimeEnough == true && frontClear == true)
        {
            _moveState = MoveState.Forward;
            _sideKickTimer = 0f;

            return forwardDirection * _forwardSpeed;
        }

        return forwardVelocity + sideVelocity;
    }

    private void StartBounceBack()
    {
        _moveState = MoveState.BounceBack;
        _bounceTimer = _bounceBackTime;

        ChooseRandomSideDirection();

        _sideKickTimer = 0f;
    }

    private void StartSideKick()
    {
        _moveState = MoveState.SideKick;
        _sideKickTimer = 0f;
        _nextSideChangeTime = Time.time + _sideChangeCooldown;
    }

    private void ChooseRandomSideDirection()
    {
        Vector2 forwardDirection = _moveDirection.normalized;
        Vector2 rightDirection = GetRightDirection(forwardDirection);

        bool leftBlocked = IsBlocked(-rightDirection, _sideCheckDistance);
        bool rightBlocked = IsBlocked(rightDirection, _sideCheckDistance);

        if (leftBlocked == true && rightBlocked == true)
        {
            _sideDirection = Random.value < 0.5f ? -1 : 1;
        }
        else if (leftBlocked == true)
        {
            _sideDirection = 1;
        }
        else if (rightBlocked == true)
        {
            _sideDirection = -1;
        }
        else
        {
            _sideDirection = Random.value < 0.5f ? -1 : 1;
        }
    }

    private bool IsBlocked(Vector2 direction, float distance)
    {
        if (direction == Vector2.zero)
        {
            return false;
        }

        RaycastHit2D hit = Physics2D.CircleCast(
            _rigidBody.position,
            _checkRadius,
            direction.normalized,
            distance,
            _wallLayer
        );

        return hit.collider != null;
    }

    private Vector2 GetRightDirection(Vector2 forwardDirection)
    {
        return new Vector2(forwardDirection.y, -forwardDirection.x);
    }
}