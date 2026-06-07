using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DaniTech_EnemyMoveTest : MonoBehaviour
{
    // 적의 현재 이동 상태를 구분하기 위한 enum
    // Forward: 아래로 전진
    // BounceBack: 벽을 만나면 잠깐 뒤로 튕김
    // SideKick: 좌우로 빠져나감
    private enum MoveState
    {
        Forward,
        BounceBack,
        SideKick
    }

    [Header("자동 전진 설정")]
    [SerializeField] private float _forwardSpeed = 1.2f;

    // 적은 위에서 아래로 내려와야 하므로 기본 방향은 Vector2.down
    [SerializeField] private Vector2 _moveDirection = Vector2.down;

    [Header("벽 감지 설정")]
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _frontCheckDistance = 0.35f;
    [SerializeField] private float _sideCheckDistance = 0.3f;
    [SerializeField] private float _checkRadius = 0.12f;

    [Header("뒤로 튕김 설정")]
    [SerializeField] private float _bounceBackSpeed = 0.8f;
    [SerializeField] private float _bounceBackTime = 0.1f;

    [Header("좌우 회피 설정")]
    [SerializeField] private float _sideKickSpeed = 1.3f;

    // 좌우로 피할 때도 아주 조금은 원래 진행 방향으로 움직이게 하는 값
    // 값이 너무 크면 벽에 계속 비비고, 너무 작으면 옆으로만 움직임
    [SerializeField] private float _sideKickForwardMultiplier = 0.12f;

    // 너무 빨리 다시 아래로 내려가지 않도록 최소 좌우 이동 시간을 둠
    [SerializeField] private float _minSideKickTime = 0.3f;

    // 좌우 방향을 너무 빠르게 바꾸지 않도록 막는 시간
    [SerializeField] private float _sideChangeCooldown = 0.25f;

    private Rigidbody2D _rigidBody;

    // 처음 상태는 아래로 전진하는 상태
    private MoveState _moveState = MoveState.Forward;

    // -1이면 왼쪽, 1이면 오른쪽으로 회피
    private int _sideDirection = 1;

    private float _bounceTimer = 0f;
    private float _sideKickTimer = 0f;
    private float _nextSideChangeTime = 0f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        // 2D 몹컨트롤 게임에서는 중력 때문에 떨어지면 안 되므로 0으로 고정
        _rigidBody.gravityScale = 0f;

        // 충돌해도 캐릭터가 빙글빙글 회전하지 않도록 Z 회전 고정
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 혹시 Inspector에서 방향을 0,0으로 잘못 넣었을 때 기본값을 아래로 다시 잡음
        if (_moveDirection == Vector2.zero)
        {
            _moveDirection = Vector2.down;
        }

        // Wall Layer를 비워두면 자동으로 "wall" 레이어를 찾아서 넣음
        if (_wallLayer.value == 0)
        {
            _wallLayer = LayerMask.GetMask("wall");
        }

        ChooseRandomSideDirection();
    }

    private void FixedUpdate()
    {
        // Rigidbody2D 이동은 Update보다 FixedUpdate에서 처리하는 게 안정적
        MoveEnemy();
    }

    private void MoveEnemy()
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
        // 진행 방향 앞쪽에 벽이 있는지 확인
        bool frontBlocked = IsBlocked(forwardDirection, _frontCheckDistance);

        if (frontBlocked == true)
        {
            StartBounceBack();

            // 벽을 만나면 진행 방향의 반대로 잠깐 튕김
            return -forwardDirection * _bounceBackSpeed;
        }

        // 벽이 없으면 원래 방향으로 전진
        return forwardDirection * _forwardSpeed;
    }

    private Vector2 MoveBounceBack(Vector2 forwardDirection)
    {
        _bounceTimer -= Time.fixedDeltaTime;

        // 뒤로 튕기는 시간이 끝나면 좌우 회피 상태로 변경
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

        // 현재 선택된 좌우 방향
        Vector2 sideVector = rightDirection * _sideDirection;

        // 옆 방향도 벽으로 막혔는지 확인
        bool sideBlocked = IsBlocked(sideVector, _sideCheckDistance);

        // 옆이 막혀 있으면 반대 방향으로 바꿈
        if (sideBlocked == true && Time.time >= _nextSideChangeTime)
        {
            _sideDirection *= -1;
            _nextSideChangeTime = Time.time + _sideChangeCooldown;

            sideVector = rightDirection * _sideDirection;
        }

        // 좌우로 피하면서 아주 조금은 아래 방향으로도 이동
        Vector2 forwardVelocity = forwardDirection * (_forwardSpeed * _sideKickForwardMultiplier);
        Vector2 sideVelocity = sideVector * _sideKickSpeed;

        bool sideKickTimeEnough = _sideKickTimer >= _minSideKickTime;
        bool frontClear = IsBlocked(forwardDirection, _frontCheckDistance) == false;

        // 최소 회피 시간이 지났고, 앞이 뚫렸으면 다시 전진 상태로 복귀
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

        // 벽을 만날 때마다 좌우 중 어디로 피할지 다시 선택
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

        // 왼쪽과 오른쪽이 둘 다 막혀 있으면 일단 랜덤 선택
        if (leftBlocked == true && rightBlocked == true)
        {
            _sideDirection = Random.value < 0.5f ? -1 : 1;
        }
        // 왼쪽이 막혔으면 오른쪽으로 이동
        else if (leftBlocked == true)
        {
            _sideDirection = 1;
        }
        // 오른쪽이 막혔으면 왼쪽으로 이동
        else if (rightBlocked == true)
        {
            _sideDirection = -1;
        }
        // 둘 다 비어 있으면 랜덤 선택
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

        // 현재 위치에서 direction 방향으로 원형 감지선을 쏴서 벽을 찾음
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
        // 진행 방향을 기준으로 오른쪽 방향을 계산
        // 적은 아래로 가므로, 이 계산을 통해 좌우 방향을 얻을 수 있음
        return new Vector2(forwardDirection.y, -forwardDirection.x);
    }

    private void OnDisable()
    {
        if (_rigidBody == null)
        {
            return;
        }

        // 비활성화될 때 혹시 남아있는 속도를 제거
        _rigidBody.linearVelocity = Vector2.zero;
    }
}