using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Gacha_Intro : UIElement
{
    #region Cashed Object
    [SerializeField] private Button Btn_Continue = null;
    [SerializeField] private Button Btn_Skip = null;
    [SerializeField] private GameObject Obj_Continue = null;

    [SerializeField] private GameObject Obj_NormalEffect = null;
    [SerializeField] private GameObject Obj_SpecialEffect = null;

    [SerializeField] private Animator Animator_Gacha = null;
    [SerializeField] private Animator Animator_Gacha_Fall = null;
    [SerializeField] private Animator Animator_Gacha_Can = null;

    #endregion

    #region Member Property
    List<GachaResultData> m_HeroList = null;
    private int m_GachaType;
    private int m_GachaUnique;
    #endregion

    #region Override Method
    public override void Init()
    {
        Btn_Continue.onClick.AddListener(OnClick_Continue);
        Btn_Skip.onClick.AddListener(OnClick_Skip);

        Obj_NormalEffect.SetActive(false);
        Obj_SpecialEffect.SetActive(false);
    }

    public override void OnClose()
    {
    }

    public override void OnOpen(List<object> Args)
    {
        SoundManager.Instance.Pause_BGM_RightNow();

        m_HeroList = (List<GachaResultData>)Args[0];
        m_GachaType = (int)Args[1];
        m_GachaUnique = (int)Args[2];

        var MaxGrade = m_HeroList.Select(Data => Data.Grade).Max();

        if (MaxGrade == 4)
            Obj_SpecialEffect.SetActive(true);
        else
            Obj_NormalEffect.SetActive(true);
    }

    public override void OnRefresh()
    {
    }
    #endregion

    #region Member Method
   
    #endregion

    #region Button Event
    private async void OnClick_Continue()
    {
        Animator_Gacha.enabled = false;
        Btn_Skip.gameObject.SetActive(false);
        Btn_Continue.interactable = false;
        Obj_Continue.SetActive(false);

        Animator_Gacha_Can.Play("Can_Open", 0, 0.0f);
        
        await UniTask.Delay(10);
        
        // 애님 시간 + 1초 대기
        var delay = Animator_Gacha_Can.GetCurrentAnimatorStateInfo(0).length;
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay + 1f));

        // 가챠 결과
        var Args = new List<object>() { m_HeroList, m_GachaType, m_GachaUnique };
        UIManager.Instance.Open<Popup_Reward_Summary_Card>(UI.Popup, "UI/Popup/Popup_Reward_summary_card", Args);

        UIManager.Instance.Close<Popup_Gacha_Intro>();
    }

    private void OnClick_Skip()
    {
        Animator_Gacha.Play(0, 0, 1f);
        Animator_Gacha_Fall.Play(0, 0, 1f);
    }
    #endregion
}
