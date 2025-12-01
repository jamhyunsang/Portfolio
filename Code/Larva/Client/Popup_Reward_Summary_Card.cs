using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Reward_Summary_Card : UIElement
{
    #region Cashed Object
    [SerializeField] private Button Btn_Close = null;
    [SerializeField] private GameObject Obj_ButtonGrid = null;

    [Header("Gacha Button")]
    [SerializeField] private Button Btn_GachaOne = null;
    [SerializeField] private Button Btn_GachaTen = null;

    [SerializeField] private Text Text_GachaOne_Price = null;
    [SerializeField] private Text Text_GachaTen_Price = null;

    [SerializeField] private Image Img_GachaOne_Price = null;
    [SerializeField] private Image Img_GachaTen_Price = null;

    [Header("One")]
    [SerializeField] private GameObject Obj_One = null;
    [SerializeField] private Element_Hero Element_One = null;
    [SerializeField] private GameObject Obj_Effect = null;
    [SerializeField] private Animation Anim_One = null;

    [Header("Ten")]
    [SerializeField] private GameObject Obj_Ten = null;
    [SerializeField] private Element_Hero[] Elements_Ten = null;
    [SerializeField] private GameObject[] Obj_Effects = null;
    [SerializeField] private Animation Anim_Ten = null;

    [Header("Tutorial Target")]
    [SerializeField] private TutorialTargetUI m_TutorialTargetUI = null;
    #endregion

    #region Member Property
    private List<GachaResultData> m_GachaList = null;

    private int m_GachaType;
    private int m_GachaUnique;
    private SummonInfo m_GachaInfo = null;
    private long m_UserCoinCount;

    private Color m_GachaPriceTextDefaultColor;
    private Color m_RedColor;
    #endregion

    #region Override Method
    public override void Init()
    {
        Btn_Close.onClick.AddListener(OnClick_Close);

        m_GachaPriceTextDefaultColor = Text_GachaOne_Price.color;

        Color color;
        if (ColorUtility.TryParseHtmlString("#FF2F2F", out color))
            m_RedColor = color;
    }

    public override async UniTask<TutorialTargetUI> GetTutorialTargetUI(eTutorialStep Step)
    {
        return m_TutorialTargetUI;
    }

    public override async void OnClose()
    {
        SoundManager.Instance.Play_BGM_RightNow();
    }

    // TODO : Item Key
    public override void OnOpen(List<object> Args)
    {
        AllActiveFalse();

        m_GachaList = (List<GachaResultData>)Args[0];
        m_GachaType = (int)Args[1];
        m_GachaUnique = (int)Args[2];

        ShowHeroList();

        SetGachaButton();
    }

    public override void OnRefresh()
    {
    }
    #endregion

    #region Member Method

    private async void ShowHeroList()
    {
        if (m_GachaList == null)
            return;

        if (m_GachaList.Count == 10)
        {
            Obj_Ten.SetActive(true);

            for (int Index = 0; Index < Elements_Ten.Length; ++Index)
            {
                var GachaData = m_GachaList[Index];
                var HeroData = UserHero.Clone(User.GetHero(GachaData.HeroUniqueKey));
                Element_Hero_Data Data = new Element_Hero_Data();
                Data.HeroKey = HeroData.Unique.HeroKey;
                Data.HeroType = HeroData.Type;
                Data.Grade = HeroData.Grade;
                Data.Lv = HeroData.Lv;
                Data.Exp = HeroData.Exp;
                Data.CP = HeroData.CP;
                Data.IsShowLv = true;
                Data.IsShowCP = true;
                Elements_Ten[Index].SetData_Outgame(Data);
                Obj_Effects[Index].SetActive(GachaData.Grade == 4);
            }

            await UniTask.Delay(System.TimeSpan.FromSeconds(Anim_Ten.clip.length));

            Obj_ButtonGrid.SetActive(true);
            Btn_Close.gameObject.SetActive(true);
        }
        else
        {
            Obj_One.SetActive(true);

            var GachaData = m_GachaList[0];
            var HeroData = UserHero.Clone(User.GetHero(GachaData.HeroUniqueKey));
            Element_Hero_Data Data = new Element_Hero_Data();
            Data.IsShowLv = true;
            Data.IsShowCP = true;
            Data.HeroKey = HeroData.Unique.HeroKey;
            Data.HeroType = HeroData.Type;
            Data.Grade = HeroData.Grade;
            Data.Lv = HeroData.Lv;
            Data.Exp = HeroData.Exp;
            Data.CP = HeroData.CP;

            Element_One.SetData_Outgame(Data);
            Obj_Effect.SetActive(GachaData.Grade == 4);

            await UniTask.Delay(System.TimeSpan.FromSeconds(Anim_One.clip.length));

            Obj_ButtonGrid.SetActive(true);
            Btn_Close.gameObject.SetActive(true);
        }

        if (TutorialManager.Instance.IsCurrentStep(eTutorialStep.Click_Larva_Summon_Close))
        {
            TutorialManager.Instance.ShowCurrentStep();
            Btn_GachaOne.enabled = false;
            Btn_GachaTen.enabled = false;
        }
    }

    private void SetGachaButton()
    {
        m_GachaInfo = DataManager.GetTable<SummonInfo>(TableType.SummonInfo).Values.SingleOrDefault(Data => Data.Key == m_GachaType && Data.Pickup_Unique_Key == m_GachaUnique);
        if (m_GachaInfo == null)
            return;

        var CoinCountData = ItemUtil.GetItemCount(m_GachaInfo.Price_2_Key);
        m_UserCoinCount = CoinCountData.OtherCount != -1 ? CoinCountData.Count + CoinCountData.OtherCount : CoinCountData.Count;

        var DefaultItemKey = m_GachaInfo.Price_2_Key;
        var DefaultPriceOne = m_GachaInfo.Price_2_One;
        var DefaultPriceTen = m_GachaInfo.Price_2_Ten;

        var HasDefaultItem = m_UserCoinCount > 0;
        var HasSufficientPriceOne = ContentsUtil.ValidateGachaPurchase(m_GachaInfo, ShopContents.GachaOne) != ePurchaseState.NotEnough;
        var HasSufficientPriceTen = ContentsUtil.ValidateGachaPurchase(m_GachaInfo, ShopContents.GachaTen) != ePurchaseState.NotEnough;

        if (!HasDefaultItem)
        {
            DefaultItemKey = m_GachaInfo.Price_1_Key;
            DefaultPriceOne = m_GachaInfo.Price_1_One;
            DefaultPriceTen = m_GachaInfo.Price_1_Ten;
        }

        Img_GachaOne_Price.sprite = ItemUtil.GetItemIcon(DefaultItemKey);
        Text_GachaOne_Price.text = TextUtil.ConvertKMB(DefaultPriceOne);

        Img_GachaTen_Price.sprite = ItemUtil.GetItemIcon(DefaultItemKey);
        Text_GachaTen_Price.text = TextUtil.ConvertKMB(DefaultPriceTen);

        Text_GachaOne_Price.color = HasSufficientPriceOne ? m_GachaPriceTextDefaultColor : m_RedColor;
        Text_GachaTen_Price.color = HasSufficientPriceTen ? m_GachaPriceTextDefaultColor : m_RedColor;

        Btn_GachaOne.onClick.AddListener(OnClick_GachaOne);
        Btn_GachaTen.onClick.AddListener(OnClick_GachaTen);

        Btn_GachaOne.gameObject.SetActive(true);
        Btn_GachaTen.gameObject.SetActive(true);
    }

    private void AllActiveFalse()
    {
        Btn_Close.gameObject.SetActive(false);
        Obj_ButtonGrid.SetActive(false);

        Btn_GachaOne.gameObject.SetActive(false);
        Btn_GachaTen.gameObject.SetActive(false);

        Obj_One.SetActive(false);
        Obj_Ten.SetActive(false);
    }

    private void HandleNotEnough()
    {
        var Message = new MessageData();
        Message.Type = PopupType.OkCancel;
        switch (m_GachaInfo.Price_1_Key)
        {
            case "Goods_Gem_Free":
            case "Goods_Gem_Price":
                Message.Message = GString.Get("UI_Message_0003");
                Message.ConfirmAction = () => { UIManager.Instance.QuickLink(UILink.Lobby_Shop_Gem); };
                break;
            case "Goods_Gold":
                Message.Message = GString.Get("UI_Message_0007");
                Message.ConfirmAction = () => { UIManager.Instance.QuickLink(UILink.Lobby_Shop_Gold); };
                break;
        }
        UIManager.Instance.OpenSystemPopup(Message);
    }

    private void ProceedToPurchase(ShopContents ShopContentsType)
    {
        var Datas = Util.MakeData($"{m_GachaInfo.Key}", $"{m_GachaInfo.Pickup_Unique_Key}");
        var Header = Util.MakeHeaderData(ShopContentsType, Datas);
        NetworkManager.Instance.SendContentsPacket(ContentsType.Shop, Header);
    }
    #endregion

    #region Button Event
    private void OnClick_Close()
    {
        UIManager.Instance.Close<Popup_Reward_Summary_Card>();
    }

    private void OnClick_GachaOne()
    {
        var PurchaseState = ContentsUtil.ValidateGachaPurchase(m_GachaInfo, ShopContents.GachaOne);

        switch (PurchaseState)
        {
            case ePurchaseState.Free:
            case ePurchaseState.WithOnlyCoin:
            case ePurchaseState.WithOnlyGem:
                ProceedToPurchase(ShopContents.GachaOne);
                break;
            case ePurchaseState.WithCoinAndGem:
                break;
            case ePurchaseState.NotEnough:
                HandleNotEnough();
                break;
        }
    }

    private void OnClick_GachaTen()
    {
        var PurchaseState = ContentsUtil.ValidateGachaPurchase(m_GachaInfo, ShopContents.GachaTen);
        switch (PurchaseState)
        {
            case ePurchaseState.Free:
            case ePurchaseState.WithOnlyCoin:
            case ePurchaseState.WithOnlyGem:
                ProceedToPurchase(ShopContents.GachaTen);
                break;
            case ePurchaseState.WithCoinAndGem:
                {
                    UIManager.Instance.Open<Popup_Coin_Consuming>(UI.Popup,
                        "UI/Popup/Popup_Coin_Consuming",
                        new List<object>
                        {
                        m_UserCoinCount,
                        m_GachaInfo.Price_1_Ten - m_UserCoinCount * (m_GachaInfo.Price_1_Ten / 10),
                        m_GachaInfo, ShopContents.GachaTen
                        });
                }
                break;
            case ePurchaseState.NotEnough:
                HandleNotEnough();
                break;
        }
    }
    #endregion
}
