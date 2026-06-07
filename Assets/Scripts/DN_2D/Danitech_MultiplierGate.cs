using UnityEngine;

// 이 스크립트가 붙은 오브젝트에는 반드시 Collider2D가 필요함
// 게이트는 충돌로 막는 물체가 아니라 "통과 감지"용이므로 Trigger로 사용
[RequireComponent(typeof(Collider2D))]
public class Danitech_MultiplierGate : MonoBehaviour
{
    [Header("배수 설정")]
    // 2면 x2 게이트, 3이면 x3 게이트
    [SerializeField] private int _multiplyValue = 2;

    [Header("증식 대상 설정")]
    // 체크하면 Player 태그를 가진 아군이 이 게이트에서 증식함
    [SerializeField] private bool _canMultiplyPlayer = true;

    // 체크하면 Enemy 태그를 가진 적이 이 게이트에서 증식함
    [SerializeField] private bool _canMultiplyEnemy = false;

    [Header("태그 설정")]
    // 아군 판정용 태그
    [SerializeField] private string _playerTag = "Player";

    // 적 판정용 태그
    [SerializeField] private string _enemyTag = "Enemy";

    [Header("생성 위치 설정")]
    // 복제된 개체들이 좌우로 살짝 퍼지는 간격
    [SerializeField] private float _spawnSpacingX = 0.25f;

    // Player는 위로 올라가므로 복제체를 살짝 아래쪽에 생성
    [SerializeField] private float _playerSpawnBackOffsetY = -0.05f;

    // Enemy는 아래로 내려가므로 복제체를 살짝 위쪽에 생성
    [SerializeField] private float _enemySpawnBackOffsetY = 0.05f;

    [Header("부모 오브젝트")]
    // 복제된 아군이 들어갈 부모 오브젝트
    [SerializeField] private Transform Transform_SummonParent;

    // 복제된 적이 들어갈 부모 오브젝트
    [SerializeField] private Transform Transform_EnemyParent;

    private Collider2D _gateCollider;

    // 각 게이트마다 고유한 ID처럼 사용
    // 같은 캐릭터가 같은 게이트에서 무한 증식하는 것을 막기 위해 필요
    private int _gateId = 0;

    private void Awake()
    {
        _gateId = GetInstanceID();

        _gateCollider = GetComponent<Collider2D>();

        // 게이트는 벽처럼 막는 용도가 아니라 통과 감지용
        _gateCollider.isTrigger = true;

        // x1 게이트는 의미가 없으므로 최소 x2로 보정
        if (_multiplyValue < 2)
        {
            _multiplyValue = 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Collider가 자식에 붙어있어도 Rigidbody2D가 붙은 실제 본체 오브젝트를 가져오기 위함
        GameObject targetObject = GetTargetObject(other);

        if (targetObject == null)
        {
            return;
        }

        bool isPlayer = targetObject.CompareTag(_playerTag);
        bool isEnemy = targetObject.CompareTag(_enemyTag);

        // Player도 Enemy도 아니면 이 게이트에서는 무시
        if (isPlayer == false && isEnemy == false)
        {
            return;
        }

        // 이 게이트가 Player 증식을 허용하지 않으면 Player는 무시
        if (isPlayer == true && _canMultiplyPlayer == false)
        {
            return;
        }

        // 이 게이트가 Enemy 증식을 허용하지 않으면 Enemy는 무시
        if (isEnemy == true && _canMultiplyEnemy == false)
        {
            return;
        }

        DaniTech_GatePassHistory gatePassHistory = GetOrAddGatePassHistory(targetObject);

        // 이미 이 게이트를 통과한 개체라면 다시 증식하지 않음
        if (gatePassHistory.HasPassedGate(_gateId))
        {
            return;
        }

        // 원본 개체도 이 게이트를 통과했다고 기록
        gatePassHistory.MarkPassedGate(_gateId);

        // 대상 종류에 맞게 복제 생성
        if (isPlayer == true)
        {
            CreateMultipliedObjects(
                targetObject,
                Transform_SummonParent,
                _playerSpawnBackOffsetY
            );
        }
        else if (isEnemy == true)
        {
            CreateMultipliedObjects(
                targetObject,
                Transform_EnemyParent,
                _enemySpawnBackOffsetY
            );
        }
    }

    private GameObject GetTargetObject(Collider2D other)
    {
        // Rigidbody2D가 부모에 있고 Collider2D가 자식에 있을 수 있으므로
        // attachedRigidbody가 있다면 그 Rigidbody2D가 붙은 오브젝트를 본체로 사용
        if (other.attachedRigidbody != null)
        {
            return other.attachedRigidbody.gameObject;
        }

        return other.gameObject;
    }

    private void CreateMultipliedObjects(GameObject originalObject, Transform parentTransform, float spawnBackOffsetY)
    {
        // x2면 1개 추가 생성
        // x3면 2개 추가 생성
        int createCount = _multiplyValue - 1;

        for (int i = 0; i < createCount; i++)
        {
            Vector3 spawnOffset = GetSpawnOffset(i, spawnBackOffsetY);

            // 부모 오브젝트가 비어 있으면 원본과 같은 부모 아래에 생성
            Transform targetParent = parentTransform;

            if (targetParent == null)
            {
                targetParent = originalObject.transform.parent;
            }

            GameObject cloneObject = Instantiate(
                originalObject,
                originalObject.transform.position + spawnOffset,
                originalObject.transform.rotation,
                targetParent
            );

            cloneObject.name = originalObject.name + "_GateCopy";

            // 복제된 개체도 이 게이트를 이미 통과한 것으로 기록
            // 이렇게 해야 복제되자마자 같은 게이트에서 또 증식하지 않음
            DaniTech_GatePassHistory cloneHistory = GetOrAddGatePassHistory(cloneObject);
            cloneHistory.MarkPassedGate(_gateId);

            CopyRigidbodyVelocity(originalObject, cloneObject);
        }
    }

    private Vector3 GetSpawnOffset(int index, float spawnBackOffsetY)
    {
        // 0번째 복제체는 왼쪽, 1번째 복제체는 오른쪽
        int sideDirection = index % 2 == 0 ? -1 : 1;

        // x4 이상일 때 더 멀리 퍼지도록 거리 단계 계산
        int distanceStep = index / 2 + 1;

        float offsetX = sideDirection * _spawnSpacingX * distanceStep;
        float offsetY = spawnBackOffsetY;

        return new Vector3(offsetX, offsetY, 0f);
    }

    private DaniTech_GatePassHistory GetOrAddGatePassHistory(GameObject targetObject)
    {
        DaniTech_GatePassHistory gatePassHistory = targetObject.GetComponent<DaniTech_GatePassHistory>();

        if (gatePassHistory == null)
        {
            gatePassHistory = targetObject.AddComponent<DaniTech_GatePassHistory>();
        }

        return gatePassHistory;
    }

    private void CopyRigidbodyVelocity(GameObject originalObject, GameObject cloneObject)
    {
        Rigidbody2D originalRigidbody = originalObject.GetComponent<Rigidbody2D>();
        Rigidbody2D cloneRigidbody = cloneObject.GetComponent<Rigidbody2D>();

        if (originalRigidbody == null)
        {
            return;
        }

        if (cloneRigidbody == null)
        {
            return;
        }

        // 원본이 움직이던 속도를 복제체에도 그대로 넘김
        // 복제 직후 멈칫하지 않고 자연스럽게 같은 방향으로 이동하게 하기 위함
        cloneRigidbody.linearVelocity = originalRigidbody.linearVelocity;
    }
}