using System;
using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class GachaCellView : EnhancedScrollerCellView
{
    #region Cashed Object
    [SerializeField] private GameObject Obj_GachaPos = null;
    [SerializeField] private Text Text_GachaName = null;
    [SerializeField] private Button Btn_GachaOne = null;
    [SerializeField] private Button Btn_GachaTen = null;
    //[SerializeField] private Button Btn_Gacha30 = null;
    [SerializeField] private Text Text_GachaOne_Price = null;
    [SerializeField] private Text Text_GachaTen_Price = null;
    //[SerializeField] private Text Text_Gacha30_Price = null;
    [SerializeField] private Image Img_GachaOne_Price = null;
    [SerializeField] private Image Img_GachaTen_Price = null;
    //[SerializeField] private Image Img_Gacha30_Price = null;
    [SerializeField] private GameObject Obj_PickUpInfo = null;
    [SerializeField] private GameObject Obj_NormalInfo = null;
    [SerializeField] private GameObject Obj_PickUpPos = null;
    [SerializeField] private Text Text_PickUpLeftTime = null;
    
    [Header("Gacha Reward")]
    [SerializeField] private Text Text_AccumulateCount;
    [SerializeField] private Text Text_GachaCount;
    [SerializeField] private Slider Slider_GachaCount;
    [SerializeField] private Text[] Array_GachaRewardCount;
    [SerializeField] private Element_Slot[] Array_GachaRewardSlot;
    [SerializeField] private GameObject[] Arr_ReadyFX;
    
    [Header("Tutorial Target")]
    [SerializeField] private TutorialTargetUI TutorialTargetUI = null;

    [SerializeField] private Button Btn_Probability = null;

    [SerializeField] private GameObject Obj_GachaTip = null;
    #endregion

    #region Member Property
    private SummonInfo m_Info = null;
    private GameObject Obj_Gacha = null;
    private GameObject Obj_PickUp = null;
    private List<int> m_GachaCountList = null;
    private int m_UserGachaCount = 0;
    private List<ItemData> m_RewardItemList;
    private Color m_GachaPriceTextDefaultColor;
    private Color m_RedColor;
    private long m_UserCoinCount = 0;
    #endregion

    #region Unity Method
    private void Awake()
    {
        Btn_GachaOne.onClick.AddListener(OnClick_GachaOne);
        Btn_GachaTen.onClick.AddListener(OnClick_GachaTen);
        Btn_Probability.onClick.AddListener(OnClick_Probability);
        //Btn_Gacha30.onClick.AddListener(OnClick_Gacha30);
        m_RewardItemList = new List<ItemData>() { null, null, null };
        m_GachaPriceTextDefaultColor = Text_GachaOne_Price.color;

        Color color;
        if (ColorUtility.TryParseHtmlString("#FF2F2F", out color))
            m_RedColor = color;
    }
    #endregion

    #region Member Method
    public void SetData(SummonInfo Info)
    {
        m_Info = Info;
        
        var ItemCountData = ItemUtil.GetItemCount(Info.Price_2_Key);
        m_UserCoinCount = ItemCountData.OtherCount > -1 ? ItemCountData.Count + ItemCountData.OtherCount : ItemCountData.Count;

        DestroyExistingGachaObject();
        var Obj = ResourceLoader.LoadAsset<GameObject>($"prefab/ui/outgame/list/{m_Info.Prefab}", m_Info.Prefab);
        Obj_Gacha = Instantiate(Obj,Obj_GachaPos.transform);
        Text_GachaName.text = GString.Get(Info.Name);

        if(Info.Key == eGachaType.PickUp.GetHashCode())
        {
            Obj_PickUpInfo.SetActive(true);
            Obj_NormalInfo.SetActive(false);
            var PickUpObj = CharacterUtil.GetCharacterKeyIllust(Info.Pickup_Char_View);
            Obj_PickUp = Instantiate(PickUpObj, Obj_PickUpPos.transform);
            Obj_PickUp.transform.localScale = Vector3.one;
            Timer(DateTime.Parse(Info.EndDate));
            Obj_GachaTip.SetActive(true);
        }
        else
        {
            Obj_NormalInfo.SetActive(true);
            Obj_PickUpInfo.SetActive(false);
            Obj_GachaTip.SetActive(false);
        }
        
        UpdatePriceUI(Info);
        UpdateFreeGachaUI(Info);
        
        SetGachaRewardData();
        ActiveRewardItems();
    }
    
    public TutorialTargetUI GetTutorialTargetUI()
    {
        return TutorialTargetUI;
    }
    
    private void DestroyExistingGachaObject()
    {
        if (Obj_Gacha != null)
            Destroy(Obj_Gacha);
        
        if (Obj_PickUp != null)
            Destroy(Obj_PickUp);
    }
    
    private void UpdatePriceUI(SummonInfo info)
    {
        var DefaultItemKey = info.Price_2_Key;
        var DefaultPriceOne = info.Price_2_One;
        var DefaultPriceTen = info.Price_2_Ten;
        
        var HasDefaultItem = m_UserCoinCount > 0;

        var HasSufficientPriceOne = ContentsUtil.ValidateGachaPurchase(info, ShopContents.GachaOne) != ePurchaseState.NotEnough; // m_UserCoinCount >= DefaultPriceTen;
        var HasSufficientPriceTen = ContentsUtil.ValidateGachaPurchase(info, ShopContents.GachaTen) != ePurchaseState.NotEnough; // m_UserCoinCount >= DefaultPriceTen;

        if (!HasDefaultItem)
        {
            DefaultItemKey = info.Price_1_Key;
            DefaultPriceOne = info.Price_1_One;
            DefaultPriceTen = info.Price_1_Ten;
        }
        
        Img_GachaOne_Price.sprite = ItemUtil.GetItemIcon(DefaultItemKey);
        Text_GachaOne_Price.text = TextUtil.ConvertKMB(DefaultPriceOne); 

        Img_GachaTen_Price.sprite = ItemUtil.GetItemIcon(DefaultItemKey);
        Text_GachaTen_Price.text = TextUtil.ConvertKMB(DefaultPriceTen);
        
        Text_GachaOne_Price.color = HasSufficientPriceOne ? m_GachaPriceTextDefaultColor : m_RedColor;
        Text_GachaTen_Price.color = HasSufficientPriceTen ? m_GachaPriceTextDefaultColor : m_RedColor;
    }
    
    private void UpdateFreeGachaUI(SummonInfo info)
    {
        // 일일 무료 가챠 1회
        var FreeGachaCountOne = 0;
        if (User.UserDailyRecordData.FreeGacha_One.TryGetValue($"{info.Key}", out FreeGachaCountOne))
        {
            if (FreeGachaCountOne > 0)
            {
                Text_GachaOne_Price.text = string.Format(GString.Get("Gacha_Free_Time"), FreeGachaCountOne);
                Text_GachaOne_Price.color = m_GachaPriceTextDefaultColor;
                Img_GachaOne_Price.gameObject.SetActive(false);
            }
            else
                Img_GachaOne_Price.gameObject.SetActive(true);
        }

        // 일일 무료 가챠 10회
        var FreeGachaCountTen = 0;
        if (User.UserDailyRecordData.FreeGacha_Ten.TryGetValue($"{info.Key}", out FreeGachaCountTen))
        {
            if (FreeGachaCountTen > 0)
            {
                Text_GachaTen_Price.text = string.Format(GString.Get("Gacha_Free_Time"), FreeGachaCountTen);
                Text_GachaTen_Price.color = m_GachaPriceTextDefaultColor;
                Img_GachaTen_Price.gameObject.SetActive(false);
            }
            else
                Img_GachaTen_Price.gameObject.SetActive(true);
        }
        
        //// 일일 무료 가챠 30회
        //var FreeGachaCount30 = 0;
        //if (User.UserDailyRecordData.FreeGacha_30.TryGetValue($"{info.Key}", out FreeGachaCount30))
        //{
        //    if (FreeGachaCount30 > 0)
        //    {
        //        Text_Gacha30_Price.text = string.Format(GString.Get("Gacha_Free_Time"), FreeGachaCount30);
        //        Text_Gacha30_Price.color = m_GachaPriceTextDefaultColor;
        //        Img_Gacha30_Price.gameObject.SetActive(false);
        //    }
        //    else
        //        Img_Gacha30_Price.gameObject.SetActive(true);
        //}
    }

    private void SetGachaRewardData()
    {
        var gachaRewardList = DataManager.GetTable<SummonReward>(TableType.SummonReward)
            .Values
            .Where(SummonReward => SummonReward.Summon_Key == m_Info.Key && SummonReward.Unique_Key == m_Info.Pickup_Unique_Key).ToList();
            
        m_GachaCountList = gachaRewardList
            .Select(SummonReward => SummonReward.Summon_Count)
            .OrderBy(Count => Count)
            .ToList();
        
        var gachaCountMax = m_GachaCountList.Any() ? m_GachaCountList.Last() : 0;
        Slider_GachaCount.maxValue = gachaCountMax;
        
        Text_AccumulateCount.text = GString.Get("ui_message_0052");
        string Key = $"{m_Info.Key}_{m_Info.Pickup_Unique_Key}";
        if(User.UserAccumulateRecordData.GachaCount.ContainsKey($"{Key}"))
            m_UserGachaCount = User.UserAccumulateRecordData.GachaCount[$"{Key}"];
        else
            m_UserGachaCount = 0;

        Text_GachaCount.text = $"{m_UserGachaCount}";
        Slider_GachaCount.value = Mathf.Min(m_UserGachaCount, gachaCountMax);
        
        for (int index = 0; index < gachaRewardList.Count(); ++index)
        {
            Array_GachaRewardCount[index].text = $"x{m_GachaCountList[index]}";
            
            Element_Slot_ItemData item = new Element_Slot_ItemData();
            item.ItemDataType = ItemData.Make(gachaRewardList[index].Reward_1_Key, gachaRewardList[index].Reward_1_Count);
            item.IsShowCount = true;
            item.IsButtonActive = true;
            Array_GachaRewardSlot[index].SetItem(item);

            m_RewardItemList[index] = item.ItemDataType;
        }
    }

    private void ActiveRewardItems()
    {
        if (m_GachaCountList.Count <= 0) return;
        
        for (int Index = 0; Index < Array_GachaRewardSlot.Length; ++Index)
        {
            var RewardButton = Array_GachaRewardSlot[Index].GetComponent<Button>();
            RewardButton.onClick.RemoveAllListeners();
            var CapturedIndex = Index;
            
            User.UserAccumulateRecordData.GachaReward.TryGetValue($"{m_Info.Key}_{m_Info.Pickup_Unique_Key}", out var RewardList);
            if (RewardList == null) return;
            var RewardInfo = RewardList.SingleOrDefault(Reward => Reward.Count == m_GachaCountList[CapturedIndex]);
            var IsNotEnough = m_UserGachaCount < m_GachaCountList[Index];
            
            if (RewardInfo != null && RewardInfo.IsRewarded)
            {
                Arr_ReadyFX[Index].SetActive(false);
                Array_GachaRewardSlot[Index].SetSelect(true);
                continue;
            }
            
            if (IsNotEnough)
            {
                Arr_ReadyFX[Index].SetActive(false);
                Array_GachaRewardSlot[Index].SetSelect(false);
                RewardButton.onClick.AddListener(() =>
                {
                    UIManager.Instance.ShowToastMessage(GString.Get("ui_message_0053"));
                });
                continue;
            }
            
            // 보상을 받지 않은 상태
            Arr_ReadyFX[Index].SetActive(true);
            Array_GachaRewardSlot[Index].SetSelect(false);
            RewardButton.onClick.AddListener(() =>
            {
                var Data = Util.MakeData($"{m_Info.Key}", $"{m_Info.Pickup_Unique_Key}", $"{m_GachaCountList[CapturedIndex]}");
                 var Header = Util.MakeHeaderData(UserAccumulateContents.GetGachaReward, Data);
                 NetworkManager.Instance.SendContentsPacket(ContentsType.UserAccumulate, Header);
            });
        }
    }

    private void HandleNotEnough()
    {
        var Message = new MessageData();
        Message.Type = PopupType.OkCancel;
        switch (m_Info.Price_1_Key)
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

    private async void Timer(DateTime EndTime)
    {
        while (gameObject != null && gameObject.activeSelf)
        {
            var LeftTime = EndTime - GameManager.Instance.DateTimeNow;
            Text_PickUpLeftTime.text = string.Format(GString.Get("SP_Relay_Time"), LeftTime.Days, LeftTime.Hours, LeftTime.Minutes);
            await UniTask.WaitForSeconds(1f, cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }
    #endregion

    #region Button Event
    private void OnClick_GachaOne()
    {
        var PurchaseState = ContentsUtil.ValidateGachaPurchase(m_Info, ShopContents.GachaOne);
        Assert.AreNotEqual(PurchaseState, ePurchaseState.WithCoinAndGem);
        
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

    private void ProceedToPurchase(ShopContents ShopContentType)
    {
        var Datas = Util.MakeData($"{m_Info.Key}", $"{m_Info.Pickup_Unique_Key}");
        var Header = Util.MakeHeaderData(ShopContentType, Datas);
        NetworkManager.Instance.SendContentsPacket(ContentsType.Shop, Header);
    }

    private void OnClick_GachaTen()
    {
        var PurchaseState = ContentsUtil.ValidateGachaPurchase(m_Info, ShopContents.GachaTen);
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
                        new List<object> { m_UserCoinCount,
                            m_Info.Price_1_Ten - m_UserCoinCount * (m_Info.Price_1_Ten / 10),
                            m_Info, ShopContents.GachaTen
                        });
                }
                break;
            case ePurchaseState.NotEnough:
                HandleNotEnough();
                break;
        }
    }
    
    private void OnClick_Gacha30()
    {
        //var PurchaseState = ContentsUtil.ValidateGachaPurchase(m_Info, ShopContents.Gacha30);
        //switch (PurchaseState)
        //{
        //    case ePurchaseState.Free:
        //    case ePurchaseState.WithOnlyCoin:
        //    case ePurchaseState.WithOnlyGem:
        //        ProceedToPurchase(ShopContents.Gacha30);
        //        break;
        //    case ePurchaseState.WithCoinAndGem:
        //        {
        //            UIManager.Instance.Open<Popup_Coin_Consuming>(UI.Popup, 
        //                "UI/Popup/Popup_Coin_Consuming",
        //                new List<object> { m_UserCoinCount,
        //                    m_Info.Price_1_Thirty - m_UserCoinCount * (m_Info.Price_1_Thirty / 30),
        //                    m_Info, ShopContents.Gacha30
        //                });
        //        }
        //        break;
        //    case ePurchaseState.NotEnough:
        //        HandleNotEnough();
        //        break;
        //}
    }

    private async void OnClick_Probability()
    {
        await UniTask.Delay(200);
        UIManager.Instance.Open<Popup_Probability_info_Larva>(UI.Popup, "UI/Popup/Popup_Probability_Info_Larva", new List<object> { eGachaProbabilityType.Gacha, m_Info.Key });
    }
    #endregion
}