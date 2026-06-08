using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DaniTech_GameBookSlotUI : MonoBehaviour
{

    [Header("슬롯 기본 정보")]
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private GameObject GObj_Selected;  //왜 이미지가 아니라 GameObject - > 활성 /비활성화 기능으로만 사용할거 라서
    [SerializeField] private DaniTechUIButton Button_SlotClick;


    private event Action<string> _onClickSlot;
    //private DNItemData _data; // 아이템 뿐만 아니라 다양한 데이터들을 도감에서 보여줄섯이므로


    private string _slotDataId; // 슬롯이 자기가 살아있는 동안 어떤 슬롯인지 DataId를 보관


    public string GetSlotDataId()
    {
        return _slotDataId;
    }
   


    private void OnEnable()
    {
        // 우리가 그냥 평소에 쓰던 버튼 클릭해줄려고 하는것 - 큰의미 x
        Button_SlotClick.BindOnClickButtonEvent(OnClick_GameBookSlot);
    }


    public void OnClick_GameBookSlot()
    {
        // 이게 오히려 중요 , 자식이 눌러졌는데 부모한테 알림
        _onClickSlot?.Invoke(_slotDataId);
    }

    public void OnDisable()
    {
        _onClickSlot = null;
    }


    public void InitSlot(string dataId, Action<string>  onClickCallback /*TableType*/)  // TODO : 카테고리에 따라 다른 데이터를 받아올수있도록 구별할 파라미터를 추가할 필요는 있다
    {
        var characterData = DaniTechGameDataManager.Instance.GetCharacterData( dataId );
        if (characterData == null)
        {
            Debug.LogWarning($"캐릭터 데이터를 찾을 수 없습니다. DataId: {dataId}", this);
            return;
        }

        // 이 슬롯이 어떤 캐릭터 데이터인지 먼저 저장
        _slotDataId = dataId;

        // 슬롯 클릭 시 부모 UI에게 알려줄 콜백 등록
        _onClickSlot += onClickCallback;

        // 슬롯에 캐릭터 이름 표시
        if (Text_MainName != null)
        {
            Text_MainName.text = characterData.Name;
        }

        string basicCostumeName = characterData.BasicCostumeName;

        Debug.Log(
            $"도감 슬롯 생성 / DataId: {dataId} / Name: {characterData.Name} / BasicCostumeName: {basicCostumeName}",
            this
        );

        if (string.IsNullOrEmpty(basicCostumeName) == true)
        {
            Debug.LogWarning($"캐릭터 이미지 주소가 비어 있습니다. DataId: {dataId}", this);
            return;
        }

        if (Image_MainIcon == null)
        {
            Debug.LogWarning("Image_MainIcon이 연결되지 않았습니다.", this);
            return;
        }

        // 이건 잘 만들어 둔거니까 묻지마 사용 암기... < Image에 아이콘, sprite리소스 불러와서 표기해줄
        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, basicCostumeName).Forget();



        // 데이터를 잘 받아 왔으면. 보관을 해주자
        _slotDataId = dataId;   // TODO 클릭 했을때. 위에서 Id를 통해서 부모한테 전달해줘야 하므로
                                // 나 아이템1번 데이터인데 눌러졌습니다.





        _onClickSlot += onClickCallback;  // 이벤트 등록완료

            

    }

    public void SetSelectedUI(bool isSelect)
    {
        GObj_Selected.SetActive(isSelect);
    }



   

}
