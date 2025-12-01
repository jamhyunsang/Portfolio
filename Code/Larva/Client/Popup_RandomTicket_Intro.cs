using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Popup_RandomTicket_Intro : UIElement
{
    #region Cashed Object
    [SerializeField] private Animator Ani_Reward = null;
    [SerializeField] private Button Btn_Continue = null;
 
    [Header("Intro")]
    [SerializeField] private SkeletonGraphic SG_Box_Intro = null;
    [SerializeField] private Image Image_Box_Intro = null;
    [SerializeField] private Sprite[] Sprites_Box = null;

    [Header("Open")]
    [SerializeField] private SkeletonGraphic SG_Box_Close = null;
    [SerializeField] private SkeletonGraphic SG_Box_Open = null;
    [SerializeField] private Button Btn_Skip = null;
    [SerializeField] private Element_Hero Item = null;
    [SerializeField] private Text Text_Name = null;
    [SerializeField] private Text Text_LeftCount = null;
    [SerializeField] private GameObject Obj_Skip = null;
    [SerializeField] private GameObject Obj_Count = null;
    [SerializeField] private GameObject Obj_LeftCount = null;
    [SerializeField] private GameObject Obj_Upgradeable = null;
    [SerializeField] private GameObject Obj_FxFrontMaxGrade = null;
    [SerializeField] private GameObject Obj_FxBackMaxGrade = null;
    #endregion

    #region Member Property
    int ShowIndex = 0;
    List<GachaResultData> HeroList = null;
    eRewardSummaryType m_RewardSummaryType = eRewardSummaryType.None;
    int GachaType = 0;

    eRewardIntroState m_State = eRewardIntroState.Intro;
    bool IsAcitveButton = false;
    int m_SummaryGrade = 0;
    #endregion

    #region Override Method
    public override void Init()
    {
        Btn_Continue.onClick.AddListener(OnClick_Continue);
        Btn_Skip.onClick.AddListener(OnClick_Skip);
        Obj_Count.SetActive(false);
        Ani_Reward.enabled = false;
    }

    public override void OnClose()
    {
    }

    public override void OnOpen(List<object> Args)
    {
        SoundManager.Instance.Pause_BGM_RightNow();

        HeroList = (List<GachaResultData>)Args[0];
        m_RewardSummaryType = (eRewardSummaryType)Args[1];

        if(m_RewardSummaryType == eRewardSummaryType.Gacha)
            GachaType = (int)Args[2];

        m_SummaryGrade = HeroList.Select(Data => Data.Grade).Max();

        StartCoroutine(ShowIntro());
    }

    public override void OnRefresh()
    {
    }
    #endregion

    #region Member Method
    private bool IsAnimationEnd()
    {
        if (Ani_Reward.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            return true;

        return false;
    }

    private IEnumerator ShowIntro()
    {
        // 등급별 박스 이미지, 스킨 적용
        var FirstHero = HeroList[0];
        switch (FirstHero.Grade)
        {
            case 1:
                Image_Box_Intro.sprite = Sprites_Box[0];
                SG_Box_Intro.Skeleton.SetSkin("skin_02");
                break;
            case 2:
                Image_Box_Intro.sprite = Sprites_Box[3];
                SG_Box_Intro.Skeleton.SetSkin("skin_05");
                break;
            case 3:
                Image_Box_Intro.sprite = Sprites_Box[2];
                SG_Box_Intro.Skeleton.SetSkin("skin_04");
                break;
            case 4:
                Image_Box_Intro.sprite = Sprites_Box[1];
                SG_Box_Intro.Skeleton.SetSkin("skin_03");
                break;
            default:
                Image_Box_Intro.sprite = Sprites_Box[0];
                SG_Box_Intro.Skeleton.SetSkin("skin_02");
                break;
        }

        Text_LeftCount.text = $"{HeroList.Count - (ShowIndex + 1)}";
        Obj_LeftCount.SetActive(HeroList.Count - (ShowIndex + 1) > 0);

        AnimationStart();

        yield return null;

        yield return new WaitUntil(IsAnimationEnd);

        Ani_Reward.enabled = false;
        IsAcitveButton = false;
    }

    private IEnumerator ShowOpen()
    {
        m_State = eRewardIntroState.Open;
        if (HeroList[ShowIndex].Type == 1)
        {
            var HeroInfo = DataManager.GetTable<HeroInfo>(TableType.HeroInfo).Values.Where(Data => Data.Key == HeroList[ShowIndex].HeroKey).SingleOrDefault();

            if (HeroInfo.Grade == 4)
            {
                Obj_FxFrontMaxGrade.SetActive(true);
                Obj_FxBackMaxGrade.SetActive(true);
            }
            else
            {
                Obj_FxFrontMaxGrade.SetActive(false);
                Obj_FxBackMaxGrade.SetActive(false);
            }

            Text_Name.text = CharacterUtil.GetCharacterName(HeroList[ShowIndex].HeroKey);
        }
        else
        {
            var SuperVillainInfo = DataManager.GetTable<SuperVillainInfo>(TableType.SuperVillainInfo).Values.Where(Data => Data.Key == HeroList[ShowIndex].HeroKey).SingleOrDefault();

            if (SuperVillainInfo.Grade == 4)
            {
                Obj_FxFrontMaxGrade.SetActive(true);
                Obj_FxBackMaxGrade.SetActive(true);
            }
            else
            {
                Obj_FxFrontMaxGrade.SetActive(false);
                Obj_FxBackMaxGrade.SetActive(false);
            }

            Text_Name.text = CharacterUtil.GetCharacterName(HeroList[ShowIndex].HeroKey);
        }

        var HeroData = UserHero.Clone(User.GetHero(HeroList[ShowIndex].HeroUniqueKey));
        Element_Hero_Data Data = new Element_Hero_Data();
        Data.IsShowLv = true;
        Data.IsShowCP = true;
        Data.HeroKey = HeroData.Unique.HeroKey;
        Data.HeroType = HeroData.Type;
        Data.Grade = HeroData.Grade;
        Data.Lv = HeroData.Lv;
        Data.Exp = HeroData.Exp;
        Data.CP = HeroData.CP;
        Item.SetData_Outgame(Data);
        int NeedExp = CharacterUtil.GetHeroLvUpNeedExp(Data.Exp);
        int CurrentExp = Data.Exp - CharacterUtil.GetHeroExpFromLv(Data.Lv);
        if (NeedExp <= CurrentExp)
        {
            Obj_Upgradeable.SetActive(true);
        }
        else
        {
            Obj_Upgradeable.SetActive(false);
        }
        Text_LeftCount.text = $"{HeroList.Count - (ShowIndex + 1)}";
        Obj_LeftCount.SetActive(HeroList.Count - (ShowIndex + 1) > 0);
        Obj_Skip.SetActive(HeroList.Count - (ShowIndex + 1) > 0);

        // 등급별 상자 skin 적용
        switch (HeroData.Grade)
        {
            case 1:
                SG_Box_Close.Skeleton.SetSkin("skin_02");
                SG_Box_Open.Skeleton.SetSkin("skin_02");
                break;
            case 2:
                SG_Box_Close.Skeleton.SetSkin("skin_05");
                SG_Box_Open.Skeleton.SetSkin("skin_05");
                break;
            case 3:
                SG_Box_Close.Skeleton.SetSkin("skin_04");
                SG_Box_Open.Skeleton.SetSkin("skin_04");
                break;
            case 4:
                SG_Box_Close.Skeleton.SetSkin("skin_03");
                SG_Box_Open.Skeleton.SetSkin("skin_03");
                break;
            default:
                SG_Box_Close.Skeleton.SetSkin("skin_02");
                SG_Box_Open.Skeleton.SetSkin("skin_02");
                break;
        }

        AnimationStart();

        yield return null;

        yield return new WaitUntil(IsAnimationEnd);

        Ani_Reward.enabled = false;
        IsAcitveButton = false;
    }

    private void RewardEnd()
    {
        List<object> Args = new List<object>();

        switch (m_RewardSummaryType)
        {
            case eRewardSummaryType.Character:
                {
                    Args = new List<object>()
                    {
                        eRewardSummaryType.Character,
                        HeroList,
                        m_SummaryGrade
                    };
                }
                break;
            case eRewardSummaryType.Gacha:
                {
                    Args = new List<object>()
                    {
                        eRewardSummaryType.Gacha,
                        HeroList,
                        m_SummaryGrade,
                        GachaType
                    };
                }
                break;
            case eRewardSummaryType.SummonTicket:
                {
                    Args = new List<object>()
                    {
                        eRewardSummaryType.SummonTicket,
                        HeroList,
                        m_SummaryGrade
                    };
                }
                break;
        }


        UIManager.Instance.Open<Popup_Reward_Summary>(UI.Popup, "UI/Popup/Popup_Reward_summary", Args);
        UIManager.Instance.Close<Popup_RandomTicket_Intro>();
    }

    private void AnimationStart()
    {
        switch (m_State)
        {
            case eRewardIntroState.Intro:
                {
                    Ani_Reward.Play("Reward_Box_Intro", -1, 0.0f);
                }
                break;
            case eRewardIntroState.Open:
                {
                    Ani_Reward.Play("Reward_Box_Open", -1, 0.0f);
                }
                break;
        }
        Ani_Reward.enabled = true;
        IsAcitveButton = false;
    }

    private void AnimationSkip()
    {
        switch (m_State)
        {
            case eRewardIntroState.Intro:
                {
                    Ani_Reward.Play("Reward_Box_Intro", -1, 1.0f);
                }
                break;
            case eRewardIntroState.Open:
                {
                    Ani_Reward.Play("Reward_Box_Open", -1, 1.0f);
                }
                break;
        }
    }
    #endregion

    #region Button Event
    private void OnClick_Continue()
    {
        if (IsAcitveButton)
            return;

        IsAcitveButton = true;

        switch (m_State)
        {
            case eRewardIntroState.Intro:
                {
                    if (!IsAnimationEnd())
                    {
                        AnimationSkip();
                    }
                    else
                    {
                        StartCoroutine(ShowOpen());
                    }
                }
                break;
            case eRewardIntroState.Open:
                {
                    if (!IsAnimationEnd())
                    {
                        AnimationSkip();
                    }
                    else
                    {
                        if (ShowIndex < HeroList.Count - 1)
                        {
                            ShowIndex++;
                            StartCoroutine(ShowOpen());
                        }
                        else
                        {
                            RewardEnd();
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    private void OnClick_Skip()
    {
        if (IsAcitveButton)
            return;

        IsAcitveButton = true;

        RewardEnd();
    }
    #endregion
}
