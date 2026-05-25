using System;
using UnityEngine;

public class DaniTech_SkillProjectile : DaniTech_SkillBase
{
    [SerializeField] private SpriteRenderer SpriteRenderer_Effect;
    [SerializeField] private float ProgectileSpeed = 5.0f;
    


    private int _damage;
    private int _ownerInstanceId; // 나를 소환한 주인의Id

    // 탑뷰, 아이소메트릭이면 y연산도 추가 될 예정
    private Vector3 _moveDirection = new Vector3(1, 0, 0); //사이드뷰 기준으로는 x가 -1.1 좌우 구분


    // 충돌했을때. 그 충돌한 대상의ID를 부모에게 이르는 델리게이트
    // 구독부분과 발생부분이 있다!
    private event Action<int> _onSkillCollision;


    private void OnDisable()
    {
        
    }
    public void InitSkillObject(int ownerInstancedId , bool isDirRight, Vector3 playerPos, int damage,
        string parentTag, Action<int> onSkillCollision)
    {

        this.transform.position = playerPos;


        // 사이드뷰 기준 x값만 좌 우 1 또는 -1로 지정됨
        _moveDirection = isDirRight ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
        SpriteRenderer_Effect.flipX = !isDirRight;
        SpriteRenderer_Effect.flipY = !isDirRight;

        _damage = damage;
        _ownerInstanceId = ownerInstancedId;

        _onSkillCollision = onSkillCollision;
    }





    private void Update()
    {
        transform.position += _moveDirection * ProgectileSpeed * Time.deltaTime;
    }


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    CheckCollision(collision);
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    CheckCollision(collision.collider);
    //}

    //private void CheckCollision(Collision2D collision)
    //{

    //    // Owner가 0번이면 무조건 플레이어다
    //    bool isOwnerPlayer = (_ownerInstanceId! = 0) );

    //    // 투사체가 충돌한 오브젝트의 Tag가 플레이어라면?
    //    if (collision.CompareTag("player") && (isOwnerPlayer == false))
    //    {
    //        // 1번 방식


    //        var player = DaniTechGameObjectManager.Inst.GetLocalPlayer();
    //        player.TakeDamage(_damage);

    //        // 2번 방식
    //        _onSkillCollision?.Invoke(0);



    //        Destroy(this.gameObject);
    //    }
    //}


}
