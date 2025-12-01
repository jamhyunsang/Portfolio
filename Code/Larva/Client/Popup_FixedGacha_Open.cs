using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Popup_FixedGacha_Open : UIElement
{
    #region Cashed Object
    [SerializeField] private Element_Hero Item = null;
    [SerializeField] private Text Text_Name = null;
    [SerializeField] private GameObject Obj_Upgradeable = null;
    [SerializeField] private GameObject Obj_FxFrontMaxGrade = null;
    [SerializeField] private GameObject Obj_FxBackMaxGrade = null;
    [SerializeField] private SkeletonGraphic SG_Box_Close = null;
    [SerializeField] private SkeletonGraphic SG_Box_Open = null;

    [SerializeField] private Button Button_Continue = null;
    #endregion

    #region Member Property
    private List<GachaResultData> m_HeroList = null;
    private int m_Grade = 0;
    #endregion

    public override void Init()
    {
        Button_Continue.onClick.AddListener(OnClick_Continue);
    }

    public override void OnClose()
    {
    }

    public override void OnOpen(List<object> Args)
    {
        SoundManager.Instance.Pause_BGM_RightNow();

        m_HeroList = (List<GachaResultData>)Args[0];
        var Hero = m_HeroList.First();

        if (Hero.Type == 1)
        {
            var HeroInfo = DataManager.GetTable<HeroInfo>(TableType.HeroInfo).Values.Where(Data => Data.Key == Hero.HeroKey).SingleOrDefault();
            m_Grade = HeroInfo.Grade;

            if (m_Grade == 4)
            {
                Obj_FxFrontMaxGrade.SetActive(true);
                Obj_FxBackMaxGrade.SetActive(true);
            }
            else
            {
                Obj_FxFrontMaxGrade.SetActive(false);
                Obj_FxBackMaxGrade.SetActive(false);
            }

            Text_Name.text = CharacterUtil.GetCharacterName(Hero.HeroKey);
        }
        else
        {
            var SuperVillainInfo = DataManager.GetTable<SuperVillainInfo>(TableType.SuperVillainInfo).Values.Where(Data => Data.Key == Hero.HeroKey).SingleOrDefault();
            m_Grade = SuperVillainInfo.Grade;

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

            Text_Name.text = CharacterUtil.GetCharacterName(Hero.HeroKey);
        }

        // 등급별 상자 skin 적용
        switch (m_Grade)
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

        var HeroData = UserHero.Clone(User.GetHero(Hero.HeroUniqueKey));
        Element_Hero_Data Data = new Element_Hero_Data();
        Data.HeroKey = HeroData.Unique.HeroKey;
        Data.HeroType = HeroData.Type;
        Data.CP = HeroData.CP;
        Data.Exp = HeroData.Exp;
        Data.Lv = HeroData.Lv;
        Data.Grade = HeroData.Grade;
        Data.IsShowLv = true;
        Data.IsShowCP = true;
        Item.SetData_Outgame(Data);

        int NeedExp = CharacterUtil.GetHeroLvUpNeedExp(Data.Lv);
        int CurrentExp = Data.Exp - CharacterUtil.GetHeroExpFromLv(Data.Lv);
        if (NeedExp <= CurrentExp)
            Obj_Upgradeable.SetActive(true);
        else
            Obj_Upgradeable.SetActive(false);
    }

    public override void OnRefresh()
    {
    }

    #region Button Event
    private void OnClick_Continue()
    {
        List<object> Args = new List<object>
        {
            eRewardSummaryType.Character,
            m_HeroList,
            m_Grade
        };

        UIManager.Instance.Open<Popup_Reward_Summary>(UI.Popup, "UI/Popup/Popup_Reward_summary", Args);
        UIManager.Instance.Close<Popup_FixedGacha_Open>();
    }
    #endregion
}
