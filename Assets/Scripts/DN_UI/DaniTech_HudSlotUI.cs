using UnityEngine;
using UnityEngine.UI;

public class DaniTech_HudSlotUI : MonoBehaviour
{
    [SerializeField] private int SlotOffsetY;

    [SerializeField] private GameObject Layout_TextArea;
    [SerializeField] private Text Text_Name;
    [SerializeField] private Slider Slider_Hp;
    [SerializeField] private Slider Slider_Mp;


    private int _instanceId;

    // 참조형을 기록(캐싱)
    private Transform _targetTransfrom;

    public void InitSlot(int instanceId, Transform targetTransform)
    {
        _instanceId = instanceId;
        _targetTransfrom = targetTransform;
        SlotOffsetY = 120;

        TryBindStatChangedEvect(targetTransform.gameObject);
    }

    private void TryBindStatChangedEvect(GameObject gObj)
    {
        // gObj가 몬스터거나 , 플레이어라면 GetComponent를 시도해보고, 잘 되면 그 곳에 있는 이벤트를 구독하자
        var player = gObj.GetComponent<DaniTech_2DPlayer>();
        if (player != null)
        {
            player.BindOnStatChangedEvect(OnTargetEntityHpChanged, OnTargetEntityMpChanged);

            return;
        }

        var monster = gObj.GetComponent<DaniTech_GameMoster>();
        if (monster != null)
        {
            monster.BindOnStatChangedEvect(OnTargetEntityHpChanged, OnTargetEntityMpChanged);
            return;
        
        }

    }

    private void OnTargetEntityHpChanged(int curHp, int maxHp)
    {
        Slider_Hp.value = (curHp / (float)maxHp);
    }

    private void OnTargetEntityMpChanged(int curMp, int maxMp)
    {
        Slider_Mp.value = (curMp / (float)maxMp);

    }

    private void Update()
    {
        // 참조형을 캐싱할때는 꼭 널체크를 사용부에서 신경써주자
        if(_targetTransfrom != null)
        {
            // this.gameObject.transform.position = _targetTransfrom.position;


            // World -> 스크린 좌표로
            Vector2 screenPos = Camera.main.WorldToScreenPoint(_targetTransfrom.position);

            // UGUI에서 사용하려고
            var rectTransform = this.GetComponent<RectTransform>();
            if(rectTransform != null)
            {
                Vector2 finalScreenPos = new Vector2(screenPos.x, screenPos.y - SlotOffsetY);
                rectTransform.anchoredPosition = finalScreenPos;
            }

        }
    }
}
