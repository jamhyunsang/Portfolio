using EnhancedUI.EnhancedScroller;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Popup_FixedGacha_Select : UIElement, IEnhancedScrollerDelegate
{
    #region Cashed Object
    [SerializeField] private Button Button_UseOne = null;
    [SerializeField] private Text Text_UseOne = null;
    [SerializeField] private Button Button_UseAll = null;
    [SerializeField] private Text Text_UseAll = null;
    [SerializeField] EnhancedScroller Scroller = null;
    [SerializeField] SkeletonGraphic SG_Box = null;
    #endregion

    #region Member Property
    private List<SummonTicketSelect> m_HeroKeyList = null;
    private LarvaSelectCellView m_SelectCellView = null;
    private int m_SelectedHeroKey;
    private ItemData m_ItemTypeData;
    #endregion

    #region Override Method
    public override void Init()
    {
        if (m_SelectCellView == null)
            m_SelectCellView = ResourceLoader.LoadAsset<GameObject>("Prefab/UI/OutGame/List/larva_select_list", "larva_select_list").GetComponent<LarvaSelectCellView>();

        Scroller.Delegate = this;
    }

    public override void OnClose()
    {
    }

    public override void OnOpen(List<object> Args)
    {
        if (Args.Count > 0)
        {
            m_ItemTypeData = Args[0] as ItemData;
            var Group = (int)Args[1];
            var UseCount = (int)Args[2];

            Text_UseOne.text = string.Format(GString.Get("ui_use_times"), 1);
            Text_UseAll.text = string.Format(GString.Get("ui_use_times"), UseCount);

            Button_UseOne.onClick.RemoveAllListeners();
            Button_UseAll.onClick.RemoveAllListeners();
            Button_UseOne.onClick.AddListener(() => OnClick_Use(1));
            Button_UseAll.onClick.AddListener(() => OnClick_Use(UseCount));

            var HeroList = DataManager.GetTable<SummonTicketSelect>(TableType.SummonTicketSelect).Values.Where(Data => Data.Group == Group);

            // 등급별 상자 스킨 적용
            var Grade = HeroList.Max(Data => Data.Grade);
            switch (Grade)
            {
                case 1:
                    SG_Box.Skeleton.SetSkin("skin_02");
                    break;
                case 2:
                    SG_Box.Skeleton.SetSkin("skin_05");
                    break;
                case 3:
                    SG_Box.Skeleton.SetSkin("skin_04");
                    break;
                case 4:
                    SG_Box.Skeleton.SetSkin("skin_03");
                    break;
                default:
                    SG_Box.Skeleton.SetSkin("skin_02");
                    break;
            }

            m_HeroKeyList = HeroList.ToList(); ;
            Scroller.ReloadData();
        }
    }

    public override void OnRefresh()
    {
    }
    #endregion

    #region Member Method

    private void OffSelectObj()
    {
        for (int count = 0; count <= Scroller.EndCellViewIndex; count++)
        {
            var Cell = Scroller.GetCellViewAtDataIndex(count) as LarvaSelectCellView;
            if (Cell != null)
                Cell.OffSelectObj();
        }
    }
    #endregion

    #region Button Event
    private void OnClick_Use(int UseCount)
    {
        if (m_SelectedHeroKey == 0)
            return;

        var Data = Util.MakeData(Util.ToJson(m_ItemTypeData), $"{m_SelectedHeroKey}", $"{UseCount}");
        var Header = Util.MakeHeaderData(ItemUseContents.UseSelectTicket, Data);
        NetworkManager.Instance.SendContentsPacket(ContentsType.ItemUse, Header,
            ReceiveAction: () => UIManager.Instance.Close<Popup_FixedGacha_Select>());
    }
    #endregion

    #region Scroller

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        LarvaSelectCellView CellView = scroller.GetCellView(m_SelectCellView) as LarvaSelectCellView;

        int StartIndex = cellIndex * 5;
        int MaxIndex = 5;

        if (m_HeroKeyList.Count <= StartIndex + MaxIndex)
            MaxIndex = m_HeroKeyList.Count - StartIndex;

        var Data = m_HeroKeyList.GetRange(StartIndex, MaxIndex);
        CellView.SetData(Data, m_SelectedHeroKey, OnClick_Hero);

        return CellView;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 160f;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return m_HeroKeyList.Count % 5 == 0 ? m_HeroKeyList.Count / 5 : m_HeroKeyList.Count / 5 + 1;
    }
    #endregion

    #region Action
    private void OnClick_Hero(int HeroKey)
    {
        OffSelectObj();
        m_SelectedHeroKey = HeroKey;
    }
    #endregion
}