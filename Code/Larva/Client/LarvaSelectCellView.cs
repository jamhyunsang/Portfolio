using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LarvaSelectCellView : EnhancedScrollerCellView
{
    #region Cashed Object
    [Header("Item")]
    [SerializeField] List<Element_Slot> Slots = null;
    #endregion

    #region Member Property
    private UnityAction<int> m_SelectAction = null;
    #endregion

    #region Member Method
    public void SetData(List<SummonTicketSelect> HeroKeyList, int SelectedHeroKey, UnityAction<int> Action)
    {
        Init();
        m_SelectAction = Action;

        for (int Index = 0; Index < HeroKeyList.Count; ++Index)
        {
            Element_Slot_HeroData Data = new Element_Slot_HeroData();
            Data.HeroKey = HeroKeyList[Index].CharIdx;
            Data.Type = HeroKeyList[Index].Type;
            Data.Grade = HeroKeyList[Index].Grade;
            Data.IsShowSelect = HeroKeyList[Index].CharIdx == SelectedHeroKey;

            var TempIndex = Index;
            Data.IsButtonActive = true;
            Data.Action = delegate { OnClick_Hero(TempIndex, Data.HeroKey); };

            Slots[Index].gameObject.SetActive(true);
            Slots[Index].SetHero_OutGame(Data);
        }
    }

    private void Init()
    {
        for (int count = 0; count < Slots.Count; count++)
        {
            Slots[count].gameObject.SetActive(false);
        }
    }
    #endregion

    public void OffSelectObj()
    {
        for (int Index = 0; Index < Slots.Count; ++Index)
        {
            Slots[Index].SetSelect(false);
        }      
    }

    private void OnClick_Hero(int Index, int HeroKey)
    {
        m_SelectAction?.Invoke(HeroKey);
        Slots[Index].SetSelect(true);
    }
}
