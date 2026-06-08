using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;




public class DaniTech_GameBookUI : DaniTechUIBase
{
    [Header("동적 생성할 프리팹")]

    [SerializeField] private GameObject prefab_Slot; //게임 오브젝트이지만, 프리팹이라는 단어를 명시

    [Header("디테일 정보 영역")]
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private Text Text_Description;
    [SerializeField] private DaniTechUIButton Button_CloseUI;
    [SerializeField] private GameObject GObj_DetailInfo;
    [SerializeField] private DaniTechUIButton Button_CloseDetailInfoUI;

    //[Header("부가 정보")]
    //[SerializeField] private GameObject Layout_SubInfoSkill;  // 그 안에 있는 UI요소를 직접 하나씩 껏다 켰다 하는게 아니라,그 레이아웃의 대표 오브젝트를만 껏다 켰다 하는게 압도적으로 편함

    [Header("슬롯 리스트 영역")]
    [SerializeField] private Transform Transform_SlotRoot; // 스크롤뷰에 슬롯이 생성될 수 있게 위치를 미리 지정해준다

    // 자료구조 추가
    private Dictionary<string, DaniTech_GameBookSlotUI> _slotList = new Dictionary<string, DaniTech_GameBookSlotUI>();

    private void OnEnable()
    {

        //도감이 열릴떄 상세창은 처음에 꺼짐
        SetDetailInfoActive(false);

        ReadItemListAndCreateSlot();

        if (Button_CloseUI != null)
        {
            Button_CloseUI.BindOnClickButtonEvent(OnClick_CloseGameBookUI);
        }
        else
        {
            Debug.LogWarning("Button_CloseUI가 연결되지 않았습니다.", this);
        }

    }

    public void OnClick_CloseGameBookUI()
    {
        Debug.Log("DNGameBookUI 닫기 버튼 클릭됨", this);

        if (DaniTechUIManager.Instance == null)
        {
            Debug.LogWarning("DaniTechUIManager.Instance가 없습니다", this);
            return;
        }

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

    private void SetDetailInfoActive(bool isActive)
    {
        if (GObj_DetailInfo == null)
        {
            Debug.LogWarning("GObj_DetailInfo가 연결되지 않았습니다.", this);
            return;
        }

        GObj_DetailInfo.SetActive(isActive);
    }

    // 상세창 닫기 
    public void OnClick_CloseDetailInfoUI()
    {
        Debug.Log("상세 정보 창 닫기 버튼 클릭됨", this);

        SetDetailInfoActive(false);
    }

    private void BindDetailInfoButtonEvent()
    {
        if (Button_CloseDetailInfoUI == null)
        {
            Debug.LogWarning("Button_CloseDetailInfoUI가 연결되지 않았습니다.", this);
            return;
        }

        // 혹시 같은 이벤트가 중복 등록되어 있을 수 있으니 먼저 한 번 제거
        Button_CloseDetailInfoUI.UnBindOnClickButtonEvent(OnClick_CloseDetailInfoUI);

        // 상세창이 다시 켜질 때마다 닫기 버튼 이벤트를 다시 연결
        Button_CloseDetailInfoUI.BindOnClickButtonEvent(OnClick_CloseDetailInfoUI);
    }

    private void ReadItemListAndCreateSlot()
    {
        // 데이터를 읽어와서 순회(foreach)를 돌면서, 아이템들을 도감 리스트에 표기


        var dataList = DaniTechGameDataManager.Instance.CharacterDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if (data == null) continue; // 데이터가 Null 일수 있으니 체크


            CreateGameBookSlot(data.Id);


        }

        //// 슬롯을 하나씩 눌러보는 코드
        //if (_slotList.Count > 0)
        //{
        //    foreach (var slotKv in _slotList)
        //    {
        //        var slot = slotKv.Value;
        //        slot.OnClick_GameBookSlot();
        //    }
        //}
        
            

                
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
        Debug.Log($"부모 도감 UI가 슬롯 선택을 받음 / SlotDataId: {slotDataId}", this);

        var currentSelectedData = DaniTechGameDataManager.Instance.GetCharacterData(slotDataId);

        if (currentSelectedData == null)
        {
            Debug.LogWarning($"선택한 캐릭터 데이터를 찾을 수 없습니다. DataId: {slotDataId}", this);
            return;
        }

        SetDetailInfoActive(true);

        BindDetailInfoButtonEvent();

        Text_MainName.text = currentSelectedData.Name;
        Text_Description.text = currentSelectedData.Description;

        string basicCostumeName = currentSelectedData.BasicCostumeName;

        if (Image_MainIcon == null)
        {
            Debug.LogWarning("Image_MainIcon이 연결되지 않았습니다.", this);
            return;
        }

        if (string.IsNullOrEmpty(basicCostumeName) == true)
        {
            Debug.LogWarning($"캐릭터 이미지 주소가 비어 있습니다. DataId: {slotDataId}", this);
            return;
        }

        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, basicCostumeName).Forget();

        foreach (var slotKv in _slotList)
        {
            var slot = slotKv.Value;
            var dataId = slot.GetSlotDataId();

            slot.SetSelectedUI(slotDataId == dataId);
        }

    }

}
