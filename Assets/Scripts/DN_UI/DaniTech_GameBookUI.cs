using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;




public class DaniTech_GameBookUI : DaniTechUIBase
{
    [Header("동적 생성할 프리팹")]

    [SerializeField] private GameObject prefab_Slot; //게임 오브젝트이지만, 프리팹이라는 단어를 명시

    [Header("디테일 정보 영역")]
    [SerializeField] private RawImage RawImage_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private Text Text_Description;
    [SerializeField] private DaniTechUIButton Button_CloseUI;

    //[Header("부가 정보")]
    //[SerializeField] private GameObject Layout_SubInfoSkill;  // 그 안에 있는 UI요소를 직접 하나씩 껏다 켰다 하는게 아니라,그 레이아웃의 대표 오브젝트를만 껏다 켰다 하는게 압도적으로 편함

    [Header("슬롯 리스트 영역")]
    [SerializeField] private Transform Transform_SlotRoot; // 스크롤뷰에 슬롯이 생성될 수 있게 위치를 미리 지정해준다

    // 자료구조 추가
    private Dictionary<string, DaniTech_GameBookSlotUI> _slotList = new Dictionary<string, DaniTech_GameBookSlotUI>();

    private void OnEnable()
    {
        // 이 UI가 열릴때 스스로, 기본적으로 아이템 도감 안에 있는 모든~~ 데이터를 불러온다
        ReadItemListAndCreateSlot();


        Button_CloseUI.BindOnClickButtonEvent(OnClick_CloseGameBookUI);

        //Button_CloseUI.

    }

    public void OnClick_CloseGameBookUI()
    {
        Debug.Log("도감에 닫기 버튼이 눌렸습니다!");

        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.DNGameBookUI);

    }


    private void OnDisable()
    {
        if(_slotList.Count > 0)
        {
            foreach(var slotKv in _slotList)
            {
                var slot = slotKv.Value; // 컴포넌트인데 , 얘로 gameObhect를 받아올수 있다

                DestroyImmediate(slot.gameObject);
            }

            _slotList.Clear();
        }
    }


    private void ReadItemListAndCreateSlot()
    {
        // 데이터를 읽어와서 순회(foreach)를 돌면서, 아이템들을 도감 리스트에 표기


        var dataList = DaniTechGameDataManager.Instance.ItemDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if (data == null) continue; // 데이터가 Null 일수 있으니 체크


            CreateGameBookSlot(data.Id);


        }


        if (_slotList.Count > 0)
        {
            foreach (var slotKv in _slotList)
            {
                var slot = slotKv.Value;
                slot.OnClick_GameBookSlot();
            }
        }
        
            

                
    }








    // 슬롯 1개만 제대로 생성해주는 로직 역할 메서드
    private void CreateGameBookSlot(string dadaId)  // 도감이 중복된다면 instanceId를 사용하는게 낫고 
                                                    // 중복되지 않는다면 dadaId가 낫다
    {
        var gObj = Instantiate(prefab_Slot, Transform_SlotRoot);
        if (gObj == null) return;

        // 게임 오브젝트는 동적생성이 됐다

        var slotComponent = gObj.GetComponent<DaniTech_GameBookSlotUI>();
        if (slotComponent == null) return;

        // 동적 생성된 자식 슬롯(게임오브젝트) 안에 있는 컴포넌트도 잘 가져왔다
        slotComponent.InitSlot(dadaId, OnclickChildSlotSelected);
        _slotList.Add(dadaId, slotComponent);



    }


    public void OnclickChildSlotSelected(string slotDataId)
    {
        var currentSelectedData = DaniTechGameDataManager.Instance.GetDNItemData(slotDataId);
        if(currentSelectedData == null) return;

        Text_MainName.text = currentSelectedData.Name;
        Text_Description.text = currentSelectedData.Description;
        //text_SellingPrice.text = currentSelectedData.SellingPrice;


        DaniTechGameUtil.LoadAndSetTexture(RawImage_MainIcon, currentSelectedData.IconPath).Forget();


        foreach (var slotKv in _slotList)
        {
            var slot = slotKv.Value;
            var dataId = slot.GetSlotDataId();
            slot.SetSelectedUI(slotDataId == dataId);

        }

    }

}
