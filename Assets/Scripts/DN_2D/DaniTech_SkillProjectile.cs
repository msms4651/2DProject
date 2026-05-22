using UnityEngine;

public class DaniTech_SkillProjectile : DaniTech_SkillBase
{
    [SerializeField] private SpriteRenderer SpriteRenderer_Effect;
    [SerializeField] private float ProgectileSpeed = 5.0f;
    


    private int _damage;
    private int _ownerInstanceId; // 나를 소환한 주인의Id

    // 탑뷰, 아이소메트릭이면 y연산도 추가 될 예정
    private Vector3 _moveDirection = new Vector3(1, 0, 0); //사이드뷰 기준으로는 x가 -1.1 좌우 구분


    public void InitSkillObject(int ownerInstancedId , bool isDirRight, Vector3 playerPos, int damage)
    {

        this.transform.position = playerPos;


        // 사이드뷰 기준 x값만 좌 우 1 또는 -1로 지정됨
        _moveDirection = isDirRight ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
        SpriteRenderer_Effect.flipX = !isDirRight;
        SpriteRenderer_Effect.flipY = !isDirRight;

        _damage = damage;
        _ownerInstanceId = ownerInstancedId;
    }





    private void Update()
    {
        transform.position += _moveDirection * 0.5f * Time.deltaTime;
    }



}
