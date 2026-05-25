using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class DaniTech_GameMoster : DaniTech_MosterBase
{
    [Header("몬스터 프리팹에서 미리 세팅할 데이터")]
    public float SkillCoolTime;
    public GameObject Prefab_thisMonsterSkillObjet;

    [SerializeField] private SpriteRenderer SpriteRenderer_Monster;




    [Header("데이터를 확인할 수 있도록 임시로 열어둠")]
    public int _instanceId; // 게임 오브젝트 매니저에서 찾기용(동사무소용) 게임에서 태어날떄 부여된 나의 주민등록번호
    public string _dataId; // 데이터 드리븐용 - 나의 부가정보를 나중에 찾을 수 있는 Id



    // DNMonsterData _thisMonsterData;
    [Header("전투에서 필요한 데이터")]
    private DNMonsterData _thisMonsterData;
    public int _baseHp;
    public int _baseAtk;
    public bool _isAlive = true;
    private bool _lookRight = true;


    private Vector3 _moveDirection;


    private void OnDisable()
    {
        _isAlive = false;
    }


    // 태어난 시점에서 어떤 정보를 저장해 주자
    public void InitMonster(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _dataId = dataId;


        // 초기화 한 다음에 그 객체가 가지고 있는 데이터를 이렇게 찾아와서 필요한 세팅을 해준다
        var monsterData = DaniTechGameDataManager.Instance.GetDNMonsterData(dataId);
        if (monsterData != null)
        {
            // 이 몬스터가 생성된 시점에서 자신의 엑셀 -> JSON을 거친 데이터를 캐싱해둔다
            _thisMonsterData = monsterData;
            _baseHp = _thisMonsterData.BaseHp;
            _baseAtk = _thisMonsterData.BaseAtk;
        }

        StartCoroutine(CheckAndUseSkill());

    }

    private int GetFinalNormalAtkDamage(int baseAtk, float normalAtkMultiple)
    {
        return GetFinalSkillDamage(baseAtk, normalAtkMultiple);
    }

    private int GetFinalSkillDamage(int baseAtk, float skillMultiple)
    {
        return (int)(baseAtk * skillMultiple);
    }

    // 코루틴이 등장 한다는건 -> 유니테스크로 호환이 가능하다
    // 일정시간마다 스킬을 사용할 예정
    // 스타트 코루틴은 이 몬스터가 생성된 시점에서 돌아도 됨!
    IEnumerator CheckAndUseSkill()
    {
        while (_isAlive)
        {
            yield return new WaitForSeconds(SkillCoolTime);

            if (_isAlive == false)
            {
                break;
            }

            ChangeMonsterDirection();
            UseSkill();

        }
    }

    void ChangeMonsterDirection()
    {
        _lookRight = !_lookRight;
        _moveDirection = new Vector3(_lookRight ? 1 : -1, 0, 0);
        SetMeshDirectionByMoveDirection((int)_moveDirection.x);
    }


    void SetMeshDirectionByMoveDirection(int x)
    {
        SpriteRenderer_Monster.flipX = (x < 0);
    }



    private void UseSkill()
      {
        // UI에서도 동적생성 했듯, 지금 스킬 투사체 오브젝트도 소환(실체화 - 동적생성)
        var gObj = Instantiate(Prefab_thisMonsterSkillObjet, DaniTechGameObjectManager.Inst.transform);
        if (gObj != null) return;

        var skillProjecttileComponent = gObj.GetComponent<DaniTech_SkillProjectile>();
        if (skillProjecttileComponent != null) return;

        // TODO : 추후 함수로 빠져야함
        float skillMultiple = _thisMonsterData.SkillAtkMultipleList.Count > 0 ? _thisMonsterData.SkillAtkMultipleList[0] : 0;
        int finalSkillDamage = GetFinalSkillDamage(_thisMonsterData.BaseAtk,skillMultiple);
        var tag = this.gameObject.tag;
        skillProjecttileComponent.InitSkillObject(_instanceId, _lookRight, this.transform.position, finalSkillDamage, tag, OnSkillCollision);
    }

    // 몬스터가 소환한 투사체의 충돌이 발생 했을 때 응답이 온다
    private void OnSkillCollision(int colliedObjectInstanceId, int damage)
    {
        if (colliedObjectInstanceId == 0) // 0이면 플레이어라는 규칙이 있으므로
        {
            var player = DaniTechGameObjectManager.Inst.GetLocalPlayer();

            // 이스킬이 충돌한 시점에서 다시한번 데미지를 계산해도 된다 - 기획적인 요소
            // float skillMultiple = _thisMonsterData.SkillAtkMultipleList.Count > 0 ? _thisMonsterData.SkillAtkMultipleList[0] : 0;
            // int finalSkillDamage = GetFinalSkillDamage(_thisMonsterData.BaseAtk, skillMultiple);


            player.TakeDamage(damage);
        }

    }


    //플레이어가 -> 몬스터한테 데미지를 줄떄 호출하는 함수
    public void TakeDamage(int playerDamage)
    {
        _baseHp -= playerDamage;

        // 피격 이펙트 같은거 활성화
        //SpriteRenderer_Damage.

        // 몬스터 죽음
        if(_baseHp < 0)
        {
            Destroy(this.gameObject);
        }

    }
}








    

