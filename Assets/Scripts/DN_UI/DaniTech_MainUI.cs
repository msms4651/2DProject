using UnityEngine;

public class DaniTech_MainUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_MyProfile;
    [SerializeField] private DaniTechUIButton Btn_StartBattle;
    [SerializeField] private DaniTechUIButton Btn_MonsterSpawn;
    [SerializeField] private DaniTechUIButton Btn_OpenInventory;
    [SerializeField] private DaniTechUIButton Btn_OpenGameBook;

    [Header("스킬 버튼")]
    [SerializeField] private DaniTechUIButton Btn_NormalAttack;
    [SerializeField] private DaniTechUIButton Btn_FirstSkill;    //Btn_CircleAttack;  
    [SerializeField] private DaniTechUIButton Btn_SecondSkill;   //Btn_RayAttack;
    [SerializeField] private DaniTechUIButton Btn_ThirdSkill;    //Btn_ProjectileAttack;

    private void OnEnable()
    {
        Btn_MyProfile.BindOnClickButtonEvent(OnClick_OpenMyProfile);
        Btn_StartBattle.BindOnClickButtonEvent(OnClick_StartBattle);
        Btn_MonsterSpawn.BindOnClickButtonEvent(OnClicK_MonsterSpawn);
        Btn_OpenInventory.BindOnClickButtonEvent(OnClick_OpenInventory);
        Btn_OpenGameBook.BindOnClickButtonEvent(OnClick_OpenGameBook);



        //스킬
        Btn_NormalAttack.BindOnClickButtonEvent(OnClick_UseNormalAttack);
        Btn_FirstSkill.BindOnClickButtonEvent(OnClick_UseFirstSkill);
        Btn_SecondSkill.BindOnClickButtonEvent(OnClick_UseSecondSkill);
        Btn_ThirdSkill.BindOnClickButtonEvent(OnClick_UseThirdSkill);

    }

    public void OnClick_UseNormalAttack()
    {
        DaniTechGameManager.Inst.LocalPlayer.UseNormalAttack();
    }

    public void OnClick_UseFirstSkill()
    {
        DaniTechGameManager.Inst.LocalPlayer.UseFirstSkill();

    }

    public void OnClick_UseSecondSkill()
    {
        DaniTechGameManager.Inst.LocalPlayer.UseSecondSkill();

    }

    public void OnClick_UseThirdSkill()
    {
        DaniTechGameManager.Inst.LocalPlayer.UseThirdSkill();

    }



    public void OnClick_OpenGameBook()
    {
        DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.DNGameBookUI);
    }
    public void OnClick_OpenInventory()
    {
        DaniTechUIManager.Instance.OpenInventoryPopup();
        DaniTechGameManager.Inst.SaveData();
    }

    public void OnClick_OpenMyProfile()
    {
        //UIManager.Instance.OpenMyProfilePopup("character_hellena_01");
        DaniTechUIManager.Instance.OpenInventoryPopup();
        Debug.LogWarning("프로필 오픈");
    }

    public void OnClick_StartBattle()
    {
        DaniTechUIManager.Instance.OpenSimplePopup("배틀 스타트!");
        Debug.LogWarning("배틀 스타트");
    }

    public void OnClicK_MonsterSpawn()
    {
        Debug.LogWarning("몬스터 스폰");
    }



}
