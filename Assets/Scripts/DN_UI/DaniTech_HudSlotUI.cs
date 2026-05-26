using UnityEngine;

public class DaniTech_HudSlotUI : MonoBehaviour
{
    private int _instanceId;

    // 참조형을 기록(캐싱)
    private Transform _targetTransfrom;

    public void InitSlot(int instanceId, Transform targetTransform)
    {
        _instanceId = instanceId;
        _targetTransfrom = targetTransform;
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
                rectTransform.anchoredPosition = screenPos;
            }

        }
    }
}
