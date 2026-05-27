using System.Collections.Generic;
using UnityEngine;

public class DaniTech_HudUI : DaniTechUIBase
{
    [SerializeField] private GameObject Prefab_HudSlot;
    [SerializeField] private Transform Transform_SlotRoot;

     // [SerializeField] private GameObject Prefab_HudSlot_Monster; //UI가 다르다면 별도로 추가 해도 됨

     private Dictionary<int, DaniTech_HudSlotUI> _hudslotList = new Dictionary<int, DaniTech_HudSlotUI>();


    public void AddHudSlot(int instanceId, Transform targerTransform)
    {
        CreateHudSlot(instanceId, targerTransform);
    }

    private void CreateHudSlot(int instanceId, Transform targerTransform)
    {
        var gObj = Instantiate(Prefab_HudSlot, Transform_SlotRoot);
        if (gObj == null) return;

        // 게임 오브젝트는 동적생성이 됐다

        var slotComponent = gObj.GetComponent<DaniTech_HudSlotUI>();
        if (slotComponent == null) return;

        //// 동적 생성된 자식 슬롯(게임오브젝트) 안에 있는 컴포넌트도 잘 가져왔다
        slotComponent.InitSlot(instanceId, targerTransform);

        _hudslotList.Add(instanceId, slotComponent);
    }

    public void RemoveHudSlot(int instanceId)
    {
        if(_hudslotList.ContainsKey(instanceId) == true)
        {
            var slot = _hudslotList[instanceId];
            // Destroy는 컴포넌트인 slot이 아니라 slot.gameObject

            Destroy(slot.gameObject);

            _hudslotList.Remove(instanceId);

        }
    }


}
