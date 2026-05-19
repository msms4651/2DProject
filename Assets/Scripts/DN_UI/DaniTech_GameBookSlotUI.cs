using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DaniTech_GameBookSlotUI : MonoBehaviour
{

    [Header("슬롯 기본 정보")]
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private GameObject GObj_Selected;//왜 이미지가 아니라 GameObject - > 활성 /비활성화
    [SerializeField] private DaniTechUIButton Button_SlotClick;


    private event Action<string> _onClickSlot;


    private string _slotDataId; // 슬롯이 자기가 살아있는 동안 어떤 슬롯인지 DataId를 보관

    private void OnEnable()
    {
        // 우리가 그냥 평소에 쓰던 버튼 클릭해줄려고 하는것 - 큰의미 x
        Button_SlotClick.BindOnClickButtonEvent(OnClick_GameBookSolt);
    }


    public void OnClick_GameBookSolt()
    {
        // 이게 오히려 중요 , 자식이 눌러졌는데 부모한테 알림
        _onClickSlot?.Invoke(_slotDataId);
    }


    public void InitSlot(string dataId, Action<string>  onClickCallback /*TableType*/)  // TODO : 카테고리에 따라 다른 데이터를 받아올수있도록 구별할 파라미터를 
    {
        var itemData = DaniTechGameDataManager.Instance.GetDNItemData( dataId );
        if (itemData == null) return;

        Text_MainName.text = itemData.Name; // 이름 반영
        
        string iconPath = itemData.IconPath;
        if (string.IsNullOrEmpty(iconPath) == true) return; // 혹시 기획자가 비웠을수 있으니


        // 이건 잘 만들어 둔거니까 묻지마 사용... < Image에 아이콘, sprite리소스 불러와서 표기해줄
        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, iconPath).Forget();



        // 데이터를 잘 받아 왔으면. 보관을 해주자
        _slotDataId = dataId;

        _onClickSlot += onClickCallback;  // 이벤트 등록완료

            
        // Text_MainName.text =
        // TODO 슬롯 로드가 들어갈 예정
        // Image_MainIcon.sprite = 


    }



    // TODO 클릭 했을때. 위에서 Id를 통해서 부모한테 전달해줘야 하므로
    // 나 아이템1번 데이터인데 눌러졌습니다.

}
