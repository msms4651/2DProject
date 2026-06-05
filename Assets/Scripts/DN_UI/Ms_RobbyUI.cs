using UnityEngine;


// UIBase -> 까먹을까봐 모노대ㅐ신 UIBase를 상속하자
public class Ms_RobbyUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Button_GameStart;
    [SerializeField] private DaniTechUIButton Button_GameQuit;
    //[SerializeField] private DaniTechUIButton Button_TestButton;



    private void OnEnable()
    {
        Button_GameStart.BindOnClickButtonEvent(OnClick_GameStart);
        Button_GameQuit.BindOnClickButtonEvent(OnClick_GameQuit);
        //Button_TestButton.BindOnClickButtonEvent(OnClick_TestButton);
    }



    //OnClick_, OnInputStringChanged_, <--- 유니티 에디터에서 등록할수있는거긴한데 어쩌구,,
    // 유니티 에서 원래 등록되었어야 하는 버튼 이벤트 인데 , 유니티 에디터에서 등록하면
    // 나중에 어떤 프리팹에서 오는건지찾기도 힘들고 관리도 힘드니까 이렇게 코드에서 등록해준다
    public void OnClick_GameStart()
    {
        //게임 시작에 대한 처리를 여기서 몰아서 해줄수가 있게 된다.

        //DaniTechGameManager.Inst.게임 시작할때 맵구성이나 부가적인요소를 여기에 해도된다


        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.DNRobbyUI);
    }


    public void OnClick_GameQuit()
    {
        // 게임 종료에 대한 처리를 여기서 몰아서 해줄수가 있게 된다.
        // 게임 매니저 만들어 놨으니까 걔한테만 부탁한다. 끝
        DaniTechGameManager.Inst.SaveAndEndGame();  

    }

    public void OnClick_TestButton()
    {
        DaniTechGameManager.Inst.SaveAndEndGame();

    }
}
