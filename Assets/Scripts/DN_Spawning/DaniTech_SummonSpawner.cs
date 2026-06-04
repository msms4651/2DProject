using UnityEngine;
using UnityEngine.EventSystems;

public class DaniTech_SummonSpawner : MonoBehaviour
{
    [Header("소환 설정")]
    [SerializeField] private GameObject _summonPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _summonParent;


    
    [Header("카메라 설정")]
    [SerializeField] private Camera _spawnCamera;

    [Header("생성 직후 분산 설정")]
    // true면 소환될 때 정확히 같은 위치가 아니라 살짝 랜덤하게 흩어져서 생성됨
    [SerializeField] private bool _useSpawnRandomOffset = true;

    // 생성될 때 좌우로 퍼지는 범위
    // 값이 클수록 처음부터 좌우로 넓게 퍼짐
    [SerializeField] private float _spawnRandomXRange = 0.25f;


    [Header("탭 판정 설정")]
    [SerializeField] private float _maxTapDuration = 0.25f;
    [SerializeField] private float _maxTapMoveDistance = 20f;

    [Header("연속 소환 방지")]
    [SerializeField] private float _spawnCooldown = 0.1f;

    [Header("UI 클릭 차단")]
    [SerializeField] private bool _blockSpawnWhenPointerOverUI = false;

    private Vector2 _pressStartPosition;
    private float _pressStartTime;
    private bool _isPressing;

    private float _lastSpawnTime;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("스페이스바로 강제 소환 시도");
            SpawnSummon(Input.mousePosition);
        }

        HandleMouseInput();
        HandleTouchInput();
    }

    private void HandleMouseInput()
    {
        // 모바일 터치가 들어오는 중이면 마우스 입력은 무시
        // 모바일 환경에서 터치와 마우스 입력이 중복 처리되는 것을 방지
        if (Input.touchCount > 0)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_blockSpawnWhenPointerOverUI && IsPointerOverUI())
            {
                Debug.Log("UI 위 클릭으로 판단되어 소환하지 않음");
                return;
            }

            _isPressing = true;
            _pressStartPosition = Input.mousePosition;
            _pressStartTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0) && _isPressing)
        {
            _isPressing = false;

            Vector2 releasePosition = Input.mousePosition;
            TrySpawnByTap(releasePosition);
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount <= 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            if (_blockSpawnWhenPointerOverUI && IsPointerOverUI(touch.fingerId))
            {
                Debug.Log("UI 위 터치로 판단되어 소환하지 않음");
                return;
            }

            _isPressing = true;
            _pressStartPosition = touch.position;
            _pressStartTime = Time.time;
        }

        if (touch.phase == TouchPhase.Ended && _isPressing)
        {
            _isPressing = false;

            Vector2 releasePosition = touch.position;
            TrySpawnByTap(releasePosition);
        }

        if (touch.phase == TouchPhase.Canceled)
        {
            _isPressing = false;
        }
    }

    private void TrySpawnByTap(Vector2 releasePosition)
    {
        float pressDuration = Time.time - _pressStartTime;
        float moveDistance = Vector2.Distance(_pressStartPosition, releasePosition);

        bool isShortTap = pressDuration <= _maxTapDuration;
        bool isNotDragged = moveDistance <= _maxTapMoveDistance;

        if (isShortTap == false)
        {
            return;
        }

        if (isNotDragged == false)
        {
            return;
        }

        if (Time.time < _lastSpawnTime + _spawnCooldown)
        {
            return;
        }

        SpawnSummon(releasePosition);
        _lastSpawnTime = Time.time;
    }

    private void SpawnSummon(Vector2 screenPosition)
    {
        if (_summonPrefab == null)
        {
            Debug.LogError("소환 prefab이 연결되지 않았습니다");
            return;
        }

        if (_spawnPoint == null)
        {
            Debug.LogError("Spawn Point가 연결되지 않았습니다");
            return;
        }

        if(_spawnCamera == null)
        {
            _spawnCamera = Camera.main;
        }

        if(_spawnCamera == null)
        {
            Debug.LogError("Spawn Camere가 연결 되지 않았고 Main Camera도 찾을수 없습니다");
            return;
        }


        // [기존]
        // _spawnPoint.position에 정확히 생성하면 모든 캐릭터가 한 줄로 겹쳐서 생성됨
        //
        // [변경]
        // _spawnPoint.position에 랜덤 오프셋을 더해서
        // 생성 직후부터 캐릭터들이 살짝 흩어진 상태로 나오게 함
        Vector3 spawnPosition = GetSpawnPositionFromPointer(screenPosition);

        GameObject summonObject = Instantiate(
            _summonPrefab,
            spawnPosition,
            Quaternion.identity,
            _summonParent
        );

        Debug.Log($"소환체 생성 : {summonObject.name}");
    }

    private Vector3 GetSpawnPositionFromPointer(Vector2 screenPosition)
    {
        Debug.Log(
            "현재 소환 스크립트 오브젝트: " + gameObject.name +
            " / SpawnCamera: " + _spawnCamera +
            " / SpawnPoint: " + _spawnPoint,
            this
        );

        if (_spawnCamera == null)
        {
            Debug.LogError("Spawn Camera가 비어 있습니다. 이 오브젝트를 클릭해서 Inspector를 확인하세요: " + gameObject.name, this);
            return Vector3.zero;
        }

        if (_spawnPoint == null)
        {
            Debug.LogError("Spawn Point가 비어 있습니다. 이 오브젝트를 클릭해서 Inspector를 확인하세요: " + gameObject.name, this);
            return Vector3.zero;
        }

        float spawnPointDepthFromCamera =
            Mathf.Abs(_spawnCamera.transform.position.z - _spawnPoint.position.z);

        Vector3 screenPoint = new Vector3(
            screenPosition.x,
            screenPosition.y,
            spawnPointDepthFromCamera
        );

        Vector3 worldPoint = _spawnCamera.ScreenToWorldPoint(screenPoint);

        Vector3 randomOffset = GetSpawnRandomOffset();

        Vector3 fixedYSpawnPosition = new Vector3(
            worldPoint.x + randomOffset.x,
            _spawnPoint.position.y,
            _spawnPoint.position.z
        );

        return fixedYSpawnPosition;
    }



    private Vector3 GetSpawnRandomOffset()
    {
        // 랜덤 분산 기능을 끄면 원래처럼 정확히 SpawnPoint 위치에서 생성됨
        if (_useSpawnRandomOffset == false)
        {
            return Vector3.zero;
        }

        // 좌우 랜덤 위치
        float randomX = Random.Range(-_spawnRandomXRange, _spawnRandomXRange);

       

        return new Vector3(randomX, 0f, 0f);
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        return EventSystem.current.IsPointerOverGameObject();
    }

    private bool IsPointerOverUI(int fingerId)
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        return EventSystem.current.IsPointerOverGameObject(fingerId);
    }
}