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
    [SerializeField] private float _blockForwardMultiplier = 0.15f;
    [SerializeField] private float _decisionKeepTime = 0.25f;

   



    private Rigidbody2D _rigidBody;

    private int _avoidDirection;
    private float _nextDecisionTime;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        _rigidBody.gravityScale = 0f;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
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

            if(_avoidDirection == 0)
            {
                _rigidBody.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 sideDirection = Vector2.right * _avoidDirection;

            bool sideBlocked = IsBlocked(sideDirection, _sideCheckDistance);

            if (sideBlocked)
            {

                _avoidDirection = _avoidDirection * -1;
                sideDirection = Vector2.right * _avoidDirection;
                sideBlocked = IsBlocked(sideDirection, _sideCheckDistance);
            }

            if(sideBlocked == false)
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
        else
        {
            _avoidDirection = 0;
        }

        _rigidBody.linearVelocity = velocity;


    }

    // 좌 우 어느방향으로 피할지 결정
    private void ChooseAvoidDirection()
    {
        bool leftBlocked = IsBlocked(Vector2.left, _sideCheckDistance);
        bool rightBlocked = IsBlocked(Vector2.right, _sideCheckDistance);

        if (leftBlocked && rightBlocked)
        {
            _avoidDirection = 0;
        }
        else if (leftBlocked)
        {
            _avoidDirection = 1;
        }
        else if (rightBlocked)
        {
            _avoidDirection = -1;
        }
        else
        {
            _avoidDirection = Random.value < 0.5f ? -1 : 1;
        }

        _nextDecisionTime = Time.time + _decisionKeepTime;
    }

    // 특정 방향에 벽이 있는지 검사
    private bool IsBlocked(Vector2 direction, float distance)
    {
        if(direction == Vector2.zero)
        {
            return false;
        }

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _checkRadius, direction.normalized, distance, _wallLayer);


        return hit.collider != null;

    }

   





}
