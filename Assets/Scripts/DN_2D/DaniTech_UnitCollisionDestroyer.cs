using UnityEngine;
// 체력이 있으면 데미지만 받기
// 방어막이 있으면 소멸 안 하기
// 보스는 한 번에 안 죽기
// 폭발 이펙트 생성하기
// 사운드 재생하기

[RequireComponent (typeof(Collider2D))]
public class DaniTech_UnitCollisionDestroyer : MonoBehaviour
{
    [Header("충돌 대상 태그")]
    // 이 태그를 가진 오브젝트와 부딪히면 서로 사라지게함
    [SerializeField] private string _targetTag = "Enemy";

    [Header("소멸 안정 설정")]
    // 한 프레임에 여러 Enemy와 동시에 부딪혔을 때 중복 처리되는것을 막기 위한 값
    private bool _isDestroyRequested = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDestroyByCollider(collision.collider);
    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDestroyByCollider(other);
    }

    private void TryDestroyByCollider(Collider2D otherCollider)
    {
        if (_isDestroyRequested == true)
        {
            return;
        }
        GameObject targetObject = GetTargetObject(otherCollider);

        if (targetObject == null)
        {
            return;
        }

        bool isTarget = CheckTargetTag(targetObject, otherCollider);

        if (isTarget == false)
        {
            return;
        }

        DestroyBothObjects(targetObject);
    }


    private GameObject GetTargetObject(Collider2D otherCollider)
    {
        // Collider가 자식 오브젝트에 붙어 있고 Rigidbody2D가 부모에 있을 수 있음
        // 그래서 attachedRigidbody가 있으면 Rigidbody2D가 붙은 오브젝트를 실제 대상이라고 봄
        if (otherCollider.attachedRigidbody != null)
        {
            return otherCollider.attachedRigidbody.gameObject;
        }


        return otherCollider.gameObject;
    }



    private bool CheckTargetTag(GameObject targetObject, Collider2D otherCollider)
    {
        // 보통은 Rigidbody2D가 붙은 본체 오브젝트의 Tag를 확인
        if (targetObject.CompareTag(_targetTag) == true)
        {
            return true;
        }

        // 혹시 Tag가 Collider 자식 오브젝트에 들어간 경우도 대비
        if (otherCollider.gameObject.CompareTag(_targetTag) == true)
        {
            return true;
        }

        return false;
    }

    private void DestroyBothObjects(GameObject targetObject)
    {
        _isDestroyRequested = true;

        // Destroy가 실제로는 프레임 끝에서 처리되기 때문에,
        // 그 사이에 추가 충돌이 다시 들어오지 않도록 Collider를 먼저 꺼줌
        DisableAllColliders(this.gameObject);
        DisableAllColliders(targetObject);

        Destroy(targetObject);
        Destroy(this.gameObject);
    }

    private void DisableAllColliders(GameObject targetObject)
    {
        Collider2D[] colliderArray = targetObject.GetComponentsInChildren<Collider2D>();

        for (int i = 0; i < colliderArray.Length; i++)
        {
            colliderArray[i].enabled = false;
        }
    }
}
