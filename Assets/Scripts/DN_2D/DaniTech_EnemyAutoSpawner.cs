using UnityEngine;

public class DaniTech_EnemyAutoSpawner : MonoBehaviour
{
    [Header("적 생성 설정")]
    [SerializeField] private GameObject Prefab_Enemy;

    // 적이 생성될 기준 위치
    // Y, Z 위치는 이 Transform 값을 기준으로 고정
    [SerializeField] private Transform Transform_EnemySpawnPoint;

    [SerializeField] private Transform Transform_EnemyParent;

    [Header("자동 생성 설정")]
    // true면 자동 생성 켜짐, false면 자동 생성 꺼짐
    [SerializeField] private bool _isAutoSpawn = true;

    // 몇 초마다 적을 생성할지 정하는 값
    [SerializeField] private float _spawnInterval = 1.5f;

    // 한 번 생성 타이밍에 몇 마리를 만들지 정하는 값
    [SerializeField] private int _spawnCountPerInterval = 1;

    [Header("입구 범위 랜덤 생성 설정")]
    // true면 입구 왼쪽~오른쪽 사이에서 랜덤 생성
    // false면 EnemySpawnPoint 위치에서만 생성
    [SerializeField] private bool _useEntranceRandomXPosition = true;

    // 입구의 왼쪽 끝 위치
    // X값만 랜덤 범위 계산에 사용
    [SerializeField] private Transform Transform_EnemySpawnLeftPoint;

    // 입구의 오른쪽 끝 위치
    // X값만 랜덤 범위 계산에 사용
    [SerializeField] private Transform Transform_EnemySpawnRightPoint;

    // 다음 적 생성 시간을 저장하는 변수
    private float _nextSpawnTime = 0f;

    private void Start()
    {
        // 게임이 시작되자마자 바로 생성하지 않고,
        // _spawnInterval 시간이 지난 뒤 첫 적이 생성되도록 예약
        _nextSpawnTime = Time.time + _spawnInterval;
    }

    private void Update()
    {
        // 자동 생성 기능이 꺼져 있으면 아무것도 하지 않음
        if (_isAutoSpawn == false)
        {
            return;
        }

        // 아직 다음 생성 시간이 되지 않았으면 아무것도 하지 않음
        if (Time.time < _nextSpawnTime)
        {
            return;
        }

        // 생성 시간이 되었으므로 적 그룹 생성
        SpawnEnemyGroup();

        // 다음 적 생성 시간을 다시 예약
        _nextSpawnTime = Time.time + _spawnInterval;
    }

    private void SpawnEnemyGroup()
    {
        // _spawnCountPerInterval 값만큼 적을 반복 생성
        for (int i = 0; i < _spawnCountPerInterval; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        // 적 프리팹이 연결되지 않았으면 생성할 수 없으므로 중단
        if (Prefab_Enemy == null)
        {
            Debug.LogError("Enemy Prefab이 연결되지 않았습니다", this);
            return;
        }

        // 생성 기준 위치가 연결되지 않았으면 위치를 알 수 없으므로 중단
        if (Transform_EnemySpawnPoint == null)
        {
            Debug.LogError("Enemy Spawn Point가 연결되지 않았습니다", this);
            return;
        }

        // 실제로 적이 생성될 위치 계산
        Vector3 spawnPosition = GetEnemySpawnPosition();

        // Enemy 프리팹을 계산된 위치에 생성
        GameObject enemyObject = Instantiate(
            Prefab_Enemy,
            spawnPosition,
            Quaternion.identity,
            Transform_EnemyParent
        );

        // Hierarchy에서 보기 쉽게 이름 정리함
        enemyObject.name = "Enemy";

        // 생성이 잘 되었는지 Console에서 확인하기 위한 로그
        Debug.Log($"적 생성 : {enemyObject.name}");
    }

    private Vector3 GetEnemySpawnPosition()
    {
        // 기본 생성 위치는 EnemySpawnPoint 위치로 설정
        Vector3 spawnPosition = Transform_EnemySpawnPoint.position;

        // 입구 랜덤 생성 기능이 꺼져 있으면 기본 위치 그대로 반환
        if (_useEntranceRandomXPosition == false)
        {
            return spawnPosition;
        }

        // 왼쪽 기준점이 없으면 랜덤 범위를 만들 수 없으므로 기본 위치 반환
        if (Transform_EnemySpawnLeftPoint == null)
        {
            Debug.LogWarning("Enemy Spawn Left Point가 연결되지 않았습니다. EnemySpawnPoint 위치에서 생성합니다.", this);
            return spawnPosition;
        }

        // 오른쪽 기준점이 없으면 랜덤 범위를 만들 수 없으므로 기본 위치 반환
        if (Transform_EnemySpawnRightPoint == null)
        {
            Debug.LogWarning("Enemy Spawn Right Point가 연결되지 않았습니다. EnemySpawnPoint 위치에서 생성합니다.", this);
            return spawnPosition;
        }

        // 왼쪽/오른쪽 포인트 중 더 작은 X값을 랜덤 범위의 시작점으로 사용
        float minX = Mathf.Min(
            Transform_EnemySpawnLeftPoint.position.x,
            Transform_EnemySpawnRightPoint.position.x
        );

        // 왼쪽/오른쪽 포인트 중 더 큰 X값을 랜덤 범위의 끝점으로 사용
        float maxX = Mathf.Max(
            Transform_EnemySpawnLeftPoint.position.x,
            Transform_EnemySpawnRightPoint.position.x
        );

        // 입구 범위 안에서 랜덤 X좌표를 하나 선택
        float randomX = Random.Range(minX, maxX);

        // X는 랜덤값 사용
        spawnPosition.x = randomX;

        // Y는 EnemySpawnPoint 위치로 고정
        spawnPosition.y = Transform_EnemySpawnPoint.position.y;

        // Z도 EnemySpawnPoint 위치로 고정
        spawnPosition.z = Transform_EnemySpawnPoint.position.z;

        // 최종 생성 위치 반환
        return spawnPosition;
    }
}