using UnityEngine;



// Player가 게이트에 들어오고
// Player를 확인하고
// 이 게이트를 처음 통과했는지 확인하고
// 처음이면 통과 기록을 남기고
// 복제 캐릭터를 생성






[RequireComponent(typeof(Collider2D))]
public class Danitech_MultiplierGate : MonoBehaviour
{
    [Header("배수 결정")]
    [SerializeField] private int _multiplyValue = 2;

    [Header("생성 위치 설정")]
    [SerializeField] private float _spawnSpacingX = 0.25f;

    // 복제된 캐릭터를 살짝 뒤쪽에서 생성할지여부
    // 위로 전진하는 게임이면Y를 살짝 낮춰서 겹침을 줄일수 있음
    [SerializeField] private float _spawnBackOffsetY = -0.05f;

    [Header("부모 오브젝트")]
    [SerializeField] private Transform _summonParent;


    [Header("플레이어 판정")]
    [SerializeField] private string _PlayerTag = "Player";



    private Collider2D _gateCollider;
    private int _gateId;

    private void Awake()
    {
        _gateId = GetInstanceID();

        _gateCollider = GetComponent<Collider2D>();
        _gateCollider.isTrigger = true;

        if (_multiplyValue < 2)
        {
            _multiplyValue = 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject playerObject = GetPlayerObject(other);
        
        if(playerObject == null)
        {
            return;
        }

        if (playerObject.CompareTag(_PlayerTag) == false)
        {
            return;
        }

        DaniTech_GatePassHistory originalHistory = GetOrAddGatePassHistory(playerObject);

        if (originalHistory.HasPassedGate(_gateId))
        {
            return;
        }
        
        
        // 원본 캐릭터도 이 게이트를 통과했다고 기록
        originalHistory.MarkPassedGate(_gateId);

        CreateMultipliedPlayers(playerObject);
    }

    private GameObject GetPlayerObject(Collider2D other)
    {
        // player의 Collider가 자식 오브젝트에 붙어있는 경우를 대비
        if(other.attachedRigidbody != null)
        {
            return other.attachedRigidbody.gameObject;
        }
        return other.gameObject;
    }

    // 실제 복제 캐릭터를 생성
    private void CreateMultipliedPlayers(GameObject originalPlayer)
    {
        int createCount = _multiplyValue - 1;

        for (int i = 0;  i < createCount; i++)
        {
            Vector3 spawnOffset = GetSpawnOffset(i);

            Transform parentTransform = _summonParent;

            if(parentTransform == null)
            {
                parentTransform = originalPlayer.transform.parent;
            }

            GameObject clonePlayer = Instantiate(originalPlayer, originalPlayer.transform.position + spawnOffset
                , originalPlayer.transform.rotation, parentTransform);

            clonePlayer.name = originalPlayer.name + "_GateCopy";

            DaniTech_GatePassHistory cloneHistory = GetOrAddGatePassHistory(clonePlayer);
            cloneHistory.MarkPassedGate(_gateId);

            CopyRigidbodyVelocity(originalPlayer, clonePlayer);

        }
    }

    private Vector3 GetSpawnOffset(int index)
    {
        int sideDirection = index % 2 == 0 ? -1 : 1;
        int distanceStep = index / 2 + 1;
        float offsetX = sideDirection * _spawnSpacingX * distanceStep;
        float offsetY = _spawnBackOffsetY;

        return new Vector3(offsetX, offsetY, 0f);
    }

    private DaniTech_GatePassHistory GetOrAddGatePassHistory(GameObject playerObject)
    {
        DaniTech_GatePassHistory gatePassHistory = playerObject.GetComponent<DaniTech_GatePassHistory>();

        if (gatePassHistory == null)
        {
            gatePassHistory= playerObject.AddComponent<DaniTech_GatePassHistory>();
        }

        return gatePassHistory;
    }

    private void CopyRigidbodyVelocity(GameObject originalPlayer, GameObject clonePlayer)
    {
        Rigidbody2D originalRigidbody = originalPlayer.GetComponent<Rigidbody2D>();
        Rigidbody2D cloneRigidbody = clonePlayer.GetComponent<Rigidbody2D>();

        if(originalRigidbody == null)
        {
            return;
        }    

        if(cloneRigidbody == null)
        {
            return;
        }

        cloneRigidbody.linearVelocity = originalRigidbody.linearVelocity;
    }

}
