using UnityEngine;
using UnityEngine.EventSystems;

public class DaniTech_SummonSpawner : MonoBehaviour
{
    [Header("소환 설정")]
    [SerializeField] private GameObject _summonPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _summonParent;

    [Header("탭 판정 설정")]
    [SerializeField] private float _maxTapDuration = 0.25f;
    [SerializeField] private float _maxTapMoveDistance = 20f;

    [Header("연속 소환 방지")]
    [SerializeField] private float _spawnCooldown = 0.1f;

    private Vector2 _pressStartPosition;
    private float _pressStartTime;
    private bool _isPressing;

    private float _lastSpawnTime;

    private void Update()
    {
        //HandleMouseInput();
        //HandleTouchInput();
        if(Input.GetKeyDown(KeyCode.Space))
    {
            Debug.Log("스페이스바로 강제 소환 시도");
            SpawnSummon();
        }

        HandleMouseInput();
        HandleTouchInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI())
            {
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
        if(Input.touchCount <= 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            if (IsPointerOverUI(touch.fingerId))
            {
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

        if(touch.phase == TouchPhase.Canceled)
        {
            _isPressing = false;
        }

    }


    private void TrySpawnByTap(Vector2 releasePosition)
    {
        float pressDuration = Time.time - _pressStartTime;
        float moveDistanec = Vector2.Distance(_pressStartPosition, releasePosition);

        bool isShortTap = pressDuration <= _maxTapDuration;
        bool isNotDragged = moveDistanec <= _maxTapMoveDistance;

        if (isShortTap == false)
        {
            return;
        }

        if(isNotDragged == false)
        {
            return;
        }

        if(Time.time < _lastSpawnTime + _spawnCooldown)
        {
            return;
        }


        SpawnSummon();
        _lastSpawnTime = Time.time;
    }


    private void SpawnSummon()
    {
        if(_summonPrefab == null)
        {
            Debug.LogError("소환 prefab이 연결되지 않았습니다");
            return;
        }

        if(_spawnPoint == null)
        {
            Debug.LogError("Spawn Point가 연결되지 않았습니다");
            return;
        }

        GameObject summonObject = Instantiate(_summonPrefab, _spawnPoint.position, Quaternion.identity, _summonParent);

        Debug.Log($"소환체 생성 : {summonObject.name}");

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
