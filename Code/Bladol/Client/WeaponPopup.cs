using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPopup : UIElement
{
    #region Cashed Object
    [Header("HeroInfo")]
    [SerializeField] private Image Img_Hero = null;
    [SerializeField] private Image Img_HeroGrade = null;
    [SerializeField] private Image Img_HeroClass = null;
    [SerializeField] private Text Text_StarLevel = null;
    [SerializeField] private Text Text_HeroClass = null;
    [SerializeField] private Text Text_HeroRank = null;
    [SerializeField] private Text Text_HeroLevel = null;
    [SerializeField] private Text Text_HeroPower = null;
    [SerializeField] private Text Text_HeroName = null;

    [Header("WeaponInfo")]
    [SerializeField] private GameObject Obj_WeaponPos = null;
    [SerializeField] private List<Element_Stat> Stats = null;
    [SerializeField] private List<ThumnailItem> Items = null;
    [SerializeField] private List<GameObject> Obj_WeaponStars = null;

    [SerializeField] private GameObject Obj_WeaponInfo = null;
    [SerializeField] private GameObject Obj_WeaponGet = null;
    [SerializeField] private GameObject Obj_WeaponAwake = null;
    [SerializeField] private GameObject Obj_WeaponLevelUp = null;

    [SerializeField] private Text Text_CurLevel = null;
    [SerializeField] private Text Text_MaxLevel = null;
    [SerializeField] private Slider Slider_Exp = null;

    [SerializeField] private Button Btn_Get = null;
    [SerializeField] private Button Btn_Awake = null;
    [SerializeField] private Button Btn_LevelUp = null;
    [SerializeField] private Button Btn_Help = null;

    [Header("Common")]
    [SerializeField] private Top_Panel Top_Panel = null;
    #endregion

    #region Member Property
    UserHero m_CurHeroData = null;
    GameObject Obj_Weapon = null;
    #endregion

    #region Override Method
    public override void Init()
    {
        Top_Panel.RegistEvent(OnClick_Close);
        BackKeyManager.Instance.RegistEvent(OnClick_Close);

        Btn_Get.onClick.AddListener(OnClick_Awake);
        Btn_LevelUp.onClick.AddListener(OnClick_LevelUp);
        Btn_Awake.onClick.AddListener(OnClick_Awake);
        Btn_Help.onClick.AddListener(OnClick_Help);
    }

    public override void OnClose()
    {
        BackKeyManager.Instance.UnRegistEvent(OnClick_Close);
    }

    public override void OnOpen()
    {
    }

    public override void OnRefresh()
    {
        m_CurHeroData = UserHero.Clone(Player.GetHero(m_CurHeroData.Index));
        SetWeapon(m_CurHeroData);
        Top_Panel.Refresh();
    }
    #endregion

    #region Member Method
    public void SetWeapon(UserHero Data)
    {
        m_CurHeroData = Data;

        AllActiveFalse();
        SetHero();

        if (m_CurHeroData.ExclusiveWeapon.StarGrade == 0)
        {
            SetGet();
        }
        else
        {
            SetStat();
            SetAwake();
            SetLevel();
        }
    }

    private void AllActiveFalse()
    {
        for (int count = 0; count < Obj_WeaponStars.Count; count++)
            Obj_WeaponStars[count].SetActive(false);

        Obj_WeaponAwake.SetActive(false);
        Obj_WeaponGet.SetActive(false);
        Obj_WeaponLevelUp.SetActive(false);
        Obj_WeaponInfo.SetActive(false);
    }

    private void SetHero()
    {
        Text_HeroClass.text = HeroUtil.GetHeroClass(m_CurHeroData.Index);
        Text_HeroRank.text = $"+{m_CurHeroData.Rank}";
        Text_StarLevel.text = $"{m_CurHeroData.StarGrade}";

        Text_HeroLevel.text = $"{m_CurHeroData.HeroLv}";
        Text_HeroPower.text = $"{m_CurHeroData.CombatPower}";
        Text_HeroName.text = HeroUtil.GetHeroName(m_CurHeroData.Index);

        HeroUtil.GetHeroImage(m_CurHeroData.Index, CharacterImageType.Profile, out Sprite Icon);
        Img_Hero.sprite = Icon;

        HeroUtil.GetHeroGradeImage(m_CurHeroData.Index, out Icon);
        Img_HeroGrade.sprite = Icon;

        HeroUtil.GetHeroClassImageIndex(m_CurHeroData.Index, out Icon);
        Img_HeroClass.sprite = Icon;

        if(Obj_Weapon == null)
            Obj_Weapon = Instantiate(HeroUtil.GetHeroExclusiveWeaponIllust(m_CurHeroData.Index), Obj_WeaponPos.transform);
    }

    private void SetStat()
    {
        Obj_WeaponInfo.SetActive(true);
        for (int count = 0; count < Stats.Count; ++count)
        {
            string StatName = string.Empty;
            string Stat = string.Empty;
            switch (count)
            {
                case 0:
                    {
                        long Cur = Calculator.GetLongExclusiveWeaponCalcStat(StatType.HP, m_CurHeroData);
                        StatName = GString.Get("stat_hp");
                        Stat = $"{Cur}";
                    }
                    break;
                case 1:
                    {
                        long Cur = Calculator.GetLongExclusiveWeaponCalcStat(StatType.PAttack, m_CurHeroData);
                        StatName = GString.Get("stat_mdefence");
                        Stat = $"{Cur}";
                    }
                    break;
                case 2:
                    {
                        long Cur = Calculator.GetLongExclusiveWeaponCalcStat(StatType.PDefence, m_CurHeroData);
                        StatName = GString.Get("stat_pattack");
                        Stat = $"{Cur}";
                    }
                    break;
                case 3:
                    {
                        long Cur = Calculator.GetLongExclusiveWeaponCalcStat(StatType.Speed, m_CurHeroData);
                        StatName = GString.Get("stat_pdefence");
                        Stat = $"{Cur}";
                    }
                    break;
                case 4:
                    {
                        long Cur = Calculator.GetLongExclusiveWeaponCalcStat(StatType.MAttack, m_CurHeroData);
                        StatName = GString.Get("stat_mattack");
                        Stat = $"{Cur}";
                    }
                    break;
                case 5:
                    {
                        long Cur = Calculator.GetLongExclusiveWeaponCalcStat(StatType.MDefence, m_CurHeroData);
                        StatName = GString.Get("stat_speed");
                        Stat = $"{Cur}";
                    }
                    break;
            }

            Stats[count].SetStat(StatName, Stat);
        }
    }

    private void SetAwake()
    {
        Obj_WeaponAwake.SetActive(true);
        for(int count = 0; count < m_CurHeroData.ExclusiveWeapon.StarGrade; count++)
        {
            Obj_WeaponStars[count].SetActive(true);
        }
    }

    private void SetLevel()
    {
        Obj_WeaponLevelUp.SetActive(true);
        Text_CurLevel.text = $"{m_CurHeroData.ExclusiveWeapon.Level}";
        Text_MaxLevel.text = $"{HeroUtil.GetHeroExclusiveWeaponMaxLevel(m_CurHeroData.Index)}";

        var CurrentExp = HeroUtil.GetHeroExclusiveWeaponNeedExp(m_CurHeroData.ExclusiveWeapon.Level);
        var TotalExp = HeroUtil.GetHeroExclusiveWeaponNeedExp(m_CurHeroData.ExclusiveWeapon.Level + 1);
        Slider_Exp.value = Util.UniformVelocity_float(0f, 1f, m_CurHeroData.ExclusiveWeapon.Exp - CurrentExp, TotalExp - CurrentExp);
    }

    private void SetGet()
    {
        Obj_WeaponGet.SetActive(true);

        var ItemList = HeroUtil.GetHeroExclusiveWeaponMaterial(m_CurHeroData);

        for (int count = 0; count < ItemList.Count; count++)
        {
            ThumnailItemOption Option = new ThumnailItemOption();
            Option.ItemType = ItemList[count].Type;
            Option.ItemKey = ItemUtil.GetItemKey(ItemList[count].Type, ItemList[count].Index);
            Option.CountText = $"{Player.GetItemCount(Option.ItemType, Option.ItemKey)}/{ItemList[count].Count}";
            Option.ShowCount = true;
            Items[count].SetThumnailItem(Option);
        }
    }
    #endregion

    #region Button Event
    private void OnClick_Close()
    {
        SoundManager.Instance.Play_ResourceSE(eResourceSE.SE_BTN);

        UIManager.Instance.Close<WeaponPopup>();
    }

    private void OnClick_Get()
    {
        SoundManager.Instance.Play_ResourceSE(eResourceSE.SE_BTN);

        var ItemList = HeroUtil.GetHeroExclusiveWeaponMaterial(m_CurHeroData);

        for (int count = 0; count < ItemList.Count; count++)
        {
            if (Player.GetItemCount(ItemList[count].Type, ItemUtil.GetItemKey(ItemList[count].Type, ItemList[count].Index)) < ItemList[count].Count)
                return;
        }

        var Header = Util.MakeHeaderData(HeroContents.ExclusiveWeaponGradeUp, $"{m_CurHeroData.Index}");
        NetworkManager.Instance.SendContentsPacket(ContentsType.Hero, Header);
    }

    private void OnClick_Awake()
    {
        SoundManager.Instance.Play_ResourceSE(eResourceSE.SE_BTN);

        if (m_CurHeroData.ExclusiveWeapon.StarGrade >= HeroUtil.GetHeroExclusiveWeaponMaxGrade(m_CurHeroData.Index))
        {
            UIManager.Instance.ShowToast(GString.Get("notice_0180"));
            return;
        }
        var Popup = UIManager.Instance.Open<HeroEnforcePopup>(UI.PopUp, "Popup/Herocontents/HeroEnforcePopup");
        Popup.SetWindow(eEnforce.ExclusiveAwake, m_CurHeroData);
    }

    private void OnClick_LevelUp()
    {
        SoundManager.Instance.Play_ResourceSE(eResourceSE.SE_BTN);

        if (m_CurHeroData.ExclusiveWeapon.Level >= HeroUtil.GetHeroExclusiveWeaponMaxLevel(m_CurHeroData.Index))
        {
            if(m_CurHeroData.ExclusiveWeapon.StarGrade >= HeroUtil.GetHeroExclusiveWeaponMaxGrade(m_CurHeroData.Index))
                UIManager.Instance.ShowToast(GString.Get("notice_0165"));
            else
                UIManager.Instance.ShowToast(GString.Get("notice_0182"));

            return;
        }

        var Popup = UIManager.Instance.Open<HeroEnforcePopup>(UI.PopUp, "Popup/Herocontents/HeroEnforcePopup");
        Popup.SetWindow(eEnforce.ExclusiveLevelUp, m_CurHeroData);
    }

    private void OnClick_Help()
    {
        SoundManager.Instance.Play_ResourceSE(eResourceSE.SE_BTN);
        MessageOption Option = new MessageOption();
        Option.TitleMessage = GString.Get("message_hero_0008");
        Option.MainMessage = GString.Get("message_hero_0006");
        Option.MessageType = MessagePopupType.Ok;
        UIManager.Instance.ShowMessage(Option);
    }
    #endregion
}
