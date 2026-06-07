using UnityEngine;

public class DaniTech_EnemyAutoSpawner : MonoBehaviour
{
    [Header("적 생성 설정")]
    [SerializeField] private GameObject Prefab_Enemy;
    [SerializeField] private Transform Transform_EnemySpawnPoint;
    [SerializeField] private Transform Transform_EnemyParent;

    [Header("자동 생성 설정")]
    [SerializeField] private bool _isAutoSpawn = true;
    [SerializeField] private float _spawnInterval = 1.5f;
    [SerializeField] private int _spawnCountPerInterval = 1;

    [Header("입구 범위  랜덤 생성 설정")]
    [SerializeField] private bool _useEntranceRandomXPosition = true;
    [SerializeField] private Transform Transform_EnemySpawnLeftPoint;
    [SerializeField] private Transform Transform_EnemySpawnRightPoint;

    private float _nextSpawnTime = 0f;

    private void Start()
    {
        _nextSpawnTime = Time.time + _spawnInterval;
    }

    private void Update()
    {
        if(_isAutoSpawn == false)
        {
            return;
        }

        if(Time.time < _nextSpawnTime)
        {
            return;
        }

        SpawnEnemyGroup();

        _nextSpawnTime = Time.time + _spawnInterval;

    }

    private void SpawnEnemyGroup()
    {
        for (int i = 0; i < _spawnCountPerInterval; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if(Prefab_Enemy == null)
        {
            Debug.LogError("Enemy Prefab이 연결되지 않았습니다", this);
            return;
        }

        if(Transform_EnemySpawnPoint == null)
        {
            Debug.LogError("Enemy Spawn Point가 연결되지 않았습니다", this);
            return;
        }

        Vector3 spawnPosition = GetEnemySpawnPosition();

        GameObject enemyObject = Instantiate(
            Prefab_Enemy,
            spawnPosition,
            Quaternion.identity,
            Transform_EnemyParent);

        enemyObject.name = "Enemy";

        Debug.Log($"적 생성 : {enemyObject.name}");
    }

    private Vector3 GetEnemySpawnPosition()
    {
        Vector3 spawnPosition = Transform_EnemySpawnPoint.position;


        if (_useEntranceRandomXPosition == false)
        {
            return spawnPosition;
        }

        if (Transform_EnemySpawnLeftPoint == null)
        {
            Debug.LogWarning("Enemy Spawn Left Point가 연결되지 않았습니다. EnemySpawnPoint 위치에서 생성합니다.", this);
            return spawnPosition;
        }


        if (Transform_EnemySpawnRightPoint == null)
        {
            Debug.LogWarning("Enemy Spawn Right Point가 연결되지 않았습니다. EnemySpawnPoint 위치에서 생성합니다.", this);
            return spawnPosition;
        }

        float minX = Mathf.Min(
            Transform_EnemySpawnLeftPoint.position.x,
            Transform_EnemySpawnRightPoint.position.x
        );



        float maxX = Mathf.Max(
            Transform_EnemySpawnLeftPoint.position.x,
            Transform_EnemySpawnRightPoint.position.x
        );

        float randomX = Random.Range(minX, maxX);


        spawnPosition.x = randomX;
        spawnPosition.y = Transform_EnemySpawnPoint.position.y;
        spawnPosition.z = Transform_EnemySpawnPoint.position.z;


        return spawnPosition;
    }
}

