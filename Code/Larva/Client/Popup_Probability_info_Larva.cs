using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Probability_info_Larva : UIElement, IEnhancedScrollerDelegate
{
    #region Cashed Object
    [SerializeField] private EnhancedScroller Scroll_Probability = null;

    [SerializeField] private Button Btn_Close = null;
    #endregion

    #region Member Property
    private List<object> m_Args = null;
    private int m_TotalRate = 0;
    private Dictionary<int, List<GachaProbabilityData>> m_Probability = null;
    private GachaProbabilityCellView CellView_Probability = null;
    #endregion

    #region Override Method
    public override void Init()
    {
        m_Probability = new Dictionary<int, List<GachaProbabilityData>>();

        if(CellView_Probability == null)
        {
            var Obj = ResourceLoader.LoadAsset<GameObject>("Prefab/UI/Outgame/List/probability_larva_element_list", "probability_larva_element_list");
            CellView_Probability = Obj.GetComponent<GachaProbabilityCellView>();
        }

        Btn_Close.onClick.AddListener(OnClick_Close);
        BackKeyManager.Instance.RegistEvent(OnClick_Close);
    }

    public override void OnClose()
    {
        BackKeyManager.Instance.UnRegistEvent(OnClick_Close);
    }

    public override void OnOpen(List<object> Args)
    {
        m_Args = Args;
        eGachaProbabilityType GachaProbabilityType = (eGachaProbabilityType)m_Args[0];
        switch (GachaProbabilityType)
        {
            case eGachaProbabilityType.Gacha:
                {
                    eGachaType GachaType = (eGachaType)m_Args[1];

                    switch (GachaType)
                    {
                        case eGachaType.PickUp:
                            {
                                var SummonProbabilityPickup = DataManager.GetTable<SummonProbabilityPickup>(TableType.SummonProbabilityPickup).Values;

                                m_TotalRate = SummonProbabilityPickup.Sum(Data => Data.Probability);

                                for (int Grade = 1; Grade <= 4; Grade++)
                                {
                                    var GradeList = SummonProbabilityPickup.Where(Data => Data.Grade == Grade && Data.Probability > 0).ToList();
                                    List<GachaProbabilityData> ProbabilityList = Util.ConvertTo<List<GachaProbabilityData>>(GradeList);
                                    m_Probability.Add(Grade, ProbabilityList);
                                }
                            }
                            break;
                        case eGachaType.Advanced:
                            {
                                var SummonProbability_Advanced = DataManager.GetTable<SummonProbability_Advanced>(TableType.SummonProbability_Advanced).Values;

                                m_TotalRate = SummonProbability_Advanced.Sum(Data => Data.Probability);

                                for (int Grade = 1; Grade <= 4; Grade++)
                                {
                                    var GradeList = SummonProbability_Advanced.Where(Data => Data.Grade == Grade && Data.Probability > 0).ToList();
                                    List<GachaProbabilityData> ProbabilityList = Util.ConvertTo<List<GachaProbabilityData>>(GradeList);
                                    m_Probability.Add(Grade, ProbabilityList);
                                }
                            }
                            break;
                        case eGachaType.Legend:
                            {
                                var SummonProbabilityLegend = DataManager.GetTable<SummonProbabilityLegend>(TableType.SummonProbabilityLegend).Values;

                                m_TotalRate = SummonProbabilityLegend.Sum(Data => Data.Probability);

                                for (int Grade = 1; Grade <= 4; Grade++)
                                {
                                    var GradeList = SummonProbabilityLegend.Where(Data => Data.Grade == Grade && Data.Probability > 0).ToList();
                                    List<GachaProbabilityData> ProbabilityList = Util.ConvertTo<List<GachaProbabilityData>>(GradeList);
                                    m_Probability.Add(Grade, ProbabilityList);
                                }
                            }
                            break;
                    }
                }
                break;
            case eGachaProbabilityType.SummonTicket:
                {
                    string TicketKey = m_Args[1].ToString();
                    var SummonTicket = DataManager.GetTable<SummonTicket>(TableType.SummonTicket).Values.Where(Data => Data.Key == TicketKey).SingleOrDefault();

                    switch ((eTicketType)SummonTicket.Type)
                    {
                        case eTicketType.Random:
                            {
                                var SummonTicketRandom = DataManager.GetTable<SummonTicketRandom>(TableType.SummonTicketRandom).Values.Where(Data => Data.Group == SummonTicket.Group).ToList();

                                m_TotalRate = SummonTicketRandom.Sum(Data => Data.Probability);

                                for (int Grade = 1; Grade <= 4; Grade++)
                                {
                                    var GradeList = SummonTicketRandom.Where(Data => Data.Grade == Grade && Data.Probability > 0).ToList();
                                    List<GachaProbabilityData> ProbabilityList = Util.ConvertTo<List<GachaProbabilityData>>(GradeList);
                                    m_Probability.Add(Grade, ProbabilityList);
                                }
                            }
                            break;
                        case eTicketType.Select:
                            {
                                var SummonTicketSelect = DataManager.GetTable<SummonTicketSelect>(TableType.SummonTicketSelect).Values.Where(Data => Data.Group == SummonTicket.Group).ToList();
                                for (int Grade = 1; Grade <= 4; Grade++)
                                {
                                    var GradeList = SummonTicketSelect.Where(Data => Data.Grade == Grade).ToList();
                                    List<GachaProbabilityData> ProbabilityList = Util.ConvertTo<List<GachaProbabilityData>>(GradeList);
                                    m_Probability.Add(Grade, ProbabilityList);
                                }
                            }
                            break;
                        case eTicketType.Pick:
                            {
                                var SummonTicketPick = DataManager.GetTable<SummonTicketPick>(TableType.SummonTicketPick).Values.Where(Data => Data.Group == SummonTicket.Group).ToList();
                                for (int Grade = 1; Grade <= 4; Grade++)
                                {
                                    var GradeList = SummonTicketPick.Where(Data => Data.Grade == Grade).ToList();
                                    List<GachaProbabilityData> ProbabilityList = Util.ConvertTo<List<GachaProbabilityData>>(GradeList);
                                    m_Probability.Add(Grade, ProbabilityList);
                                }
                            }
                            break;
                    }
                }
                break;
        }


        Scroll_Probability.Delegate = this;
        Scroll_Probability.ReloadData();
    }

    public override void OnRefresh()
    {
    }
    #endregion

    #region Button Event
    private void OnClick_Close()
    {
        UIManager.Instance.Close<Popup_Probability_info_Larva>();
    }
    #endregion

    #region Scroll
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var CellView = Scroll_Probability.GetCellView(CellView_Probability) as GachaProbabilityCellView;
        CellView.SetData(4 - dataIndex, m_TotalRate, m_Probability[4 - dataIndex]);
        return CellView;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        if (m_Probability[4 - dataIndex].Count == 0)
            return 0f;

        int ListCount = m_Probability[4 - dataIndex].Count;
        int Count = ListCount / 5;
        if (ListCount % 5 > 0)
            Count++;
        return CellView_Probability.TitleHeight + (Count * CellView_Probability.ListHeight);
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return m_Probability.Count;
    }
    #endregion
}
