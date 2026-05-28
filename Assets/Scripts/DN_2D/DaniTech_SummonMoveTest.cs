using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class DaniTech_SummonMoveTest : MonoBehaviour
{
    [Header("자동 전진 설정")]
    [SerializeField] private float _forwardSpeed = 3f;


    [Header("좌우 테스트 이동 설정")]
    [SerializeField] private float _sideMoveSpeed = 5f;

    [Header("이동 방향")]
    [SerializeField] private Vector2 _moveDirection = Vector2.up;

    private Rigidbody2D _rigidBody;
    private float _horizontalInput;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        _rigidBody.gravityScale = 0f;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        _horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _horizontalInput = -1f;
        } 

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _horizontalInput += 1f;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 forwardVelocity = _moveDirection.normalized * _forwardSpeed;
        Vector2 sideVelocity = Vector2.right * (_horizontalInput * _sideMoveSpeed);

        _rigidBody.linearVelocity = forwardVelocity + sideVelocity;
    }






}
