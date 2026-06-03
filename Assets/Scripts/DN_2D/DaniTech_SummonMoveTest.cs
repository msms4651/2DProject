using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class DaniTech_SummonMoveTest : MonoBehaviour
{
    [Header("자동 전진 설정")]
    [SerializeField] private float _forwardSpeed = 0.2f;


   

    [Header("이동 방향")]
    [SerializeField] private Vector2 _moveDirection = Vector2.up;

    [Header("벽 회피 설정")]
    [SerializeField] private LayerMask _wallLayer;

    [SerializeField] private float _frontCheckDistance = 0.3f;
    [SerializeField] private float _sideCheckDistance = 0.4f;
    [SerializeField] private float _checkRadius = 0.2f;

    [SerializeField] private float _avoidSideSpeed = 1.2f;
    [SerializeField] private float _blockForwardMultiplier = 0f;
    [SerializeField] private float _decisionKeepTime = 0.25f;

    [SerializeField] private float _stuckCheckTime = 0.4f;
    [SerializeField] private float _stuckMoveThreshold = 0.005f;
    [SerializeField] private float _stuckSideBoost = 1.4f;



    private Rigidbody2D _rigidBody;

    private int _avoidDirection;
    private float _nextDecisionTime;

    private Vector2 _lastPosition;
    private float _stuckTimer;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        _rigidBody.gravityScale = 0f;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        _lastPosition = _rigidBody.position;
    }

   

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 forwardDirection = _moveDirection.normalized;

        bool frontBlocked = IsBlocked(forwardDirection, _frontCheckDistance);

        Vector2 velocity = forwardDirection * _forwardSpeed;

        if (frontBlocked)
        {
            if(_avoidDirection == 0 || Time.time >= _nextDecisionTime)
            {
                ChooseAvoidDirection();
            }

            if (_avoidDirection == 0)
            {
                velocity = Vector2.zero;
            }
            else
            {
                Vector2 sideDirection = Vector2.right * _avoidDirection;

                bool sideBlocked = IsBlocked(sideDirection, _sideCheckDistance);

                if (sideBlocked)
                {

                    _avoidDirection = _avoidDirection * -1;
                    sideDirection = Vector2.right * _avoidDirection;
                    sideBlocked = IsBlocked(sideDirection, _sideCheckDistance);
                }

                if (sideBlocked == false)
                {
                    Vector2 slowForwardVelocity = forwardDirection * (_forwardSpeed * _blockForwardMultiplier);
                    Vector2 sideVelocity = sideDirection * _avoidSideSpeed;

                    velocity = slowForwardVelocity + sideVelocity;
                }
                else
                {
                    velocity = Vector2.zero;
                }
            }

            }
        else
        {
            _avoidDirection = 0;
        }

        float movedDistance = Vector2.Distance(_rigidBody.position, _lastPosition);

        if(movedDistance < _stuckMoveThreshold)
        {
            _stuckTimer += Time.fixedDeltaTime;
        }
        else
        {
            _stuckTimer = 0f;
        }

        if(_stuckTimer >= _stuckCheckTime)
        {
            if(_avoidDirection == 0)
            {
                _avoidDirection = Random.value < 0.5f ? -1 : 1;
            }
            else
            {
                _avoidDirection = _avoidDirection * -1;
            }

            Vector2 stuckEscapeDirection = Vector2.right * _avoidDirection;
            velocity = stuckEscapeDirection * (_avoidSideSpeed * _stuckSideBoost);

            _stuckTimer = 0f;
            _nextDecisionTime = Time.time + _decisionKeepTime;
        }

        _lastPosition = _rigidBody.position;

            _rigidBody.linearVelocity = velocity;




    }

    // 좌 우 어느방향으로 피할지 결정
    private void ChooseAvoidDirection()
    {
        float leftScore = GetOpenSideScore(-1);
        float rightScore = GetOpenSideScore(1);


        if (leftScore <= 0f && rightScore <= 0f)
        {
            _avoidDirection = 0;
        }
        else if (rightScore > leftScore )
        {
            _avoidDirection = 1;
        }
        else if (leftScore > rightScore)
        {
            _avoidDirection = -1;
        }
        else
        {
            _avoidDirection = Random.value < 0.5f ? -1 : 1;
        }

        _nextDecisionTime = Time.time + _decisionKeepTime;
    }

    private float GetOpenSideScore(int direction)
    {
        Vector2 forwardDirection = _moveDirection.normalized;
        Vector2 sideDirection = Vector2.right * direction;

        Vector2 centerPosition = _rigidBody.position;
        Vector2 sidePosition = centerPosition + sideDirection * _sideCheckDistance;
        Vector2 diagonalPosition = centerPosition + sideDirection * _sideCheckDistance + forwardDirection * _frontCheckDistance;

        float score = 0f;

        bool sideBlocked = Physics2D.CircleCast(
            centerPosition,
            _checkRadius,
            sideDirection,
            _sideCheckDistance,
            _wallLayer
        ).collider != null;

        bool diagonalBlocked = Physics2D.OverlapCircle(
            diagonalPosition,
            _checkRadius,
            _wallLayer
        ) != null;

        bool forwardFromSideBlocked = Physics2D.CircleCast(
            sidePosition,
            _checkRadius,
            forwardDirection,
            _frontCheckDistance,
            _wallLayer
        ).collider != null;

        if (sideBlocked == false)
        {
            score += 1f;
        }

        if (diagonalBlocked == false)
        {
            score += 1f;
        }

        if (forwardFromSideBlocked == false)
        {
            score += 2f;
        }

        return score;
    }


    // 특정 방향에 벽이 있는지 검사
    private bool IsBlocked(Vector2 direction, float distance)
    {
        if(direction == Vector2.zero)
        {
            return false;
        }

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _checkRadius, direction.normalized, distance, _wallLayer);
        // 직선레이로 변경
        // 한번충돌했을때 다음충돌에서 개선
        // 에이스타패스파인딩(길찾기알고리즘)  ai네비게이션

        return hit.collider != null;

    }

   





}
